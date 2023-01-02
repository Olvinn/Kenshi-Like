using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Units.Weapons;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Views
{
    public class UnitView : MonoBehaviour
    {
        public Unit Model { get; private set; }
        public List<UnitView> Sensed => sense.views;
        public Vector3 Position { get; private set; }
        public UnitView Target;

        [field:SerializeField] public MovementStatus MovementStatus { get; private set; }
        [field:SerializeField] public FightStatus FightStatus { get; private set; }
        
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject selection;
        [SerializeField] private UnitAnimationEventCatcher animationEventCatcher;
        [SerializeField] private UnitAttack attack;
        [SerializeField] private TriggerDetector sense;
        [SerializeField] private UnitVisuals visuals;
        
        private Transform _destinationTransform;
        private Vector3 _destinationPos;
        private Action<List<UnitView>> _onHitUnits;
        private float _animationSpeed = 1f;

        private void Awake()
        {
            selection.SetActive(false);
            
            animationEventCatcher.OnHitFront += HitFront;
            animationEventCatcher.OnGetDamageComplete += GetDamageComplete;
            animationEventCatcher.OnAttackComplete += CompleteAttack;
        }

        private void Update()
        {
            if (!agent.enabled)
                return;

            Position = transform.position;
            if (!agent.pathPending && MovementStatus == MovementStatus.Moving)
            {
                agent.isStopped = false;
                if (_destinationTransform != null)
                    agent.SetDestination(_destinationTransform.position);
                else
                    agent.SetDestination(_destinationPos);
            }
            
            if (!agent.pathPending && agent.remainingDistance < 1f)
            {
                if (MovementStatus == MovementStatus.Moving)
                {
                    MovementStatus = MovementStatus.Waiting;
                    agent.isStopped = true;
                    _destinationTransform = null;
                }
            }

            if (Target != null)
            {
                transform.LookAt(Target.transform.position);
                
                if (Target.Model.IsDead)
                    Target = null;
            }

            animator.speed = _animationSpeed;
            switch (FightStatus)
            {
                case FightStatus.AwaitingAttacking:
                    animator.Play("Attack");
                    FightStatus = FightStatus.Attacking;
                    break;
                case FightStatus.AwaitingDamaging:
                    animator.Play("GetDamage");
                    FightStatus = FightStatus.Damaging;
                    break;
            }
        }

        private void OnDisable()
        {
            agent.enabled = false;
            sense.enabled = false;
            attack.enabled = false;
            var c = GetComponent<CapsuleCollider>();
            c.radius = 0f;
            c.enabled = false;
        }

        private void OnEnable()
        {
            agent.enabled = true;
            sense.enabled = true;
            attack.enabled = true;
            var c = GetComponent<CapsuleCollider>();
            c.radius = .5f;
            c.enabled = true;
        }

        private void OnDrawGizmos()
        {
            GUI.color = Color.black;
            GUI.backgroundColor = Color.white;
            if (Model != null)
            {
                Handles.Label(transform.position, $"{name}: {Model.HPPercentage}");
                var cs = Model.GetListOfCommands();
                float offset = .1f;
                foreach (var c in cs)
                {
                    Handles.Label(transform.position + Vector3.down * offset, $"Command: {c}");
                    offset += .1f;
                }
            }
        }

        public void InjectModel(Unit model)
        {
            Model = model;
            agent.enabled = true;
        }

        /// <summary>
        /// Set the units appearance
        /// TODO: separate appearance data from Character
        /// </summary>
        /// <param name="appearance"></param>
        public void SetAppearance(Character appearance)
        {
            var arr = Enum.GetValues(typeof(UnitColorType)).Cast<UnitColorType>().ToList();
            foreach (var color in arr)
            {
                visuals.SetColor(color, appearance.GetColor(color));
            }
        }

        /// <summary>
        /// Turns off most UnitView logic and plays dying animation
        /// </summary>
        public void Die()
        {
            StopAllCoroutines();
            animator.Play("Die");
            Deselect();
            this.enabled = false;
        }

        /// <summary>
        /// Set destination for move to
        /// </summary>
        /// <param name="destination"></param>
        public void MoveTo(Vector3 destination)
        {
            _destinationPos = destination;
            MovementStatus = MovementStatus.Moving;
            _destinationTransform = null;
        }

        /// <summary>
        /// Set destination for move to
        /// </summary>
        /// <param name="destination"></param>
        public void MoveTo(Transform destination)
        {
            _destinationTransform = destination;
            MovementStatus = MovementStatus.Moving;
        }

        /// <summary>
        /// Select unit
        /// </summary>
        public void Select()
        {
            selection.SetActive(true);
        }

        /// <summary>
        /// Deselect unit
        /// </summary>
        public void Deselect()
        {
            selection.SetActive(false);
        }

        /// <summary>
        /// Play fight-ready animation
        /// </summary>
        public void PerformFightReadyAnimation()
        {
            animator.SetBool("Provoked", true);
        }

        /// <summary>
        /// Play Idle animation
        /// </summary>
        public void PerformIdleAnimation()
        {
            animator.SetBool("Provoked", false);
        }

        /// <summary>
        /// Checking can this unit attack target due their Views
        /// Multithreading safe
        /// </summary>
        /// <param name="target">Target which want to attack</param>
        /// <returns></returns>
        public bool CanAttack(UnitView target)
        {
            return Vector3.Distance(target.Position, Position) < 2f;
        }

        /// <summary>
        /// Play animation to hit enemy. If hit will successful, the callback will called.
        /// Multithreading safe
        /// </summary>
        /// <param name="callback">UnitViews that has been hit</param>
        public void PerformAttackAnimation(float attackRate, Action<List<UnitView>> callback)
        {
            FightStatus = FightStatus.AwaitingAttacking;
            _animationSpeed = attackRate;
            _onHitUnits = callback;
        }

        /// <summary>
        /// Set unit max speed
        /// </summary>
        /// <param name="speed"></param>
        public void SetMaxSpeed(float speed)
        {
            agent.speed = speed;
        }

        /// <summary>
        /// Play animation representing taking damage. It will interrupt any other animation
        /// Multithreading safe
        /// </summary>
        public void PerformGetDamageAnimation()
        {
            _animationSpeed = 1f;
            FightStatus = FightStatus.AwaitingDamaging;
        }

        /// <summary>
        /// Rotate UnitView on target
        /// Multithreading safe
        /// </summary>
        /// <param name="target"></param>
        public void RotateOn(UnitView target)
        {
            this.Target = target;
        }

        public void SetPosition(Vector3 pos)
        {
            agent.Warp(pos);
        }

        private void HitFront()
        {
            attack.BroadcastDamageInFront(_onHitUnits);
            FightStatus = FightStatus.Waiting;
        }

        private void CompleteAttack()
        {
            FightStatus = FightStatus.Waiting;
            _animationSpeed = 1f;
            _onHitUnits = null;
        }

        private void GetDamageComplete()
        {
            FightStatus = FightStatus.Waiting;
        }
    }

    public enum MovementStatus
    {
        Waiting,
        Moving,
    }

    public enum FightStatus
    {
        Waiting,
        AwaitingAttacking,
        Attacking,
        AwaitingDamaging,
        Damaging,
        Parrying
    }
}

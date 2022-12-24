using System;
using System.Collections.Generic;
using Units.Weapons;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public class UnitView : MonoBehaviour
    {
        public Unit Model { get; private set; }
        public event Action<UnitView> OnUnitSensed;
        public List<UnitView> Sensed => sense.views;
        public Vector3 Position { get; private set; }

        [field:SerializeField] public MovementStatus MovementStatus { get; private set; }
        [field:SerializeField] public FightStatus FightStatus { get; private set; }
        
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject selection;
        [SerializeField] private UnitAnimationEventCatcher animationEventCatcher;
        [SerializeField] private UnitAttack attack;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private TriggerDetector sense;
        
        private UnitView _target;
        private Transform _destinationTransform;
        private Vector3 _destinationPos;
        private Action<List<UnitView>> _onHitUnits;

        private void Awake()
        {
            selection.SetActive(false);
            
            animationEventCatcher.OnHitFront += HitFront;
            animationEventCatcher.OnGetDamageComplete += GetDamageComplete;
            animationEventCatcher.OnAttackComplete += CompleteAttack;
            
            sense.OnUnitSensed += OnUnitSensed;
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

            if (_target != null)
            {
                transform.LookAt(_target.transform.position);
                
                if (_target.Model.IsDead)
                    _target = null;
            }

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

        private void OnDrawGizmos()
        {
            GUI.color = Color.black;
            GUI.backgroundColor = Color.white;
            if (Application.isPlaying)
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
        }

        public void SetColor(Color color)
        {
            meshRenderer.material.color = color;
        }

        /// <summary>
        /// Turns off most UnitView logic and plays dying animation
        /// </summary>
        public void Die()
        {
            StopAllCoroutines();
            meshRenderer.material.color = Color.black;
            agent.enabled = false;
            animator.Play("Die");
            Deselect();
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
        /// </summary>
        /// <param name="target">Target which want to attack</param>
        /// <returns></returns>
        public bool CanAttack(UnitView target)
        {
            return Vector3.Distance(target.Position, Position) < 2f;
        }

        /// <summary>
        /// Play animation to hit enemy. If hit will successful, the callback will called.
        /// </summary>
        /// <param name="callback">UnitViews that has been hit</param>
        public void PerformAttackAnimation(Action<List<UnitView>> callback)
        {
            FightStatus = FightStatus.AwaitingAttacking;

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
        /// </summary>
        public void PerformGetDamageAnimation()
        {
            FightStatus = FightStatus.AwaitingDamaging;
        }

        /// <summary>
        /// Rotate UnitView on target
        /// </summary>
        /// <param name="target"></param>
        public void RotateOn(UnitView target)
        {
            _target = target;
        }

        private void HitFront()
        {
            attack.BroadcastDamageInFront(_onHitUnits);
            FightStatus = FightStatus.Waiting;
        }

        private void CompleteAttack()
        {
            FightStatus = FightStatus.Waiting;
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

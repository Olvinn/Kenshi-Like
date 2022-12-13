using System;
using Units.Weapons;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public class UnitView : MonoBehaviour
    {
        public Action OnReachDestination, OnCompleteHit, OnCompleteGetDamage;
        public Unit Model { get; private set; }

        [field:SerializeField] public MovementStatus MovementStatus { get; private set; }
        [field:SerializeField] public FightStatus FightStatus { get; private set; }
        
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject selection;
        [SerializeField] private UnitAnimationEventCatcher animationEventCatcher;
        [SerializeField] private WeaponTriggerDetector weaponTrigger;
        [SerializeField] private MeshRenderer meshRenderer;

        [SerializeField] private Color color;
        private UnitView _target;
        private Action<UnitView> _onHitUnit;

        private void Awake()
        {
            meshRenderer.material.color = color;
            selection.SetActive(false);
            animationEventCatcher.OnHitComplete += HitComplete;
            animationEventCatcher.OnGetDamageComplete += GetDamageComplete;
        }

        private void Update()
        { 
            if (!agent.enabled)
                return;
            
            if (!agent.pathPending && agent.remainingDistance < 1f)
            {
                if (MovementStatus == MovementStatus.Moving)
                {
                    MovementStatus = MovementStatus.Aimless;
                    OnReachDestination?.Invoke();
                }
            }

            if (_target != null)
            {
                transform.LookAt(_target.transform.position);
            }
        }

        public void InjectModel(Unit model)
        {
            Model = model;
        }

        /// <summary>
        /// Turns off most UnitView logic and plays dying animation
        /// </summary>
        public void Die()
        {
            StopAllCoroutines();
            meshRenderer.material.color = Color.black;
            agent.enabled = false;
            animator.SetTrigger("Die");
        }

        /// <summary>
        /// Set destination for move to
        /// </summary>
        /// <param name="destination"></param>
        public void MoveTo(Vector3 destination)
        {
            agent.SetDestination(destination);
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
            return Vector3.Distance(target.transform.position, transform.position) < 2f;
        }

        /// <summary>
        /// Play animation to hit enemy. If hit will successful, the callback will called.
        /// Callback can be called multiple times with different UnitViews
        /// </summary>
        /// <param name="callback"></param>
        public void PerformAttackAnimation(Action<UnitView> callback)
        {
            FightStatus = FightStatus.Attacking;
            animator.Play("Attack");

            _onHitUnit = callback;
            weaponTrigger.OnTriggerWithUnit = (unit) => CheckAttackedUnit(unit, callback);
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
        /// Play animation represents taking damage. It will interrupt any other animation
        /// </summary>
        public void PerformGetDamageAnimation()
        {
            if (FightStatus == FightStatus.Attacking)
                HitComplete();
            FightStatus = FightStatus.GettingDamage;
            animator.SetTrigger("GetDamage");
        }

        /// <summary>
        /// Rotate UnitView on target
        /// </summary>
        /// <param name="target"></param>
        public void RotateOn(UnitView target)
        {
            _target = target;
        }

        private void CheckAttackedUnit(UnitView target, Action<UnitView> callback)
        {
            if (target == this)
                return;
            callback?.Invoke(target);
        }

        private void HitComplete()
        {
            if (_onHitUnit != null)
                weaponTrigger.OnTriggerWithUnit = null;
            _onHitUnit = null;
            FightStatus = FightStatus.Idle;
            OnCompleteHit?.Invoke();
        }

        private void GetDamageComplete()
        {
            FightStatus = FightStatus.Idle;
            OnCompleteGetDamage?.Invoke();
        }
    }

    public enum MovementStatus
    {
        Aimless,
        Moving,
    }

    public enum FightStatus
    {
        Idle,
        Attacking,
        GettingDamage,
        Parrying
    }
}

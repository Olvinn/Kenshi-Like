using System;
using Data;
using OldUnits.Views.IK;
using UnityEngine;

namespace OldUnits
{
    public class AnimationController : MonoBehaviour
    {
        public AnimationControllerState State { get; private set; }
        public float attackSpeed;
        public Transform AnimatorTransform => newAnimator.transform;
        public bool CanMove => State is not (AnimationControllerState.Damaging or AnimationControllerState.Dodging or AnimationControllerState.Attacking);
        public event Action OnHitBasic;
        
        [SerializeField] private Animator newAnimator;
        [SerializeField] private UnitAnimationEventCatcher newCatcher;
        [SerializeField] private IKController ik;

        private Vector3 _mov, _movGoal;
        private float _swimmingLayerWeight;

        private void Awake()
        {
            newCatcher.OnHitBasic += HitBasic;
            newCatcher.OnGetDamageComplete += GetDamageComplete;
            newCatcher.OnAttackComplete += AttackComplete;
            newCatcher.OnDodgingComplete += DodgeComplete;
        }

        private void Update()
        {
            var constants = GameContext.Instance?.Constants;
            if (constants == null)
                return;
            
            _mov = Vector3.Lerp(_mov, _movGoal, Time.deltaTime * constants.AnimationLerpSpeed);
                
            newAnimator.SetFloat("Speed", _mov.z);
            newAnimator.SetFloat("Strafe", _mov.x);
            
            newAnimator.speed = 1;
            ik.leftArmPos = null;
            ik.legsIKOn = true;
            
            switch (State)
            {
                case AnimationControllerState.Blocking:
                    _swimmingLayerWeight -= Time.deltaTime * constants.AnimationLayerTransitionSpeed;
                    break;
                case AnimationControllerState.Attacking:
                    newAnimator.speed = attackSpeed;
                    _swimmingLayerWeight -= Time.deltaTime * constants.AnimationLayerTransitionSpeed;
                    break;
                case AnimationControllerState.Swimming:
                    ik.legsIKOn = false;
                    _swimmingLayerWeight += Time.deltaTime * constants.AnimationLayerTransitionSpeed;
                    break;
                default:
                    _swimmingLayerWeight -= Time.deltaTime * constants.AnimationLayerTransitionSpeed;
                    break;
            }
            _swimmingLayerWeight = Mathf.Clamp01(_swimmingLayerWeight);
            newAnimator.SetLayerWeight(2, _swimmingLayerWeight);
        }

        private void Start()
        {
            State = AnimationControllerState.Idle;
        }

        public void UpdateMovingAnimation(Vector3 mov)
        {
            if (!newAnimator.isActiveAndEnabled || State is AnimationControllerState.Damaging or AnimationControllerState.Dodging)
            {
                return;
            }

            _movGoal = mov;
        }

        public void Run()
        {
            if (State == AnimationControllerState.Swimming)
                State = AnimationControllerState.Idle;
        }

        public void Swim()
        {
            State = AnimationControllerState.Swimming;
        }

        public void PerformAttackAnimation(Action callback, float animationSpeed = 1f)
        {
            if (!newAnimator.isActiveAndEnabled || 
                State is not (AnimationControllerState.Idle or AnimationControllerState.Blocking))
            {
                callback?.Invoke();
                return;
            }
            
            newAnimator.SetBool("Block", false);
            newAnimator.SetTrigger("AttackBasic");
            attackSpeed = animationSpeed;
            State = AnimationControllerState.Attacking;
        }

        public void PerformGetDamageAnimation(Action callback)
        {
            if (!newAnimator.isActiveAndEnabled)
            {
                callback?.Invoke();
                return;
            }
            
            newAnimator.SetBool("Block", false);
            newAnimator.Play("Get damage");
            State = AnimationControllerState.Damaging;
        }

        public void PerformDodgeAnimation(Action callback)
        {
            if (!newAnimator.isActiveAndEnabled || 
                State is not (AnimationControllerState.Idle or AnimationControllerState.Blocking or AnimationControllerState.Damaging))
            {
                callback?.Invoke();
                return;
            }

            newAnimator.SetBool("Block", false);
            newAnimator.SetTrigger("Dodge");
            State = AnimationControllerState.Dodging;
        }

        public void PerformBlockAnimation()
        {
            if (!newAnimator.isActiveAndEnabled || State != AnimationControllerState.Idle)
                return;
            
            newAnimator.SetBool("Block", true);
            State = AnimationControllerState.Blocking;
        }

        public void PerformDyingAnimation()
        {
            
        }

        /// <summary>
        /// Play fight-ready animation
        /// </summary>
        public void PerformFightReadyAnimation()
        {
            if (!newAnimator.isActiveAndEnabled)
                return;

            newAnimator.SetLayerWeight(1, 1);
        }

        /// <summary>
        /// Play Idle animation
        /// </summary>
        public void PerformIdleAnimation()
        {
            if (!newAnimator.isActiveAndEnabled)
                return;

            newAnimator.SetLayerWeight(1, 0);
        }

        private void HitBasic()
        {
            OnHitBasic?.Invoke();
        }

        private void AttackComplete()
        {
            if (State == AnimationControllerState.Attacking)
                State = AnimationControllerState.Idle;
        }

        private void GetDamageComplete()
        {
            if (State == AnimationControllerState.Damaging)
                State = AnimationControllerState.Idle;
        }

        private void DodgeComplete()
        {
            if (State == AnimationControllerState.Dodging)
                State = AnimationControllerState.Idle;
            AnimatorTransform.localPosition = Vector3.zero;
        }
    }

    public enum AnimationControllerState
    {
        Idle,
        Attacking,
        Damaging,
        Dodging,
        Blocking,
        Swimming
    }
}

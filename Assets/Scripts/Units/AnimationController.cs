using System;
using System.Collections;
using Units.Views.IK;
using UnityEngine;

namespace Units
{
    public class AnimationController : MonoBehaviour
    {
        public AnimationControllerState State { get; private set; }
        public float attackSpeed;
        public Transform AnimatorTransform => newAnimator.transform;
        public bool CanMove => State is not (AnimationControllerState.Damaging or AnimationControllerState.Dodging);
        public event Action OnHitBasic;
        
        [SerializeField] private Animator newAnimator;
        [SerializeField] private UnitAnimationEventCatcher newCatcher;
        [SerializeField] private IKController ik;
        [SerializeField] private Transform blockFront;

        private Action _onCompleteAnimation;

        private void Awake()
        {
            newCatcher.OnHitBasic += HitBasic;
            newCatcher.OnGetDamageComplete += GetDamageComplete;
            newCatcher.OnAttackComplete += AttackComplete;
            newCatcher.OnDodgingComplete += DodgeComplete;
        }

        private void Update()
        {
            if (State == AnimationControllerState.Blocking)
                ik.armsPos = blockFront;
            else
                ik.armsPos = null;

            if (State == AnimationControllerState.Attacking)
                newAnimator.speed = attackSpeed;
            else
                newAnimator.speed = 1;
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
                
            newAnimator.SetFloat("Speed", mov.z);
            newAnimator.SetFloat("Strafe", mov.x);
        }

        public void PerformAttackAnimation(Action callback, float animationSpeed = 1f)
        {
            if (!newAnimator.isActiveAndEnabled || State is not (AnimationControllerState.Idle or AnimationControllerState.Blocking))
            {
                callback?.Invoke();
                return;
            }
            
            _onCompleteAnimation = callback;
            newAnimator.SetTrigger("AttackBasic");
            attackSpeed = animationSpeed;
            State = AnimationControllerState.Attacking;

            StopAllCoroutines();
            StartCoroutine(Saver(3f));
        }

        public void PerformGetDamageAnimation(Action callback)
        {
            if (!newAnimator.isActiveAndEnabled)
            {
                callback?.Invoke();
                return;
            }
            
            _onCompleteAnimation = callback;
            newAnimator.Play("Get damage");
            State = AnimationControllerState.Damaging;

            StopAllCoroutines();
            StartCoroutine(Saver(3f));
        }

        public void PerformDodgeAnimation(Action callback)
        {
            if (!newAnimator.isActiveAndEnabled || State is not (AnimationControllerState.Idle or AnimationControllerState.Blocking))
            {
                callback?.Invoke();
                return;
            }

            _onCompleteAnimation = callback;
            newAnimator.SetTrigger("Dodge");
            State = AnimationControllerState.Dodging;

            StopAllCoroutines();
            StartCoroutine(Saver(2f));
        }

        public void PerformBlockAnimation()
        {
            if (!newAnimator.isActiveAndEnabled || State != AnimationControllerState.Idle)
                return;
            
            State = AnimationControllerState.Blocking;
            StopAllCoroutines();
            StartCoroutine(Saver(1f));
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
            _onCompleteAnimation?.Invoke();
            State = AnimationControllerState.Idle;
            StopAllCoroutines();
        }

        private void GetDamageComplete()
        {
            _onCompleteAnimation?.Invoke();
            State = AnimationControllerState.Idle;
            StopAllCoroutines();
        }

        private void DodgeComplete()
        {
            _onCompleteAnimation?.Invoke();
            State = AnimationControllerState.Idle;
            StopAllCoroutines();
            AnimatorTransform.localPosition = Vector3.zero;
        }

        IEnumerator Saver(float time)
        {
            yield return new WaitForSeconds(time);
            _onCompleteAnimation?.Invoke();
            _onCompleteAnimation = null;
            State = AnimationControllerState.Idle;
        }
    }

    public enum AnimationControllerState
    {
        Idle,
        Attacking,
        Damaging,
        Dodging,
        Blocking
    }
}

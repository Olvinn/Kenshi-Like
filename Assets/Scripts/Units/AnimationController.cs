using System;
using System.Collections;
using UnityEngine;

namespace Units
{
    public class AnimationController : MonoBehaviour
    {
        public AnimationControllerState State { get; private set; }
        public Transform AnimatorTransform => newAnimator.transform;
        public bool CanMove => State is not (AnimationControllerState.Damaging or AnimationControllerState.Dodging);
        public event Action OnHitBasic;
        
        [SerializeField] private Animator oldAnimator;
        [SerializeField] private Animator newAnimator;
        [SerializeField] private UnitAnimationEventCatcher oldCatcher;
        [SerializeField] private UnitAnimationEventCatcher newCatcher;

        private Action _onCompleteAnimation;

        private void Awake()
        {
            newCatcher.OnHitBasic += HitBasic;
            oldCatcher.OnGetDamageComplete += GetDamageComplete;
            newCatcher.OnAttackComplete += AttackComplete;
            newCatcher.OnDodgingComplete += DodgeComplete;
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
            if (!oldAnimator.isActiveAndEnabled || State != AnimationControllerState.Idle)
            {
                callback?.Invoke();
                return;
            }
            
            _onCompleteAnimation = callback;
            newAnimator.SetTrigger("AttackBasic");
            State = AnimationControllerState.Attacking;

            StartCoroutine(Saver(3f));
        }

        public void PerformGetDamageAnimation(Action callback)
        {
            if (!oldAnimator.isActiveAndEnabled || State != AnimationControllerState.Idle)
            {
                callback?.Invoke();
                return;
            }
            
            _onCompleteAnimation = callback;
            oldAnimator.Play("GetDamage");
            State = AnimationControllerState.Damaging;

            StartCoroutine(Saver(3f));
        }

        public void PerformDodgeAnimation(Action callback)
        {
            if (!newAnimator.isActiveAndEnabled || State != AnimationControllerState.Idle)
            {
                callback?.Invoke();
                return;
            }

            _onCompleteAnimation = callback;
            newAnimator.SetTrigger("Dodge");
            State = AnimationControllerState.Dodging;

            StartCoroutine(Saver(2f));
        }

        public void PerformDyingAnimation()
        {
            if (!oldAnimator.isActiveAndEnabled)
                return;
            
            oldAnimator.Play("Die");
        }

        /// <summary>
        /// Play fight-ready animation
        /// </summary>
        public void PerformFightReadyAnimation()
        {
            if (!oldAnimator.isActiveAndEnabled)
                return;

            newAnimator.SetLayerWeight(1, 1);
        }

        /// <summary>
        /// Play Idle animation
        /// </summary>
        public void PerformIdleAnimation()
        {
            if (!oldAnimator.isActiveAndEnabled)
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
            _onCompleteAnimation = null;
            State = AnimationControllerState.Idle;
            StopAllCoroutines();
        }

        private void GetDamageComplete()
        {
            _onCompleteAnimation?.Invoke();
            _onCompleteAnimation = null;
            State = AnimationControllerState.Idle;
            StopAllCoroutines();
        }

        private void DodgeComplete()
        {
            _onCompleteAnimation?.Invoke();
            _onCompleteAnimation = null;
            State = AnimationControllerState.Idle;
            StopAllCoroutines();
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
        Dodging
    }
}

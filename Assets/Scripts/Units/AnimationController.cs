using System;
using System.Collections;
using Units.Views.IK;
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
        [SerializeField] private IKController ik;
        [SerializeField] private Transform blockFront;

        private Action _onCompleteAnimation;

        private void Awake()
        {
            newCatcher.OnHitBasic += HitBasic;
            oldCatcher.OnGetDamageComplete += GetDamageComplete;
            newCatcher.OnAttackComplete += AttackComplete;
            newCatcher.OnDodgingComplete += DodgeComplete;
        }

        private void Update()
        {
            if (State == AnimationControllerState.Blocking)
                ik.armsPos = blockFront;
            else
                ik.armsPos = null;
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
            if (!oldAnimator.isActiveAndEnabled || State is not (AnimationControllerState.Idle or AnimationControllerState.Blocking))
            {
                callback?.Invoke();
                return;
            }
            
            _onCompleteAnimation = callback;
            newAnimator.SetTrigger("AttackBasic");
            newAnimator.speed = animationSpeed;
            State = AnimationControllerState.Attacking;

            StopAllCoroutines();
            StartCoroutine(Saver(3f));
        }

        public void PerformGetDamageAnimation(Action callback)
        {
            if (!oldAnimator.isActiveAndEnabled || State is not (AnimationControllerState.Idle or AnimationControllerState.Blocking))
            {
                callback?.Invoke();
                return;
            }
            
            _onCompleteAnimation = callback;
            oldAnimator.Play("GetDamage");
            newAnimator.speed = 1;
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
            newAnimator.speed = 1;
            State = AnimationControllerState.Dodging;

            StopAllCoroutines();
            StartCoroutine(Saver(2f));
        }

        public void PerformBlockAnimation()
        {
            if (!newAnimator.isActiveAndEnabled || State is AnimationControllerState.Attacking or AnimationControllerState.Damaging)
                return;
            
            State = AnimationControllerState.Blocking;
            StopAllCoroutines();
            StartCoroutine(Saver(5f));
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

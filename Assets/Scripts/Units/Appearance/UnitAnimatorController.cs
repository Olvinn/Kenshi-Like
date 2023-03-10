using System.Collections;
using UnityEngine;

namespace Units.Appearance
{
    public class UnitAnimatorController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float lod0Threshold = 30, lod1Threshold = 60, updateFrequencyMultiplier = .1f;
        
        private int _speedHash, _strafeHash;
        private float _savedTime;
        private int _framesCounter, _framesUpdateDelay;
        private int _lod;
        private bool _forceAnimatorToBeActive;

        private Coroutine _greatSwordLayerTransition;
        
        private void Awake()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();

            _animator.keepAnimatorControllerStateOnDisable = true;
            _speedHash = Animator.StringToHash("Speed");
            _strafeHash = Animator.StringToHash("Strafe");
        }

        private IEnumerator Start()
        {
            _animator.enabled = true;
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            while (stateInfo.normalizedTime < 1.0f)
            {
                yield return null;
                stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            }
            if (!_forceAnimatorToBeActive)
                _animator.enabled = false;
        }

        private void Update()
        {
            if (_forceAnimatorToBeActive)
                return;
            
            _framesCounter--;
            if (_framesCounter >= 0)
                return;
            
            _animator.Update(Time.time - _savedTime);
            _savedTime = Time.time;
            _framesCounter = _framesUpdateDelay;
        }

        public void ApplyVelocity(Vector3 velocity)
        {
            _animator.SetFloat(_speedHash, velocity.z);
            _animator.SetFloat(_strafeHash, velocity.x);
        }

        public void PlayAttack()
        {
            _animator.Play("Attack 1");
        }

        public void PlayDead()
        {
            _animator.Play("Death");
        }

        public void UpdateDistanceToCamera(float distance)
        {
            int newUpdateDelay = (int)(distance * updateFrequencyMultiplier);
            _framesCounter += newUpdateDelay - _framesUpdateDelay;
            _framesUpdateDelay = newUpdateDelay;
        }

        public void SetAnimatorActive(bool value)
        {
            _forceAnimatorToBeActive = value;
            _animator.enabled = _forceAnimatorToBeActive;
        }

        public void ActivateGreatSwordLayer(bool value)
        {
            if (_greatSwordLayerTransition != null)
                StopCoroutine(_greatSwordLayerTransition);
            _greatSwordLayerTransition = StartCoroutine(GreatSwordLayerTransition(value));
        }

        private IEnumerator GreatSwordLayerTransition(bool isActive, float transitionTime = .5f)
        {
            float currentState = _animator.GetLayerWeight(1);
            float goal = isActive ? 1 : 0;
            while (Mathf.Abs(currentState - goal) > .01f)
            {
                currentState = Mathf.MoveTowards(currentState, goal, Time.deltaTime / transitionTime);
                _animator.SetLayerWeight(1, currentState);
                yield return null;
            }
            _animator.SetLayerWeight(1, goal);
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
        }
#endif
    }
}

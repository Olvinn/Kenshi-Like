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

        private int _currentActiveLayer;
        private Coroutine _layerTransition;
        
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

        public void PlayHitReaction()
        {
            _animator.Play("Hit reaction");
        }

        public void SetActiveLayer(int layer)
        {
            if (_currentActiveLayer == layer)
                return;
            
            if (_layerTransition != null)
                StopCoroutine(_layerTransition);
            _layerTransition = StartCoroutine(LayerTransition(layer));
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

        private IEnumerator LayerTransition(int layer)
        {
            float newLayerWeight = _animator.GetLayerWeight(layer);
            float currentLayerWeight = _animator.GetLayerWeight(_currentActiveLayer);
            while (Mathf.Abs(newLayerWeight - 1) > .01f)
            {
                if (_currentActiveLayer > 0)
                    currentLayerWeight = Mathf.MoveTowards(currentLayerWeight, 0, Time.deltaTime *.5f);
                newLayerWeight = Mathf.MoveTowards(newLayerWeight, 1, Time.deltaTime *.5f);
                _animator.SetLayerWeight(layer, newLayerWeight);
                yield return null;
            }
            _animator.SetLayerWeight(1, 1);
            _currentActiveLayer = layer;
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

using System.Collections;
using UnityEngine;

namespace Units.Appearance
{
    public class UnitAnimatorController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private int _speedHash, _strafeHash;
        private float _savedTime;
        private int _framesCounter, _framesUpdateDelay;
        private int _lod;
        
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
            _animator.enabled = false;
        }

        private void Update()
        {
            if (_animator.enabled)
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

        public void UpdateDistanceToCamera(float distance) // hardcoded with magic numbers for now
        {
            if (distance < 30 && _lod != 0)
            {
                _animator.SetLayerWeight(0, 1);
                _animator.SetLayerWeight(1, 0);
                _animator.SetLayerWeight(2, 0);
                _lod = 0;
            }
            else if (distance < 60 && _lod != 1)
            {
                _animator.SetLayerWeight(0, 0);
                _animator.SetLayerWeight(1, 1);
                _animator.SetLayerWeight(2, 0);
                _lod = 1;
            }
            else if (_lod != 2)
            {
                _animator.SetLayerWeight(0, 0);
                _animator.SetLayerWeight(1, 0);
                _animator.SetLayerWeight(2, 1);
                _lod = 2;
            }

            int newUpdateDelay = (int)(distance * 0.10f);
            _framesCounter += newUpdateDelay - _framesUpdateDelay;
            _framesUpdateDelay = newUpdateDelay;
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

using System;
using System.Collections;
using UnityEngine;

namespace Units.Appearance
{
    public class UnitAnimatorController : MonoBehaviour
    {
        [SerializeField] private float _animatorUpdateFrequencyBias = 10;
        [SerializeField] private float _animatorUpdateFrequencyMaximumDistance = 100;
        [SerializeField] private int _animatorUpdateFrequencyMinimum = 15;
        [SerializeField] private Animator _animator;
        private int _speedHash, _strafeHash;
        private float _savedTime;
        private int _updateDelay;
        private float _distanceToCamera;
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
            yield return null; //it needs two frames to initialize
            ApplyVelocity(Vector3.zero);
            yield return null;
            _animator.enabled = false;
        }

        private void Update()
        {
            _updateDelay--;
            if (_updateDelay >= 0)
                return;
            
            _animator.Update(Time.time - _savedTime);
            _savedTime = Time.time;
            
            _updateDelay =  (int)Mathf.Lerp(0, _animatorUpdateFrequencyMinimum, 
                ((_distanceToCamera - _animatorUpdateFrequencyBias) / _animatorUpdateFrequencyMaximumDistance));
        }

        public void ApplyVelocity(Vector3 velocity)
        {
            _animator.SetFloat(_speedHash, velocity.z);
            _animator.SetFloat(_strafeHash, velocity.x);
        }

        public void UpdateDistanceToCamera(float distance) // hardcoded with magic numbers for now
        {
            _distanceToCamera = distance;
            if (_distanceToCamera < 30 && _lod != 0)
            {
                _animator.SetLayerWeight(0, 1);
                _animator.SetLayerWeight(1, 0);
                _animator.SetLayerWeight(3, 0);
                _lod = 0;
            }
            else if (_distanceToCamera < 60 && _lod != 1)
            {
                _animator.SetLayerWeight(0, 0);
                _animator.SetLayerWeight(1, 1);
                _animator.SetLayerWeight(3, 0);
                _lod = 1;
            }
            else if (_lod != 2)
            {
                _animator.SetLayerWeight(0, 0);
                _animator.SetLayerWeight(1, 0);
                _animator.SetLayerWeight(3, 1);
                _lod = 2;
            }
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

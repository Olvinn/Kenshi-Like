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
            if (_animator.enabled)
                return;
            
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

        public void UpdateDistanceToCamera(float distance)
        {
            _distanceToCamera = distance;
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

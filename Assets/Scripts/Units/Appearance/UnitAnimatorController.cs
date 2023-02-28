using System.Collections;
using UnityEngine;

namespace Units.Appearance
{
    public class UnitAnimatorController : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _updateFrequencyToDistance;
        [SerializeField] private Animator _animator;
        private int _speedHash, _strafeHash;
        private float _savedTime;
        private float _updateDelay;
        private Camera _camera;
        
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
            _camera = Camera.main;
            _animator.enabled = true;
            yield return null; //it needs two frames to initialize
            yield return null;
            _animator.enabled = false;
        }

        private void Update()
        {
            if (_animator.enabled)
                return;
            _updateDelay -= Time.deltaTime;
            if (_updateDelay > 0)
                return;
            _animator.Update(Time.time - _savedTime);
            _updateDelay = _updateFrequencyToDistance.Evaluate(Vector3.Distance(_camera.transform.position, transform.position));
            _savedTime = Time.time;
        }

        public void ApplyVelocity(Vector3 velocity)
        {
            _animator.SetFloat(_speedHash, velocity.z);
            _animator.SetFloat(_strafeHash, velocity.x);
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

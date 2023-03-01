using System.Collections;
using UnityEngine;

namespace Units.Appearance
{
    public class UnitAnimatorController : MonoBehaviour
    {
        [SerializeField] private float _animatorUpdateFrequencyBias = 10;
        [SerializeField] private float _animatorUpdateFrequencyMaximumDistance = 100;
        [SerializeField] private int _animatorUpdateFrequencyMinimum = 15;
        [SerializeField] private Animator[] _animators;
        private Animator _current;
        private int _speedHash, _strafeHash;
        private float _savedTime;
        private int _updateDelay;
        private float _distanceToCamera;
        
        private void Awake()
        {
            if (_animators == null)
                _animators = GetComponentsInChildren<Animator>();

            foreach (var animator in _animators)
                animator.keepAnimatorControllerStateOnDisable = true;
            _speedHash = Animator.StringToHash("Speed");
            _strafeHash = Animator.StringToHash("Strafe");
        }

        private IEnumerator Start()
        {
            LODGroup lod = GetComponent<LODGroup>();
            lod.enabled = false;
            foreach (var animator in _animators)
                animator.enabled = true;
            yield return null; //it needs two frames to initialize
            foreach (var animator in _animators) //it also need to do smth to start working as normal
            {
                animator.SetFloat(_speedHash, 0);
                animator.SetFloat(_strafeHash, 0);
            }
            yield return null;
            foreach (var animator in _animators)
                animator.enabled = false;
            lod.enabled = true;
            FindActiveLOD();
        }

        private void Update()
        {
            if (_current == null || !_current.gameObject.activeSelf)
                FindActiveLOD();
            
            _updateDelay--;
            if (_updateDelay >= 0)
                return;
            
            _current.Update(Time.time - _savedTime);
            _savedTime = Time.time;
            
            _updateDelay =  (int)Mathf.Lerp(0, _animatorUpdateFrequencyMinimum, 
                ((_distanceToCamera - _animatorUpdateFrequencyBias) / _animatorUpdateFrequencyMaximumDistance));
        }

        public void ApplyVelocity(Vector3 velocity)
        {
            if (_current == null)
                return;
            
            _current.SetFloat(_speedHash, velocity.z);
            _current.SetFloat(_strafeHash, velocity.x);
        }

        public void UpdateDistanceToCamera(float distance)
        {
            _distanceToCamera = distance;
        }

        private void FindActiveLOD()
        {
            foreach (var animator in _animators)
                if (animator.gameObject.activeSelf)
                    _current = animator;
            ApplyVelocity(Vector3.zero);
            _current.Update(Time.time);
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_animators == null)
                _animators = GetComponentsInChildren<Animator>();
        }
#endif
    }
}

using UnityEngine;

namespace Units.Views.IK
{
    public class IKController : MonoBehaviour
    {
        public Transform target;
        public float transitionSpeed = 2f;
        
        [SerializeField] private Animator animator;
        [SerializeField] private UnitAppearance appearance;

        private bool _isVisible;
        private Vector3 _lookPos;
        private float _lookWeight;
        private Transform _curTarget;
        
        private void Start()
        {
            appearance.OnInvisible += OnBecameInvisible;
            appearance.OnVisible += OnBecameVisible;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (!_isVisible)
                return;
            
            if (_curTarget != null && _curTarget == target)
            {
                _lookPos = _curTarget.position;
                _lookWeight += Time.deltaTime * transitionSpeed;
            }
            else
            {
                _lookWeight -= Time.deltaTime * transitionSpeed;
                if (_lookWeight <= 0)
                    _curTarget = target;
            }
            
            _lookWeight = Mathf.Clamp01(_lookWeight);
            animator.SetLookAtWeight(_lookWeight);
            animator.SetLookAtPosition(_lookPos);
        }

        private void OnBecameInvisible()
        {
            _isVisible = false;
        }
        
        private void OnBecameVisible()
        {
            _isVisible = true;
        }
    }
}

using UnityEngine;

namespace Units.Views.IK
{
    public class IKController : MonoBehaviour
    {
        public Transform Target;
        
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
            if (Target != null && _curTarget == Target)
            {
                _lookPos = Target.position;
                _lookWeight += Time.deltaTime;
            }
            else
            {
                _lookWeight -= Time.deltaTime;
                if (_lookWeight != 0)
                    _curTarget = Target;
            }
            
            if (!_isVisible)
                return;
            
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

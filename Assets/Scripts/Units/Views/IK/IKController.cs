using UnityEngine;

namespace Units.Views.IK
{
    public class IKController : MonoBehaviour
    {
        public Transform target;
        public float transitionSpeed = 2f;
        public Transform armsPos;
        
        [SerializeField] private Animator animator;
        [SerializeField] private UnitAppearance appearance;


        private bool _isVisible;
        private Vector3 _lookPos, _armsPos;
        private Quaternion _armsRot;
        private float _lookWeight, _armsWeight;
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
            
            HeadIK();
            ArmsIK();
        }

        private void OnBecameInvisible()
        {
            _isVisible = false;
        }
        
        private void OnBecameVisible()
        {
            _isVisible = true;
        }

        private void HeadIK()
        {
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
            animator.SetLookAtWeight(_lookWeight, .2f, .9f, 1f, .5f);
            animator.SetLookAtPosition(_lookPos);
        }

        private void ArmsIK()
        {
            if (armsPos == null)
            {
                if (_armsWeight == 0)
                    return;
                
                _armsWeight -= Time.deltaTime * 5;
            }
            else
            {
                _armsPos = armsPos.position;
                _armsRot = armsPos.rotation;
                _armsWeight += Time.deltaTime * 2;
            }
            
            _armsWeight = Mathf.Clamp01(_armsWeight);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _armsWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _armsWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, _armsPos);
            animator.SetIKRotation(AvatarIKGoal.RightHand, _armsRot);
            
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _armsWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _armsWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, _armsPos);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(180,180,180) * _armsRot);
        }
    }
}

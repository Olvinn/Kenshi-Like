using Data;
using UnityEngine;

namespace OldUnits.Views.IK
{
    public class IKController : MonoBehaviour
    {
        public Transform target;
        public float transitionSpeed = 2f;
        public Transform leftArmPos;
        public bool legsIKOn = true;
        
        [SerializeField] private Animator animator;
        [SerializeField] private UnitAppearance appearance;
        [SerializeField] private Transform pelvis;

        private bool _isVisible;
        private Vector3 _lookPos, _leftArmPos;
        private Quaternion _leftArmRot;
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
            LegsIK();
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
            if (leftArmPos == null)
            {
                if (_armsWeight == 0)
                    return;
                
                _armsWeight -= Time.deltaTime * 2;
            }
            else
            {
                _leftArmPos = leftArmPos.position;
                _leftArmRot = leftArmPos.rotation;
                _armsWeight += Time.deltaTime;
            }
            
            _armsWeight = Mathf.Clamp01(_armsWeight);

            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _armsWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _armsWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, _leftArmPos);
            animator.SetIKRotation(AvatarIKGoal.RightHand, _leftArmRot);
            
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _armsWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _armsWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftArmPos);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.Euler(180,180,180) * _leftArmRot);
        }

        private void LegsIK()
        {
            pelvis.localPosition = Vector3.down;
            
            if (!legsIKOn)
                return;
            
            float l = LegIK(AvatarIKGoal.LeftFoot);
            float r = LegIK(AvatarIKGoal.RightFoot);
            r = r < l ? l : r;
            pelvis.localPosition = Vector3.down + Vector3.down * r;
        }

        private float LegIK(AvatarIKGoal foot)
        {
            var constants = GameContext.Instance.Constants;
            
            float diff = 0;
            float weight = 0;
            RaycastHit hit;
            Vector3 animPos = Vector3.zero;
            Quaternion animRot = animator.GetIKRotation(foot);
            switch (foot)
            {
                case AvatarIKGoal.LeftFoot:
                    animPos = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
                    break;
                case AvatarIKGoal.RightFoot:
                    animPos = animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
                    break;
            }
            Ray ray = new Ray(animPos + Vector3.up * constants.LegsIKRayCastHeight, Vector3.down);
            if (Physics.Raycast(ray, out hit, constants.LegsIKRayLength, 1))
            {
                animPos -= transform.position;
                weight = 1 - ((animPos.y - .1f) * 10f);
                weight = Mathf.Clamp01(weight);
                animRot = Quaternion.FromToRotation(Vector3.up, hit.normal) * animRot;
                animPos = hit.point + Vector3.up * .1f;
                diff = transform.position.y - animPos.y;
            }
            animator.SetIKPositionWeight(foot, weight);
            animator.SetIKRotationWeight(foot, weight * .5f);
            animator.SetIKPosition(foot, animPos);
            animator.SetIKRotation(foot, animRot);
            return diff;
        }
    }
}

using Data;
using UnityEngine;

namespace Units.Views.IK
{
    public class IKController : MonoBehaviour
    {
        public Transform target;
        public float transitionSpeed = 2f;
        public Transform armsPos;
        public bool legsIKOn = true;
        
        [SerializeField] private Animator animator;
        [SerializeField] private UnitAppearance appearance;
        [SerializeField] private Transform pelvis;

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
            if (armsPos == null)
            {
                if (_armsWeight == 0)
                    return;
                
                _armsWeight -= Time.deltaTime * 3;
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
            Ray ray = new Ray(animPos + Vector3.up * Constants.instance.LegsIKRayCastHeight, Vector3.down);
            if (Physics.Raycast(ray, out hit, Constants.instance.LegsIKRayLength, 1))
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

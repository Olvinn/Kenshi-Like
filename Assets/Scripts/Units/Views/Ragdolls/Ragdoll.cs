using System.Collections.Generic;
using UnityEngine;

namespace Units.Views.Ragdolls
{
    public class Ragdoll : MonoBehaviour
    {
        [SerializeField] private List<RagdollPart> parts;
        [SerializeField] private Animator animator;

        private bool _isVisible;
        private bool _isRagdolled;

        private void FixedUpdate()
        {
            if (_isVisible && !_isRagdolled)
                foreach (var part in parts)
                {
                    part.UpdateVelocity();
                }
        }

        private void OnBecameVisible()
        {
            _isVisible = true;
        }

        private void OnBecameInvisible()
        {
            _isVisible = false;
        }

        public void StartRagdoll()
        {
            if (_isRagdolled)
                return;

            animator.enabled = false;
            foreach (var part in parts)
            {
                part.DoRagdoll();
            }

            _isRagdolled = true;
        }

        public void StopRagdoll()
        {
            if (!_isRagdolled)
                return;

            animator.enabled = true;
            foreach (var part in parts)
            {
                part.DoAnimations();
            }

            _isRagdolled = false;
        }
    }
}

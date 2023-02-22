using System.Collections.Generic;
using UnityEngine;

namespace OldUnits.Views.Ragdolls
{
    public class Ragdoll : MonoBehaviour
    {
        [SerializeField] private List<RagdollPart> parts;
        [SerializeField] private Animator animator;

        private bool _isRagdolled;

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

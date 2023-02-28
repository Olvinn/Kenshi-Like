using UnityEngine;

namespace OldUnits.Views.Ragdolls
{
    public class RagdollPart : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;

        private float _prevUpdate;
        private bool _ragdolled;
        
        private void Awake()
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
            rb.drag = .8f;
        }

        public void DoRagdoll()
        {
            rb.detectCollisions = true;
            rb.isKinematic = false;
        }

        public void DoAnimations()
        {
            rb.detectCollisions = false;
            rb.isKinematic = true;
        }
    }
}

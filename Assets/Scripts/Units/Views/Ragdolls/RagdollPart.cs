using UnityEngine;

namespace Units.Views.Ragdolls
{
    public class RagdollPart : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;

        private float _prevUpdate;
        private Vector3 _velocity, _savedPos;
        
        private void Awake()
        {
            rb.isKinematic = true;
        }

        public void UpdateVelocity()
        {
            float delta = Time.time - _prevUpdate;
            if (delta < 1f)
                _velocity = (transform.position - _savedPos) / delta;
            else
                _velocity = Vector3.zero;
            _savedPos = transform.position;
            _prevUpdate = Time.time;
        }

        public void DoRagdoll()
        {
            rb.isKinematic = false;
            rb.velocity = _velocity;
        }

        public void DoAnimations()
        {
            rb.isKinematic = true;
        }
    }
}

using UnityEngine;

namespace Units.Views.Ragdolls
{
    public class RagdollPart : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;

        private float _prevUpdate;
        private Vector3 _velocity, _savedPos;
        private bool _ragdolled;
        
        private void Awake()
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        public void UpdateVelocity()
        {
            if (_velocity.magnitude < .1f && _ragdolled)
            {
                rb.detectCollisions = false;
                rb.isKinematic = true;
                return;
            }

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
            rb.detectCollisions = true;
            _ragdolled = true;
            rb.isKinematic = false;
            rb.velocity = _velocity;
        }

        public void DoAnimations()
        {
            rb.detectCollisions = false;
            _ragdolled = false;
            rb.isKinematic = true;
        }
    }
}

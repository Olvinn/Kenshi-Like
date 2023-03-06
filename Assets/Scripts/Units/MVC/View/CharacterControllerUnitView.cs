using Units.Structures;
using UnityEngine;

namespace Units.MVC.View
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class CharacterControllerUnitView : UnitView
    {
        private CharacterController _characterController;
        private Vector3 _savedPosition, _moveDirection, _localVelocity;
        private Camera _mainCamera;
        private bool _rotateOnMoveDirection = true;
        private float _speed, _rotationSpeed = 120;
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _characterController.center = Vector3.up;
        }

        public void SetStats(UnitStats stats)
        {
            _speed = stats.speed;
        }

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        public void Move(Vector3 direction)
        {
            direction.y = 0;
            if (direction.magnitude > 1f)
                direction.Normalize();
            _moveDirection = direction;
        }

        public void WarpTo(Vector3 position)
        {
            transform.position = position;
            onPositionChanged?.Invoke(transform.position);
        }

        protected override void ProceedMovement()
        {
            movingState = _moveDirection != Vector3.zero ? MovingStatus.Moving : MovingStatus.Staying;
            
            Vector3 camRot = _mainCamera.transform.forward;
            camRot.y = 0;
            if (Vector3.Dot(camRot.normalized, transform.forward) <= -0.99999f) //prevent jitter when character moving towards camera
                _localVelocity = new Vector3(0,0, _speed) * _moveDirection.magnitude;
            else
            {
                Quaternion local = Quaternion.FromToRotation(transform.forward, camRot);
                _localVelocity = local * (_moveDirection * _speed);
            }

            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, camRot);
            _characterController.Move(rot * _moveDirection * (_speed * Time.deltaTime) + Vector3.down);

            if (_savedPosition != transform.position)
            {
                onPositionChanged?.Invoke(transform.position);
                _savedPosition = transform.position;
            }
        }

        protected override void Rotate()
        {
            if (movingState == MovingStatus.Staying)
                return;

            float rot;
            if (!_rotateOnMoveDirection)
            {
                Vector3 camRot = _mainCamera.transform.forward;
                camRot.y = 0;
                rot = Vector3.SignedAngle(Vector3.forward, camRot, Vector3.up);
            }
            else
            {
                Vector3 v = _characterController.velocity;
                rot = Vector3.SignedAngle(Vector3.forward, v, Vector3.up);
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                Quaternion.AngleAxis(rot, Vector3.up),
                Time.deltaTime * _rotationSpeed);
        }

        protected override void UpdateAnimator()
        {
            _animator.ApplyVelocity(_localVelocity);
            _animator.UpdateDistanceToCamera(Vector3.Distance(_mainCamera.transform.position, transform.position));
        }
    }
}

using Units.Structures;
using UnityEngine;

namespace Units.MVC.View
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class CharacterControllerUnitView : UnitView
    {
        private CharacterController _characterController;
        private Vector3 _savedPosition, _moveDirection;
        private float _speed, _rotationSpeed = 180, _currentSpeed;
        private bool _lookingForwardVelocity;
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _characterController.center = Vector3.up;
        }

        public override void SetStats(UnitStats stats)
        {
            _speed = stats.speed;
        }

        public override void MoveToPosition(Vector3 position, bool shift)
        {
            throw new System.NotImplementedException();
        }

        public override void MoveToDirection(Vector3 direction, bool shift)
        {
            if (direction == Vector3.zero)
            {
                if (!TryChangeState(UnitViewState.Staying))
                    return;
            }
            else if (shift)
            {
                if (!TryChangeState(UnitViewState.Shifting))
                    return;
            }
            else
            {
                if (!TryChangeState(UnitViewState.Moving))
                    return;
            }
            
            direction.y = 0;
            if (direction.magnitude > 1f)
                direction.Normalize();
            _moveDirection = direction;
        }

        public override void WarpTo(Vector3 position)
        {
            transform.position = position;
            onPositionChanged?.Invoke(transform.position);
        }

        protected override void ProceedMovement()
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed,_speed + (state == UnitViewState.Shifting && _lookingForwardVelocity ? 3 : 0), Time.deltaTime * 5);
            _localVelocity = Vector3.zero;  
            
            if (state != UnitViewState.Moving && state != UnitViewState.Shifting)
                return;
            
            Vector3 camRot = _mainCamera.transform.forward;
            camRot.y = 0;
            if (Vector3.Dot(camRot.normalized, transform.forward) <= -0.99999f) //prevent jitter when character moving towards camera
                _localVelocity = new Vector3(0,0, _currentSpeed) * _moveDirection.magnitude;
            else
            {
                Quaternion local = Quaternion.FromToRotation(transform.forward, camRot);
                _localVelocity = local * (_moveDirection * _currentSpeed);
            }

            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, camRot);
            _characterController.Move(rot * _moveDirection * (_currentSpeed * Time.deltaTime) + Vector3.down);

            if (_savedPosition != transform.position)
            {
                onPositionChanged?.Invoke(transform.position);
                _savedPosition = transform.position;
            }
        }

        protected override void Rotate()
        {
            if (state is UnitViewState.GettingDamage or UnitViewState.Dead)
                return;
            
            float rot = 0;
            if (state is UnitViewState.Staying or UnitViewState.Moving or UnitViewState.Attacking)
            {
                Vector3 camRot = _mainCamera.transform.forward;
                camRot.y = 0;
                rot = Vector3.SignedAngle(Vector3.forward, camRot, Vector3.up);
            }
            else if (state is UnitViewState.Shifting)
            {
                Vector3 v = _characterController.velocity;
                rot = Vector3.SignedAngle(Vector3.forward, v, Vector3.up);
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                Quaternion.AngleAxis(rot, Vector3.up),
                Time.deltaTime * _rotationSpeed);
            _lookingForwardVelocity =
                Vector3.Angle(transform.forward, transform.localToWorldMatrix * _localVelocity) < 15;
        }

        protected override void OnAppearancePrefabLoaded(GameObject appearanceGO)
        {
            base.OnAppearancePrefabLoaded(appearanceGO);
            if (_animator != null)
            {
                _animator.SetAnimatorActive(true);
            }
        }
    }
}

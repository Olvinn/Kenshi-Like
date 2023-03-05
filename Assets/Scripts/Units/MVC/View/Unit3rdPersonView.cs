using System;
using AssetsManagement;
using Units.Appearance;
using Units.Structures;
using UnityEngine;

namespace Units.MVC.View
{
    [RequireComponent(typeof(CharacterController))]
    public class Unit3rdPersonView : MonoBehaviour
    {
        public Action<Vector3> onPositionChanged;
        public MovingStatus movingState { get; private set; }

        private UnitAppearanceController _appearance;
        private UnitAnimatorController _animator;
        private CharacterController _characterController;
        private Vector3 _savedPosition, _moveDirection, _velocity;
        private UnitAppearance _appearanceData;
        private Camera _mainCamera;
        private bool _isVisualsInitialized;
        private float _speed;
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            _mainCamera = Camera.main;
        }
        
        private void Update()
        {
            ProceedMovement();
            
            if (!_isVisualsInitialized)
                return;
            
            UpdateAnimator();
        }

        public void SetStats(UnitStats stats)
        {
            _speed = stats.speed;
        }

        public void SetAppearance(UnitAppearance appearance)
        {
            _appearanceData = appearance;
            AssetsManager.LoadAsset(appearance.prefab, transform, OnAppearancePrefabLoaded);
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

        private void ProceedMovement()
        {
            movingState = _moveDirection != Vector3.zero ? MovingStatus.Moving : MovingStatus.Staying;

            Vector3 camRot = _mainCamera.transform.forward;
            camRot.y = 0;
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, camRot);
            _velocity = rot * _moveDirection * _speed;
            _characterController.Move(_velocity * Time.deltaTime);

            if (_savedPosition != transform.position)
            {
                onPositionChanged?.Invoke(transform.position);
                _savedPosition = transform.position;
            }
        }

        private void UpdateAnimator()
        {
            _animator.ApplyVelocity(_velocity);
            _animator.UpdateDistanceToCamera(Vector3.Distance(_mainCamera.transform.position, transform.position));
        }

        private void OnAppearancePrefabLoaded(GameObject appearanceGO)
        {
            if (!appearanceGO)
                return;
            
            _appearance = appearanceGO.GetComponent<UnitAppearanceController>();;
            _appearance.SetAppearance(_appearanceData);
            _appearance.transform.position = Vector3.down;

            _animator = appearanceGO.GetComponent<UnitAnimatorController>();
            if (_animator != null)
            {
                _isVisualsInitialized = true;
            }
        }
    }
}

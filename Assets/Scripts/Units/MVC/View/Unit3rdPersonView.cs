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
        private Vector3 _savedPosition, _moveDirection, _localVelocity;
        private UnitAppearance _appearanceData;
        private Camera _mainCamera;
        private bool _isVisualsInitialized;
        private float _speed;
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _characterController.center = Vector3.up;
        }

        private void Start()
        {
            _mainCamera = Camera.main;
        }
        
        private void Update()
        {
            ProceedMovement();
            ProceedRotation();
            
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
            
            Quaternion local = Quaternion.FromToRotation(transform.forward, camRot);
            _localVelocity = local * _moveDirection * _speed;
            
            _characterController.Move(rot * _moveDirection * (_speed * Time.deltaTime) + Vector3.down);

            if (_savedPosition != transform.position)
            {
                onPositionChanged?.Invoke(transform.position);
                _savedPosition = transform.position;
            }
        }

        private void ProceedRotation()
        {
            if (movingState == MovingStatus.Staying)
                return;
            
            Vector3 camRot = _mainCamera.transform.forward;
            camRot.y = 0;
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, camRot);
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, Time.deltaTime * 60);
        }

        private void UpdateAnimator()
        {
            _animator.ApplyVelocity(_localVelocity);
            _animator.UpdateDistanceToCamera(Vector3.Distance(_mainCamera.transform.position, transform.position));
        }

        private void OnAppearancePrefabLoaded(GameObject appearanceGO)
        {
            if (!appearanceGO)
                return;
            
            _appearance = appearanceGO.GetComponent<UnitAppearanceController>();;
            _appearance.SetAppearance(_appearanceData);

            _animator = appearanceGO.GetComponent<UnitAnimatorController>();
            if (_animator != null)
            {
                _animator.SetAnimatorActive(true);
                _isVisualsInitialized = true;
            }
        }
    }
}

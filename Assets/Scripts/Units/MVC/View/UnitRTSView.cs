using System;
using AssetsManagement;
using Units.Appearance;
using Units.Structures;
using UnityEngine;
using UnityEngine.AI;

namespace Units.MVC.View
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitRTSView : MonoBehaviour
    {
        public Action<Vector3> onPositionChanged;
        public Action onReachDestination;
        public MovingStatus movingState { get; private set; }

        private UnitAppearanceController _appearance;
        private UnitAnimatorController _animator;
        private NavMeshAgent _agent;
        private Vector3 _savedPosition;
        private UnitAppearance _appearanceData;
        private Camera _mainCamera;
        private bool _isVisualsInitialized;
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _agent.autoBraking = false;
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            _mainCamera = Camera.main;
        }

        // it maybe runs to frequently. maybe it should be called by the controller 
        private void Update()
        {
            ProceedMovement();
            
            if (!_isVisualsInitialized)
                return;
            
            UpdateAnimator();
        }

        public void SetStats(UnitStats stats)
        {
            _agent.speed = stats.speed;
        }

        public void SetAppearance(UnitAppearance appearance)
        {
            _appearanceData = appearance;
            AssetsManager.LoadAsset(appearance.prefab, transform, OnAppearancePrefabLoaded);
        }

        public void MoveTo(Vector3 destination)
        {
            MoveTo(destination, .5f);
        }

        public void MoveTo(Vector3 destination, float stoppingDistance)
        {
            if (Vector3.Distance(transform.position, destination) <= stoppingDistance)
            {
                _agent.isStopped = true;
                movingState = MovingStatus.Staying;
                return;
            }

            _agent.stoppingDistance = stoppingDistance;
            _agent.SetDestination(destination);
            movingState = MovingStatus.Moving;
            _agent.isStopped = false;
        }

        public void WarpTo(Vector3 position)
        {
            _agent.Warp(position);
            onPositionChanged?.Invoke(transform.position);
        }

        private void ProceedMovement()
        {
            if (movingState == MovingStatus.Moving)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    _agent.isStopped = true;
                    movingState = MovingStatus.Staying;
                    onReachDestination?.Invoke();
                }
            }

            if (_savedPosition != transform.position)
            {
                onPositionChanged?.Invoke(transform.position);
                _savedPosition = transform.position;
            }
        }

        private void UpdateAnimator()
        {
            _animator.ApplyVelocity(transform.worldToLocalMatrix * _agent.velocity);
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
                _isVisualsInitialized = true;
            }
        }
    }
}

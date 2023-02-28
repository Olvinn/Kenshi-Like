using System;
using AssetsManagement;
using Units.Appearance;
using Units.Structures;
using UnityEngine;
using UnityEngine.AI;

namespace Units.MVC.View
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitView : MonoBehaviour
    {
        public Action<Vector3> onPositionChanged;
        public Action onReachDestination;
        public MovingStatus movingStatus { get; private set; }

        private UnitAppearanceController _appearance;
        private UnitAnimatorController _animator;
        private NavMeshAgent _agent;
        private Vector3 _savedPosition;
        private UnitAppearance _appearanceData;
        private bool _isLoaded;
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (movingStatus == MovingStatus.Moving)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    _agent.isStopped = true;
                    movingStatus = MovingStatus.Staying;
                    onReachDestination?.Invoke();
                }
            }

            if (_savedPosition != transform.position)
            {
                onPositionChanged?.Invoke(transform.position);
                _savedPosition = transform.position;
            }
            
            if (!_isLoaded)
                return;
            
            _animator.ApplyVelocity(transform.worldToLocalMatrix * _agent.velocity);
            _animator.UpdateDistanceToCamera(Vector3.Distance(Camera.main.transform.position, transform.position));
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
                movingStatus = MovingStatus.Staying;
                return;
            }

            _agent.stoppingDistance = stoppingDistance;
            _agent.destination = destination;
            movingStatus = MovingStatus.Moving;
            _agent.isStopped = false;
        }

        public void WarpTo(Vector3 position)
        {
            _agent.Warp(position);
            onPositionChanged?.Invoke(transform.position);
        }

        private void OnAppearancePrefabLoaded(GameObject appearanceGO)
        {
            _isLoaded = true;
            
            _appearance = appearanceGO.GetComponent<UnitAppearanceController>();;
            _appearance.SetAppearance(_appearanceData);

            _animator = appearanceGO.GetComponent<UnitAnimatorController>();
        }
    }
}

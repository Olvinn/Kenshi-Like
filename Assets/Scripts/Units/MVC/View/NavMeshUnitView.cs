using System;
using Units.Structures;
using UnityEngine;
using UnityEngine.AI;

namespace Units.MVC.View
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshUnitView : UnitView
    {
        public Action onReachDestination;
        private NavMeshAgent _agent;
        private Vector3 _savedPosition;
        private Camera _mainCamera;
        
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

        public override void SetStats(UnitStats stats)
        {
            _agent.speed = stats.speed;
        }

        // --- Non-Liskov methods ---
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
        // --------------------------

        public override void WarpTo(Vector3 position)
        {
            _agent.Warp(position);
            onPositionChanged?.Invoke(transform.position);
        }

        protected override void ProceedMovement()
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

        protected override void UpdateAnimator()
        {
            _animator.ApplyVelocity(transform.worldToLocalMatrix * _agent.velocity);
            _animator.UpdateDistanceToCamera(Vector3.Distance(_mainCamera.transform.position, transform.position));
        }
    }
}

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
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        protected override void Start()
        {
            base.Start();
            _agent.autoBraking = false;
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
        }

        public override void SetStats(UnitStats stats)
        {
            _agent.speed = stats.speed;
        }

        public override void MoveToPosition(Vector3 destination, bool shift)
        {
            if (!_agent.enabled)
                return;
            
            float stoppingDistance = .5f;
            if (Vector3.Distance(transform.position, destination) <= stoppingDistance)
            {
                if (!TryChangeState(UnitViewState.Staying))
                    return;
                
                _agent.isStopped = true;
            }
            else
            { 
                if (shift)
                {
                    if (!TryChangeState(UnitViewState.Shifting))
                        return;
                }
                else
                {
                    if (!TryChangeState(UnitViewState.Moving))
                        return;
                }
                
                _agent.isStopped = false;
                _agent.stoppingDistance = stoppingDistance;
                _agent.SetDestination(destination);
            }
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
            
            _agent.velocity = direction * _agent.speed;
        }

        public override void WarpTo(Vector3 position)
        {
            _agent.Warp(position);
            onPositionChanged?.Invoke(transform.position);
        }

        public override void Die()
        {
            base.Die();
            _agent.enabled = false;
        }

        protected override void ProceedMovement()
        {
            if (!_agent.enabled)
                return;
            
            if (state is not UnitViewState.Moving or UnitViewState.Shifting)
                _agent.isStopped = true;
            else
                _agent.isStopped = false;
            
            if (state == UnitViewState.Moving)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    _agent.isStopped = true;
                    TryChangeState(UnitViewState.Staying);
                    onReachDestination?.Invoke();
                }
            }

            if (_savedPosition != transform.position)
            {
                onPositionChanged?.Invoke(transform.position);
                _savedPosition = transform.position;
            }

            _localVelocity = transform.worldToLocalMatrix * _agent.velocity;
        }

        protected override void Rotate()
        {
            
        }
    }
}

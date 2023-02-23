using System;
using Units.Structures;
using UnityEngine;
using UnityEngine.AI;

namespace Units.MVC.View
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitView : MonoBehaviour
    {
        public Action onReachDestination;
        public MovingStatus movingStatus { get; private set; }
        
        private NavMeshAgent _agent;
        
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
        }

        public void SetStats(UnitStats stats)
        {
            _agent.speed = stats.speed;
        }

        public void MoveTo(Vector3 destination, float stoppingDistance = .5f)
        {
            if (Vector3.Distance(transform.position, destination) <= stoppingDistance)
                return;
            
            _agent.stoppingDistance = stoppingDistance;
            _agent.destination = destination;
            movingStatus = MovingStatus.Moving;
            _agent.isStopped = false;
        }
    }
}

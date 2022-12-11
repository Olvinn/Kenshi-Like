using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public class UnitView : MonoBehaviour
    {
        public event Action OnReachDestination;
        [field:SerializeField] public MovementStatus MovementStatus { get; private set; }
        
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;

        [SerializeField] private Color color;
        
        private MeshRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _renderer.material.color = color;
        }

        public void Die()
        {
            agent.enabled = false;
            animator.SetTrigger("Die");
        }

        public void MoveTo(Vector3 destination)
        {
            agent.SetDestination(destination);
            MovementStatus = MovementStatus.Moving;
        }

        public void Select()
        {
            _renderer.material.color = Color.green;
        }

        public void Deselect()
        {
            _renderer.material.color = color;
        }

        public void SetProvoked()
        {
            animator.SetBool("Provoked", true);
        }

        public void SetIdle()
        {
            animator.SetBool("Provoked", false);
        }

        public bool CanAttack(UnitView target)
        {
            return Vector3.Distance(target.transform.position, transform.position) < 2f;
        }

        public void DoAttackAnimation(Action callback)
        {
            animator.Play("Attack");
            StartCoroutine(Wait(callback));
        }

        private void Update()
        { 
            if (!agent.enabled)
                return;
            
            if (!agent.pathPending && agent.remainingDistance < 1f)
            {
                if (MovementStatus == MovementStatus.Moving)
                {
                    MovementStatus = MovementStatus.Aimless;
                    Debug.Log("Destination reached");
                    OnReachDestination?.Invoke();
                }
            }
        }

        IEnumerator Wait(Action callback)
        {
            yield return new WaitForSeconds(1);
            callback?.Invoke();
        }
    }

    public enum MovementStatus
    {
        Aimless,
        Moving,
    }
}

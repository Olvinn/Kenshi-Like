using System;
using System.Collections;
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
        [SerializeField] private GameObject selection;

        [SerializeField] private Color color;
        
        private MeshRenderer _renderer;
        private UnitView _target;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _renderer.material.color = color;
            selection.SetActive(false);
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

            if (_target != null)
            {
                transform.LookAt(_target.transform.position);
            }
        }

        public void Die()
        {
            StopAllCoroutines();
            _renderer.material.color = Color.black;
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
            selection.SetActive(true);
        }

        public void Deselect()
        {
            selection.SetActive(false);
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

        public void SetMaxSpeed(float speed)
        {
            agent.speed = speed;
        }

        public void GetDamage()
        {
            StartCoroutine(Damage());
        }

        public void RotateOn(UnitView target)
        {
            _target = target;
        }

        IEnumerator Wait(Action callback)
        {
            yield return new WaitForSeconds(1);
            callback?.Invoke();
        }

        IEnumerator Damage()
        {
            _renderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            _renderer.material.color = color;
            yield return new WaitForSeconds(0.2f);
            _renderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            _renderer.material.color = color;
        }
    }

    public enum MovementStatus
    {
        Aimless,
        Moving,
    }
}

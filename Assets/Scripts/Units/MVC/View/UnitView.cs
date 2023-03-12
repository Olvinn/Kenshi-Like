using System;
using System.Collections;
using AssetsManagement;
using Units.Appearance;
using Units.Structures;
using UnityEngine;

namespace Units.MVC.View
{
    public abstract class UnitView : MonoBehaviour
    {
        public Action<Vector3> onPositionChanged;
        public Action<int> onGetDamage;
        public UnitViewState state { get; protected set; }
        
        protected UnitAppearanceController _appearance;
        protected UnitAnimatorController _animator;
        protected UnitAppearance _appearanceData;
        private bool _isVisualsInitialized;

        [SerializeField] private Vector3 _attackHitPoint = new Vector3(0, 1.25f, 1.25f);
        [SerializeField] private float _attackHitRadius = .5f;

        private Coroutine _attacking, _gettingDamage;

        protected virtual void Update()
        {
            ProceedMovement();
            Rotate();
            
            if (!_isVisualsInitialized)
                return;
            
            UpdateAnimator();
        }
        
        public virtual void SetAppearance(UnitAppearance appearance)
        {
            _appearanceData = appearance;
            AssetsManager.LoadAsset(appearance.prefab, transform, OnAppearancePrefabLoaded);
        }

        protected virtual void OnAppearancePrefabLoaded(GameObject appearanceGO)
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

        public void Attack(float animationLength, float hitOffset, int damage)
        {
            if (IsBusy())
                return;
            
            state = UnitViewState.Attacking;
            _animator.PlayAttack();
            _attacking = StartCoroutine(WaitingForAttackEnds(animationLength, hitOffset, damage));
        }

        public void GetDamage(float animationLength)
        {
            if (_attacking != null)
                StopCoroutine(_attacking);
            if (_gettingDamage != null)
                StopCoroutine(_gettingDamage);
            state = UnitViewState.GettingDamage;
            _animator.PlayHitReaction();
            _gettingDamage = StartCoroutine(WaitingForGettingDamageEnds(animationLength));
        }

        protected void OnGetDamage(int damage)
        {
            onGetDamage?.Invoke(damage);
        }

        public virtual void Die()
        {
            _animator.PlayDead();
            state = UnitViewState.Dead;
            var col = GetComponent<Collider>();
            if (col)
                col.enabled = false;
        }

        public abstract void SetStats(UnitStats stats);
        public abstract void WarpTo(Vector3 position);
        public abstract void MoveToPosition(Vector3 position);
        public abstract void MoveToDirection(Vector3 direction);
        public abstract void SetFightReady(bool value);

        protected abstract void ProceedMovement();
        protected abstract void Rotate();
        protected abstract void UpdateAnimator();

        protected bool IsBusy()
        {
            return state is UnitViewState.Attacking or UnitViewState.Dead or UnitViewState.GettingDamage;
        }

        IEnumerator WaitingForAttackEnds(float length, float hitOffset, int damage)
        {
            Debug.Log($"Attacking: {length} + {hitOffset}");
            yield return new WaitForSeconds(hitOffset);
            var cols = Physics.OverlapSphere(transform.TransformPoint(_attackHitPoint), _attackHitRadius);
            foreach (var col in cols)
            {
                var temp = col.GetComponent<UnitView>();
                if (temp)
                    temp.OnGetDamage(damage);
            }
            yield return new WaitForSeconds(length - hitOffset);
            state = UnitViewState.Idle;
        }

        IEnumerator WaitingForGettingDamageEnds(float length)
        {
            yield return new WaitForSeconds(length);
            state = UnitViewState.Idle;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + _attackHitPoint, _attackHitRadius);
        }
#endif
    }
}

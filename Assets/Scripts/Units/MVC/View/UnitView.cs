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
        public Action onAttackHit;
        public UnitViewState state { get; protected set; }
        
        protected UnitAppearanceController _appearance;
        protected UnitAnimatorController _animator;
        protected UnitAppearance _appearanceData;
        private bool _isVisualsInitialized;

        [SerializeField] private Vector3 _attackHitPoint = new Vector3(0, 1.25f, 1.25f);
        [SerializeField] private float _attackHitRadius = .5f;

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

        public void Attack(float animationLength, float hitOffset)
        {
            if (state == UnitViewState.Attacking)
                return;
            
            state = UnitViewState.Attacking;
            _animator.PlayAttack();
            StartCoroutine(WaitingForAttackEnds(animationLength, hitOffset));
        }

        public abstract void SetStats(UnitStats stats);
        public abstract void WarpTo(Vector3 position);
        public abstract void MoveToPosition(Vector3 position);
        public abstract void MoveToDirection(Vector3 direction);
        public abstract void SetFightReady(bool value);

        protected abstract void ProceedMovement();
        protected abstract void Rotate();
        protected abstract void UpdateAnimator();

        protected void GetDamage()
        {
            onAttackHit?.Invoke();
            _animator.PlayDead();
            state = UnitViewState.Dead;
        }

        protected bool IsBusy()
        {
            return state == UnitViewState.Attacking || state == UnitViewState.Dead;
        }

        IEnumerator WaitingForAttackEnds(float timer, float hitOffset)
        {
            yield return new WaitForSeconds(hitOffset);
            var cols = Physics.OverlapSphere(transform.TransformPoint(_attackHitPoint), _attackHitRadius);
            foreach (var col in cols)
            {
                var temp = col.GetComponent<UnitView>();
                if (temp)
                    temp.GetDamage();
            }
            yield return new WaitForSeconds(timer - hitOffset);
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

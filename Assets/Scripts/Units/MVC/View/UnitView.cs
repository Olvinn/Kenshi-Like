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

        protected UnitAppearanceController _visuals;
        protected UnitAnimatorController _animator;
        protected UnitAppearance _appearanceData;
        private bool _isVisualsInitialized;

        private Coroutine _attacking, _gettingDamage;

        protected virtual void Update()
        {
            ProceedMovement();
            Rotate();
            
            if (!_isVisualsInitialized)
                return;
            
            UpdateAnimator();
        }

        private void LateUpdate()
        {
            if (state == UnitViewState.Shifting)
                state = UnitViewState.Moving;
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
            
            _visuals = appearanceGO.GetComponent<UnitAppearanceController>();
            _animator = appearanceGO.GetComponent<UnitAnimatorController>();
            
            if (_animator != null && _visuals != null)
            {
                _visuals.SetAppearance(_appearanceData);
                _animator.SetActiveLayer((int)_appearanceData.animationSet);
                _isVisualsInitialized = true;
            }
        }

        public void Shift()
        {
            if (state == UnitViewState.Moving)
                state = UnitViewState.Shifting;
        }

        public void Attack(float animationLength, float hitOffset, int damage)
        {
            if (IsBusy())
                return;
            
            state = UnitViewState.Attacking;
            _animator.PlayAttack();
            _attacking = StartCoroutine(WaitingForAttackEnds(animationLength, hitOffset, damage));
        }

        public void ReactOnDamage(float animationLength)
        {
            if (_attacking != null)
                StopCoroutine(_attacking);
            if (_gettingDamage != null)
                StopCoroutine(_gettingDamage);
            state = UnitViewState.GettingDamage;
            _animator.PlayHitReaction();
            _gettingDamage = StartCoroutine(WaitingForGettingDamageEnds(animationLength));
        }

        public void GetDamage(int damage)
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

        protected abstract void ProceedMovement();
        protected abstract void Rotate();
        protected abstract void UpdateAnimator();

        protected bool IsBusy()
        {
            return state is UnitViewState.Attacking or UnitViewState.Dead or UnitViewState.GettingDamage;
        }

        IEnumerator WaitingForAttackEnds(float length, float hitOffset, int damage)
        {
            yield return new WaitForSeconds(hitOffset);
            Attack1(damage, (int)_appearanceData.animationSet);
            yield return new WaitForSeconds(length - hitOffset);
            state = UnitViewState.Idle;
        }

        IEnumerator WaitingForGettingDamageEnds(float length)
        {
            yield return new WaitForSeconds(length);
            state = UnitViewState.Idle;
        }
        
        public void Attack1(int damage, int layer)
        {
            Collider[] cols = new Collider[10];
            var attackPoint = AttackHelper.singleton.GetAttack1Data(layer);
#if UNITY_EDITOR
            debug = attackPoint;
            time = Time.time;
#endif
            Physics.OverlapSphereNonAlloc(transform.TransformPoint(attackPoint.position), attackPoint.radius, cols);
            foreach (var col in cols)
            {
                if (!col)
                    continue;
                var temp = col.GetComponent<UnitView>();
                if (temp)
                    temp.GetDamage(damage);
            }
        }
        
#if UNITY_EDITOR
        private AttackPoint debug;
        private float time;
        private void OnDrawGizmos()
        {
            if (Time.time - time < 3)
                Gizmos.DrawWireSphere(transform.TransformPoint(debug.position), debug.radius);
        }
#endif
    }
}

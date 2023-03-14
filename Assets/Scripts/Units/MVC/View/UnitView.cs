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
        public UnitViewState state { get; private set; }

        protected UnitAppearanceController _visuals;
        protected UnitAnimatorController _animator;
        protected UnitAppearance _appearanceData;
        protected Vector3 _localVelocity;
        
        protected Camera _mainCamera;
        private bool _isVisualsInitialized;
        private Coroutine _attacking, _gettingDamage;

        #region UnityMethods

        protected virtual void Start()
        {
            _mainCamera = Camera.main;
        }

        protected virtual void Update()
        {
            ProceedMovement();
            Rotate();
            
            if (!_isVisualsInitialized)
                return;
            
            UpdateAnimator();
        }

        #endregion

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

        public void Attack(float animationLength, float hitOffset, int damage)
        {
            if (state == UnitViewState.Attacking || !TryChangeState(UnitViewState.Attacking))
                return;
            
            _animator.PlayAttack();
            _attacking = StartCoroutine(ProcessingAttack(animationLength, hitOffset, damage));
        }

        public void ReactOnDamage(float animationLength)
        {
            if (!TryChangeState(UnitViewState.GettingDamage))
                return;
            
            if (_attacking != null)
                StopCoroutine(_attacking);
            if (_gettingDamage != null)
                StopCoroutine(_gettingDamage);
            
            _animator.PlayHitReaction();
            _gettingDamage = StartCoroutine(ProcessingGettingDamage(animationLength));
        }

        public void GetDamage(int damage)
        {
            onGetDamage?.Invoke(damage);
        }

        public virtual void Die()
        {
            if (!TryChangeState(UnitViewState.Dead))
                return;
            
            _animator.PlayDead();
            var col = GetComponent<Collider>();
            if (col)
                col.enabled = false;
            if (_gettingDamage != null)
                StopCoroutine(_gettingDamage);
            if (_attacking != null)
                StopCoroutine(_attacking);
        }
        
        protected void UpdateAnimator()
        {
            _animator.ApplyVelocity(_localVelocity);
            _animator.UpdateDistanceToCamera(Vector3.Distance(_mainCamera.transform.position, transform.position));
        }

        private void AttackCheck(int damage, int layer)
        {
            Collider[] cols = new Collider[5];
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

        protected bool TryChangeState(UnitViewState newState)
        {
            if (newState == state)
                return true;
            
            switch (newState)
            {
                case UnitViewState.Idle:
                    if (state is not UnitViewState.Dead)
                    {
                        state = newState;
                        return true;
                    }
                    break;
                case UnitViewState.Staying:
                    if (state is UnitViewState.Moving or UnitViewState.Idle)
                    {
                        state = newState;
                        return true;
                    }
                    break;
                case UnitViewState.Moving:
                    if (state is UnitViewState.Staying or UnitViewState.Shifting or UnitViewState.Idle)
                    {
                        state = newState;
                        return true;
                    }
                    break;
                case UnitViewState.Attacking:
                    if (state is UnitViewState.Shifting or UnitViewState.Staying or UnitViewState.Moving or UnitViewState.Idle)
                    {
                        state = newState;
                        return true;
                    }
                    break;
                case UnitViewState.Shifting:
                    if (state is UnitViewState.Moving or UnitViewState.Staying or UnitViewState.Idle)
                    {
                        state = newState;
                        return true;
                    }
                    break;
                case UnitViewState.GettingDamage:
                    if (state is not UnitViewState.Dead)
                    {
                        state = newState;
                        return true;
                    }
                    break;
                case UnitViewState.Dead:
                    state = UnitViewState.Dead;
                    return true;
            }

            return false;
        }

        public abstract void SetStats(UnitStats stats);
        public abstract void WarpTo(Vector3 position);
        public abstract void MoveToPosition(Vector3 position, bool shift);
        public abstract void MoveToDirection(Vector3 direction, bool shift);
        protected abstract void ProceedMovement();
        protected abstract void Rotate();

        IEnumerator ProcessingAttack(float length, float hitOffset, int damage)
        {
            yield return new WaitForSeconds(hitOffset);
            AttackCheck(damage, (int)_appearanceData.animationSet);
            yield return new WaitForSeconds(length - hitOffset);
            TryChangeState(UnitViewState.Idle);
        }

        IEnumerator ProcessingGettingDamage(float length)
        {
            yield return new WaitForSeconds(length);
            TryChangeState(UnitViewState.Idle);
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

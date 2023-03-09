using System;
using AssetsManagement;
using Units.Appearance;
using Units.Structures;
using UnityEngine;

namespace Units.MVC.View
{
    public abstract class UnitView : MonoBehaviour
    {
        public Action<Vector3> onPositionChanged;
        public ViewState movingState { get; protected set; }
        
        protected UnitAppearanceController _appearance;
        protected UnitAnimatorController _animator;
        protected UnitAppearance _appearanceData;
        private bool _isVisualsInitialized;
        
        private void Update()
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

        public abstract void SetStats(UnitStats stats);
        public abstract void WarpTo(Vector3 position);
        public abstract void MoveToPosition(Vector3 position);
        public abstract void MoveToDirection(Vector3 direction);
        public abstract void Attack();
        public abstract void SetFightReady(bool value);

        protected abstract void ProceedMovement();
        protected abstract void Rotate();
        protected abstract void UpdateAnimator();
    }
}

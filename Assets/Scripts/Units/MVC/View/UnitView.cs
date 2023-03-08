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
        public MovingStatus movingState { get; protected set; }
        
        protected UnitAppearanceController _appearance;
        protected UnitAnimatorController _animator;
        protected UnitAppearance _appearanceData;
        private bool _isVisualsInitialized;

        public virtual void SetAppearance(UnitAppearance appearance)
        {
            _appearanceData = appearance;
            AssetsManager.LoadAsset(appearance.prefab, transform, OnAppearancePrefabLoaded);
        }
        
        public virtual void SetStats(UnitStats stats) { }
        public virtual void WarpTo(Vector3 position) { }
        
        private void Update()
        {
            ProceedMovement();
            Rotate();
            
            if (!_isVisualsInitialized)
                return;
            
            UpdateAnimator();
        }
        
        protected virtual void ProceedMovement() { }
        protected virtual void Rotate() { }
        protected virtual void UpdateAnimator() { }

        private void OnAppearancePrefabLoaded(GameObject appearanceGO)
        {
            if (!appearanceGO)
                return;
            
            _appearance = appearanceGO.GetComponent<UnitAppearanceController>();;
            _appearance.SetAppearance(_appearanceData);

            _animator = appearanceGO.GetComponent<UnitAnimatorController>();
            if (_animator != null)
            {
                _animator.SetAnimatorActive(true);
                _isVisualsInitialized = true;
            }
        }
    }
}

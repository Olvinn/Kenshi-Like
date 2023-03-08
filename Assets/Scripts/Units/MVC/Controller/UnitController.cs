using Units.MVC.Model;
using Units.MVC.View;
using UnityEngine;

namespace Units.MVC.Controller
{
    public abstract class UnitController : MonoBehaviour
    {
        protected UnitModel _model;
        protected UnitView _view;

        public void SetUp(UnitModel model, UnitView view)
        {
            Clear();
            _view = view;
            _model = model;
            UpdateView();
            UpdateSubscriptions();
        }

        public virtual void Clear()
        {
            if (_model != null)
                ClearModelSubscriptions();
            if (_view != null)
                ClearViewSubscriptions();
            
            _model = null;
            _view = null;
        }

        public virtual Vector3 GetViewPosition()
        {
            return _view.transform.position;
        }

        private void UpdateView()
        {
            if (_model == null || _view == null)
                return;
            _view.SetStats(_model.GetStats());
            _view.SetAppearance(_model.GetAppearance());
            _view.WarpTo(_model.GetPosition());
        }

        private void UpdateSubscriptions()
        {
            if (_model == null || _view == null)
                return;
            
            _model.onPositionChanged = _view.WarpTo;
            _view.onPositionChanged = _model.SetPositionSilent;
        }

        private void ClearModelSubscriptions()
        {
            _model.onPositionChanged = null;
        }

        private void ClearViewSubscriptions()
        {
            _view.onPositionChanged = null;
        }
    }
}

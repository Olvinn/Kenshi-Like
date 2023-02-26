using Units.MVC.Model;
using Units.MVC.View;
using UnityEngine;

namespace Units.MVC.Controller
{
    public class UnitController : MonoBehaviour
    {
        private UnitModel _model;
        private UnitView _view;

        public void SetModel(UnitModel model)
        {
            if (_model != null)
                ClearModelSubscriptions();
            _model = model;
            UpdateView();
            UpdateSubscriptions();
        }

        public void SetView(UnitView view)
        {
            if (_view != null)
                ClearViewSubscriptions();
            _view = view;
            UpdateView();
            UpdateSubscriptions();
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
            _model.onSetDestination = _view.MoveTo;
            _model.onPositionChange = _view.WarpTo;
            _view.onPositionChanged = _model.SetPositionSilent;
        }

        private void ClearModelSubscriptions()
        {
            _model.onSetDestination = null;
            _model.onPositionChange = null;
        }

        private void ClearViewSubscriptions()
        {
            _view.onPositionChanged = null;
        }
    }
}

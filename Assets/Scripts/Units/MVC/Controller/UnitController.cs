using System;
using Units.MVC.Model;
using Units.MVC.View;
using UnityEngine;

namespace Units.MVC.Controller
{
    public class UnitController : MonoBehaviour
    {
        private UnitModel _model;
        private UnitView _view;

        private void Update()
        {
            if (_model == null || _view == null)
                return;
            _model.UpdatePosition(_view.transform.position);
        }

        public void SetModel(UnitModel model)
        {
            _model = model;
            SubscribeOnModelEvents();
            UpdateView();
        }

        public void SetView(UnitView view)
        {
            _view = view;
            SubscribeOnModelEvents();
            UpdateView();
        }

        private void UpdateView()
        {
            if (_model == null || _view == null)
                return;
            _view.SetStats(_model.GetStats());
        }

        private void SubscribeOnModelEvents()
        {
            if (_model == null || _view == null)
                return;
            _model.onDestinationChanged = _view.MoveTo;
            _model.onPositionChanged = _view.WarpTo;
        }
    }
}

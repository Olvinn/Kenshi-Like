using Units.MVC.Model;
using Units.MVC.View;
using UnityEngine;

namespace Units.MVC.Controller
{
    public class UnitController
    {
        public UnitModel model;
        public UnitView view;

        public void SetUp(UnitModel model, UnitView view)
        {
            Clear();
            this.view = view;
            this.model = model;
            UpdateView();
            UpdateSubscriptions();
        }

        public void Clear()
        {
            if (model != null)
                ClearModelSubscriptions();
            if (view != null)
                ClearViewSubscriptions();
            
            model = null;
            view = null;
        }

        public Vector3 GetViewPosition()
        {
            return view.transform.position;
        }

        private void UpdateView()
        {
            if (model == null || view == null)
                return;
            view.SetStats(model.GetStats());
            view.SetAppearance(model.GetAppearance());
            view.WarpTo(model.GetPosition());
        }

        private void UpdateSubscriptions()
        {
            if (model == null || view == null)
                return;
            
            model.onPositionChanged = view.WarpTo;
            view.onPositionChanged = model.SetPositionSilent;
        }

        private void ClearModelSubscriptions()
        {
            model.onPositionChanged = null;
        }

        private void ClearViewSubscriptions()
        {
            view.onPositionChanged = null;
        }
    }
}

using AssetsManagement;
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
            model.onHPChanged = OnModelHPChanged;
            view.onPositionChanged = model.SetPositionSilent;
            view.onGetDamage = model.GetDamage;
        }

        private void ClearModelSubscriptions()
        {
            model.onPositionChanged = null;
            model.onHPChanged = null;
        }

        private void ClearViewSubscriptions()
        {
            view.onPositionChanged = null;
            view.onGetDamage = null;
        }

        public void OnModelHPChanged(int hp)
        {
            if (model.GetStats().healthPoints <= 0)
                view.Die();
            else
                view.GetDamage(AnimationsHelper.singleton.GetReaction1Duration(model.GetAppearance().animationSet, 0));
        }
    }
}

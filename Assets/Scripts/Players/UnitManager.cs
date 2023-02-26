using System.Collections.Generic;
using Units.MVC.Controller;
using Units.MVC.Model;
using Units.MVC.View;
using UnityEngine;

namespace Players
{
    public class UnitManager
    {
        public int unitsCount => _units.Count;

        private List<UnitModel> _units;

        public UnitManager()
        {
            _units = new List<UnitModel>();
        }

        public void AddUnit(UnitModel model)
        {
            var controller = UnitControllerFactory.Create();
            var view = UnitViewFactory.Create();
            controller.SetModel(model);
            controller.SetView(view);
            _units.Add(model);
        }

        public void Move(Vector3 destination)
        {
            foreach (var model in _units)
            {
                model.MoveTo(destination);
            }
        }
    }
}

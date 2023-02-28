using System.Collections.Generic;
using Units.MVC.Controller;
using Units.MVC.Model;
using Units.MVC.View;
using UnityEngine;

namespace Actors
{
    public class UnitManager
    {
        public int unitsCount => _units.Count;

        private Dictionary<UnitModel, UnitController> _units;

        public UnitManager()
        {
            _units = new Dictionary<UnitModel, UnitController>();
        }

        public void AddUnit(UnitModel model)
        {
            if (_units.ContainsKey(model))
                return;
            var controller = UnitControllerFactory.Create();
            var view = UnitViewFactory.Create();
            controller.SetUp(model, view);
            _units.Add(model, controller);
        }

        public void Move(Vector3 destination)
        {
            foreach (var model in _units.Keys)
            {
                model.MoveTo(destination);
            }
        }

        public void Move(UnitModel unit, Vector3 destination)
        {
            if (!_units.ContainsKey(unit))
                return;
            unit.MoveTo(destination);
        }
    }
}

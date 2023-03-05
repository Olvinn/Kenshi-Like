using System;
using System.Collections.Generic;
using Units.Commands;
using Units.MVC.Controller;
using Units.MVC.Model;
using Units.MVC.View;
using UnityEngine;

namespace Players
{
    public class UnitManager
    {
        public Action<UnitModel> onUnitCompleteCommands;
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
            controller.onCommandsComplete = () => onUnitCompleteCommands(model);
        }

        public void Move(Vector3 destination)
        {
            foreach (var controller in _units.Values)
            {
                controller.AddCommand(new UnitCommandMove(destination));
            }
        }

        public void Move(UnitModel unit, Vector3 destination)
        {
            if (!_units.ContainsKey(unit))
                return;
            _units[unit].AddCommand(new UnitCommandMove(destination));
        }
    }
}

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

        private Dictionary<UnitModel, CommandsUnitController> _units;

        public UnitManager()
        {
            _units = new Dictionary<UnitModel, CommandsUnitController>();
        }

        public void AddUnit(UnitModel model)
        {
            if (_units.ContainsKey(model))
                return;
            var controller = UnitControllerFactory.CreateNPC();
            var view = UnitViewFactory.CreateRTS();
            controller.SetUp(model, view);
            _units.Add(model, controller);
            controller.onCommandsComplete = () => onUnitCompleteCommands(model);
        }

        public void AddCommand(UnitModel unit, UnitCommand command)
        {
            if (!_units.ContainsKey(unit))
                return;
            
            _units[unit].AddCommand(command);
        }
    }
}

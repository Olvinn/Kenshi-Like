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

        private Dictionary<UnitModel, NPCUnitController> _units;

        public UnitManager()
        {
            _units = new Dictionary<UnitModel, NPCUnitController>();
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

        public void Move(UnitModel unit, Vector3 destination)
        {
            if (!_units.ContainsKey(unit))
                return;
            _units[unit].AddCommand(new UnitCommandMove(destination));
        }
    }
}

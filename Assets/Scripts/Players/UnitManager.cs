using System.Collections.Generic;
using Units.MVC.Controller;
using Units.MVC.Model;
using Units.MVC.View;

namespace Players
{
    public class UnitManager
    {
        public int unitsCount => _units.Count;

        private List<UnitController> _units;

        public UnitManager()
        {
            _units = new List<UnitController>();
        }

        public void AddUnit(UnitModel model)
        {
            var controller = UnitControllerFactory.Create();
            var view = UnitViewFactory.Create();
            controller.SetModel(model);
            controller.SetView(view);
            _units.Add(controller);
        }
    }
}

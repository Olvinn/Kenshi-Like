using Units.MVC.Model;
using Units.MVC.View;
using UnityEngine;

namespace Units.MVC.Controller
{
    public sealed class PlayerUnitController : UnitController
    {
        protected CharacterControllerUnitView _characterControllerView;

        private void Update()
        {
            _characterControllerView.Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        }

        public void SetUp(UnitModel model, CharacterControllerUnitView view)
        {
            base.SetUp(model, view);
            _characterControllerView = view;
        }

        public override void Clear()
        {
            base.Clear();

            _characterControllerView = null;
        }
    }
}

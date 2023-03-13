using Units.MVC.Model;
using Units.MVC.View;
using UnityEngine;

namespace Units.MVC.Controller
{
    public sealed class DirectUnitController : MonoBehaviour
    {
        //Composition over inheritance. There is no way to keep inheritance clean and following Liskov principle
        private UnitController _baseController;

        private void Awake()
        {
            _baseController = new UnitController();
        }

        private void Update()
        {
            _baseController.view.MoveToDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            
            if (Input.GetMouseButtonDown(0))
                _baseController.Attack();
        }

        public void SetUp(UnitModel model, CharacterControllerUnitView view)
        {
            _baseController.SetUp(model, view);
        }

        public void Clear()
        {
            _baseController.Clear();

            _baseController.view = null;
        }
    }
}

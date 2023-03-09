using Units.MVC.Model;
using Units.MVC.View;
using UnityEngine;

namespace Units.MVC.Controller
{
    public sealed class DirectUnitController : MonoBehaviour
    {
        private CharacterControllerUnitView _characterControllerView;

        //Composition over inheritance. There is no way to keep inheritance clean and following Liskov principle
        private UnitController<CharacterControllerUnitView> _baseController;

        private bool _isFightReady;

        private void Awake()
        {
            _baseController = new UnitController<CharacterControllerUnitView>();
        }

        private void Update()
        {
            _characterControllerView.MoveToDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            
            if (Input.GetMouseButtonDown(0))
                _characterControllerView.Attack();
            
            if (Input.GetKeyDown(KeyCode.R))
                SetFightReady(!_isFightReady);
        }

        public void SetUp(UnitModel model, CharacterControllerUnitView view)
        {
            _baseController.SetUp(model, view);
            _characterControllerView = view;
        }

        public void Clear()
        {
            _baseController.Clear();

            _characterControllerView = null;
        }

        private void SetFightReady(bool value)
        {
            if (_isFightReady == value)
                return;
            
            _isFightReady = value;
            _baseController.view.SetFightReady(value);
        }
    }
}

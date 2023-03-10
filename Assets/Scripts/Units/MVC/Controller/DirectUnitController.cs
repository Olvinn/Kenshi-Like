using System;
using AssetsManagement;
using Units.MVC.Model;
using Units.MVC.View;
using UnityEngine;

namespace Units.MVC.Controller
{
    public sealed class DirectUnitController : MonoBehaviour
    {
        //Composition over inheritance. There is no way to keep inheritance clean and following Liskov principle
        private UnitController _baseController;

        private bool _isFightReady;

        private void Awake()
        {
            _baseController = new UnitController();
        }

        private void Update()
        {
            _baseController.view.MoveToDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            
            if (Input.GetMouseButtonDown(0))
                _baseController.view.Attack(AnimationsHelper.singleton.GetAttack1TimeDuration(_isFightReady ? 1 : 0), 
                    AnimationsHelper.singleton.GetAttack1HitOffset(_isFightReady ? 1 : 0));
            
            if (Input.GetKeyDown(KeyCode.R))
                SetFightReady(!_isFightReady);
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

        private void SetFightReady(bool value)
        {
            if (_isFightReady == value)
                return;
            
            _isFightReady = value;
            _baseController.view.SetFightReady(value);
        }
    }
}

using System;
using System.Collections.Generic;
using Units.Commands;
using Units.MVC.Model;
using Units.MVC.View;
using Units.Structures;
using UnityEngine;

namespace Units.MVC.Controller
{
    public sealed class CommandsUnitController : MonoBehaviour
    {
        public Action onCommandsComplete;
        public bool isBusy => _currentCommand != null || _commands.Count > 0;
        public UnitState state { get; private set; }
        
        private LinkedList<UnitCommand> _commands;
        private UnitCommand _currentCommand;
        
        //Composition over inheritance. There is no way to keep inheritance clean and following Liskov principle
        private UnitController _baseController;

        private void Awake()
        {
            _commands = new LinkedList<UnitCommand>();
            _baseController = new UnitController();
        }

        private void Update()
        {
            if (!_baseController.view)
                return;

            ProceedCommands();
            UpdateState();
        }

        public void SetUp(UnitModel model, NavMeshUnitView view)
        {
            _baseController.SetUp(model, view);
        }

        public void Clear()
        {
            _baseController.Clear();
            
            _commands.Clear();
            
            if (_currentCommand != null)
                _currentCommand.Dispose();
            _currentCommand = null;
        }

        public void AddCommand(UnitCommand command)
        {
            _commands.AddLast(command);
        }

        public void Attack()
        {
            _baseController.Attack();
        }

        public void MoveTo(Vector3 destination)
        {
            state = UnitState.Moving;
            _baseController.view.MoveToPosition(destination, false);
        }

        public Vector3 GetViewPosition()
        {
            return _baseController.GetViewPosition();
        }

        private void UpdateState()
        {
            switch (_baseController.view.state)
            {
                case UnitViewState.Moving:
                    state = UnitState.Moving;
                    break;
                case UnitViewState.Staying:
                    state = UnitState.Idle;
                    break;
            }
        }

        private void ProceedCommands()
        {
            if (_commands.Count > 0 && _currentCommand == null)
            {
                CommandDequeueAndExecute();
            }
            
            if (_currentCommand != null)
                _currentCommand.Update();
        }

        private void CommandDequeueAndExecute()
        {
            if (_currentCommand != null)
            {
                _currentCommand.Dispose();
                _currentCommand = null;
            }

            if (_commands.Count == 0)
            {
                onCommandsComplete?.Invoke();
                return;
            }
            
            _currentCommand = _commands.First.Value;
            _commands.RemoveFirst();
            _currentCommand.onComplete = CommandDequeueAndExecute;
            _currentCommand.Execute(this);
        }
    }
}

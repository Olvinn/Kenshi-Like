using System;
using System.Collections.Generic;
using Units.Commands;
using Units.MVC.Model;
using Units.MVC.View;
using Units.Structures;
using UnityEngine;

namespace Units.MVC.Controller
{
    public class NPCUnitController : UnitController
    {
        public Action onCommandsComplete;
        public bool isBusy => _currentCommand != null || _commands.Count > 0;
        public UnitState state { get; private set; }
        
        private UnitModel _model;
        private NavMeshUnitView _navMeshView;
        private LinkedList<UnitCommand> _commands;
        private UnitCommand _currentCommand;

        private void Awake()
        {
            _commands = new LinkedList<UnitCommand>();
        }

        private void Update()
        {
            if (!_navMeshView)
                return;

            ProceedCommands();
            UpdateState();
        }

        public void SetUp(UnitModel model, NavMeshUnitView view)
        {
            base.SetUp(model, view);
            _navMeshView = view;
        }

        public override void Clear()
        {
            base.Clear();
            
            _navMeshView = null;
            
            _commands.Clear();
            
            if (_currentCommand != null)
                _currentCommand.Dispose();
            _currentCommand = null;
        }

        // --- Non-Liskov methods ---
        public void AddCommand(UnitCommand command)
        {
            _commands.AddLast(command);
        }

        public void MoveTo(Vector3 destination)
        {
            state = UnitState.Moving;
            _navMeshView.MoveTo(destination);
        }
        // --------------------------

        private void UpdateState()
        {
            switch (_navMeshView.movingState)
            {
                case MovingStatus.Moving:
                    state = UnitState.Moving;
                    break;
                case MovingStatus.Staying:
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

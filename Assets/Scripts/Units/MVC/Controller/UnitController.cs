using System;
using System.Collections.Generic;
using Units.Commands;
using Units.MVC.Model;
using Units.MVC.View;
using Units.Structures;
using UnityEngine;

namespace Units.MVC.Controller
{
    public class UnitController : MonoBehaviour
    {
        public Action onCommandsComplete;
        public bool isBusy => _currentCommand != null || _commands.Count > 0;
        public UnitState state { get; private set; }
        
        private UnitModel _model;
        private UnitRTSView _view;
        private LinkedList<UnitCommand> _commands;
        private UnitCommand _currentCommand;

        private void Awake()
        {
            _commands = new LinkedList<UnitCommand>();
        }

        private void Update()
        {
            if (!_view)
                return;

            ProceedCommands();
            UpdateState();
        }

        public void SetUp(UnitModel model, UnitRTSView view)
        {
            Clear();
            _view = view;
            _model = model;
            UpdateView();
            UpdateSubscriptions();
        }

        public void Clear()
        {
            if (_model != null)
                ClearModelSubscriptions();
            if (_view != null)
                ClearViewSubscriptions();
            
            _model = null;
            _view = null;
            
            _commands.Clear();
            
            if (_currentCommand != null)
                _currentCommand.Dispose();
            _currentCommand = null;
        }

        public void AddCommand(UnitCommand command)
        {
            _commands.AddLast(command);
        }

        public void MoveTo(Vector3 destination)
        {
            state = UnitState.Moving;
            _view.MoveTo(destination);
        }

        public Vector3 GetViewPosition()
        {
            return _view.transform.position;
        }

        private void UpdateState()
        {
            switch (_view.movingState)
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

        private void UpdateView()
        {
            if (_model == null || _view == null)
                return;
            _view.SetStats(_model.GetStats());
            _view.SetAppearance(_model.GetAppearance());
            _view.WarpTo(_model.GetPosition());
        }

        private void UpdateSubscriptions()
        {
            if (_model == null || _view == null)
                return;
            
            _model.onPositionChanged = _view.WarpTo;
            _view.onPositionChanged = _model.SetPositionSilent;
        }

        private void ClearModelSubscriptions()
        {
            _model.onPositionChanged = null;
        }

        private void ClearViewSubscriptions()
        {
            _view.onPositionChanged = null;
            _view.onReachDestination = null;
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

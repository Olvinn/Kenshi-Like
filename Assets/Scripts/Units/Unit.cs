using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Units.Commands;
using UnityEngine;
using Damages;
using Data;
using Units.Views;

namespace Units
{
    public class Unit
    {
        public event Action<Unit> OnDie; 
        public bool IsDead => _currentHP <= 0;
        public Vector3 Position => View.Position;
        public UnitView View { get; private set; }
        public TeamEnum Team { get; private set; }
        public float HPPercentage => _currentHP / _data.GetAttribute(AttributeType.HealthPoints);

        private Character _data;
        private LinkedList<Command> _commands;
        private Command _currentCommand;
        private bool _isExecutingCommands = false;

        private float _currentHP;
        private bool _isBusy = false;

        public List<string> GetListOfCommands()
        {
            List<string> result = new List<string>();
            if (_currentCommand != null)
                result.Add(_currentCommand.CommandName);
            if (_commands.Count != 0)
                result.AddRange(_commands.Select((x) => x.CommandName));
            return result;
        }

        public Unit(Character data)
        {
            _data = data;
            _commands = new LinkedList<Command>();
            _currentHP = _data.GetAttribute(AttributeType.HealthPoints);
        }
        
        public void InjectView(UnitView view)
        {
            View = view;
            view.SetMaxSpeed(_data.GetAttribute(AttributeType.Speed));
            view.SetAppearance(_data);
        }

        public void Update()
        {
            if (IsDead)
                return;

            if (!_isExecutingCommands && !_isBusy)
            {
                ProcessSense();
            }

            if (_currentCommand != null)
                _currentCommand.Update();
            
            if (_commands.Count > 0 && !_isExecutingCommands)
                ExecuteCommands();
        }

        public void Die()
        {
            View.Die();
            ClearCommands();
            OnDie?.Invoke(this);
        }

        public void GetDamage(Damage dmg)
        {
            if (IsDead)
                return;
                
            if (dmg.source == this || dmg.source.Team == Team)
                return;
            
            View.PerformGetDamageAnimation();
            _currentHP -= dmg.damage;
            if (_currentHP <= 0)
                Die();

            if (_currentCommand == null || _currentCommand.Type == CommandType.Attack)
            {
                var c = _currentCommand as AttackCommand;
                if (c == null)
                    return;
                if (Vector3.Distance(c.Target.Position, Position) > Vector3.Distance(dmg.source.Position, Position))
                    AddReactionCommand(new AttackCommand(this, dmg.source, false));
            }
        }

        public void Attack(Unit target)
        {
            if (IsDead)
                return;
            
            View.RotateOn(target.View);
            View.PerformAttackAnimation(_data.GetAttribute(AttributeType.AttackRate), (units) =>
            {
                foreach (var unit in units)
                {
                    unit.Model.GetDamage(new Damage() { source = this, damage = _data.GetAttribute(AttributeType.Damage) });
                }
            });
        }
        
        public void MoveTo(Transform destination)
        {
            if (IsDead)
                return;
            View.MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            if (IsDead)
                return;
            View.MoveTo(destination);
        }

        public void AddCommand(Command command)
        {
            if (IsDead)
                return;
            
            if (command == null)
                return;

            if (_currentCommand != null)
            {
                if (_currentCommand.Equals(command))
                    return;
                if (_currentCommand.IsDirectCommand && !command.IsDirectCommand)
                    return;
                if (!_currentCommand.IsDirectCommand && command.IsDirectCommand)
                    ClearCommands();

                _commands.AddLast(command);
            }
            else
            {
                _currentCommand = command;
                _currentCommand.OnDone += ContinueCommands;
                _currentCommand.Execute();
            }
        }

        private void AddReactionCommand(Command command)
        {
            if (IsDead)
                return;
            
            if (command == null)
                return;

            if (_currentCommand != null)
            {
                if (_currentCommand.Equals(command))
                    return;
                if (_currentCommand.IsDirectCommand)
                    return;
            }

            if (_commands.Contains(command))
                _commands.Remove(command);

            if (_currentCommand != null)
            {
                _currentCommand.Interrupt();
                _commands.AddFirst(_currentCommand);
            }

            _commands.AddFirst(command);
            _isExecutingCommands = true;
            ContinueCommands();
        }

        public void ClearCommands()
        {
            if (_currentCommand == null)
                return;
            
            foreach (var command in _commands)
                command.Dispose();
            _commands.Clear();
            _currentCommand.Dispose();
            _currentCommand = null;
            _isExecutingCommands = false;
        }

        public void SetTeam(TeamEnum team)
        {
            Team = team;
        }

        private void ExecuteCommands()
        {
            if (IsDead)
                return;
            if (_isExecutingCommands)
                return;
            
            _isExecutingCommands = true;
            ContinueCommands();
        }

        private void ContinueCommands()
        {
            if (_currentCommand != null)
            {
                _currentCommand.Dispose();
                _currentCommand = null;
            }

            if (_commands.Count == 0)
            {
                _isExecutingCommands = false;
            }
            else
            {
                _currentCommand = _commands.First.Value;
                _commands.RemoveFirst();
                _currentCommand.OnDone += ContinueCommands;
                _currentCommand.Execute();
            }
        }

        private void ProcessSense()
        {
            _isBusy = true;
            
            float d = float.MaxValue;
            Unit enemy = null;
            foreach (var view in View.Sensed)
            {
                if (view.Model != this && !view.Model.IsDead && view.Model.Team != Team)
                {
                    float temp = Vector3.Distance(Position, view.Model.Position);
                    if (temp < d)
                    {
                        enemy = view.Model;
                        d = temp;
                    }
                }
            }

            if (enemy != null)
            {
                AddReactionCommand(new AttackCommand(this, enemy, false));
            }

            _isBusy = false;
        }
    }

    public enum TeamEnum
    {
        Player,
        EnemyAI
    }
}

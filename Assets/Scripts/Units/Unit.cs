using System;
using System.Collections.Generic;
using System.Linq;
using Units.Commands;
using UnityEngine;
using Damages;
using Data;
using Units.Views;
using Random = UnityEngine.Random;

namespace Units
{
    public class Unit
    {
        public event Action<Unit> OnDie; 
        public bool IsDead => _currentHP <= 0;
        public Vector3 Position => View.Position;
        public UnitView View { get; private set; }
        public TeamEnum Team { get; private set; }
        public float HPPercentage => _currentHP / _data.GetParameter(ParametersType.HealthPoints);

        private Character _data;
        private LinkedList<Command> _commands;
        private Command _currentCommand;
        private bool _isExecutingCommands = false;

        private float _currentHP;
        private bool _isBusy = false;
        private float _attackDelay = 0f;
        private float _savedTime;
        private int _attackers;

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
            _currentHP = _data.GetParameter(ParametersType.HealthPoints);
        }
        
        public void InjectView(UnitView view)
        {
            View = view;
            view.SetMaxSpeed(_data.GetParameter(ParametersType.Speed));
            view.SetAppearance(_data);
            view.OnHit = OnHit;
        }

        public void Update()
        {
            if (IsDead)
                return;

            float delta = Time.time - _savedTime;

            if (_attackDelay > 0)
                _attackDelay -= delta;

            if (!_isExecutingCommands && !_isBusy)
            {
                ProcessSense();
            }

            if (_currentCommand != null)
                _currentCommand.Update();
            
            if (_commands.Count > 0 && !_isExecutingCommands)
                ExecuteCommands();

            _savedTime = Time.time;
        }

        public void Die()
        {
            View.Die();
            ClearCommands();
            OnDie?.Invoke(this);
        }

        public void DetectAttack(Unit attacker)
        {
            _attackers++;
            
            if (View.IsDodging() || View.IsBlocking())
                return;
            
            if (attacker._data.GetParameter(ParametersType.AttackRate) <
                _data.GetParameter(ParametersType.AttackRate) || _attackDelay > 0 || _attackers >= 3)
            {
                if (Random.Range(0f, 1f) < _data.GetParameter(ParametersType.DodgeChance))
                    View.Dodge();
                else if (Random.Range(0f, 1f) < _data.GetParameter(ParametersType.BlockChance))
                    View.Block();
            }
        }

        public void GetDamage(Damage dmg)
        {
            if (IsDead)
                return;

            _attackers = 0;
            if (dmg.source == this || dmg.source.Team == Team)
                return;
            
            if (!View.IsDodging() && !View.IsBlocking())
            {
                View.GetDamage();
                _currentHP -= dmg.damage;
                if (_currentHP <= 0)
                    Die();
            }
            else if (View.IsBlocking())
            {
                View.SuccesfullBlock();
            }

            if (_currentCommand == null || _currentCommand.Type == CommandType.Attack)
            {
                var c = _currentCommand as AttackCommand;
                if (c == null)
                    return;
                if (Vector3.Distance(c.Target.Position, Position) > Vector3.Distance(dmg.source.Position, Position))
                    AddReactionCommand(new AttackCommand(dmg.source, false));
            }
        }

        public void Attack(Unit target)
        {
            if (IsDead || !CanAttack(target))
                return;

            _attackDelay = _data.GetParameter(ParametersType.AttackDelay);
            View.RotateOn(target.View);
            View.Attack(_data.GetParameter(ParametersType.AttackRate));
            target.DetectAttack(this);
        }
        
        public void MoveTo(Transform destination, float stopDistance)
        {
            if (IsDead)
                return;
            View.MoveTo(destination, stopDistance);
        }

        public void MoveTo(Vector3 destination, float stopDistance)
        {
            if (IsDead)
                return;
            View.MoveTo(destination, stopDistance);
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
                _currentCommand.Do(this);
            }
        }

        public bool CanAttack(Unit unit)
        {
            return _attackDelay <= 0 && View.CanAttack(unit.View);
        }
        
        private void OnHit(List<UnitView> units)
        {
            if (units != null)
            {
                foreach (var unit in units)
                {
                    unit.Model.GetDamage(new Damage()
                        { source = this, damage = _data.GetParameter(ParametersType.Damage) });
                }
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
                _currentCommand.Do(this);
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
                AddReactionCommand(new AttackCommand(enemy, false));
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

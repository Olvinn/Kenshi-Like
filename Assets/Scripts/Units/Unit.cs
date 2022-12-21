using System;
using System.Collections.Generic;
using Units.Commands;
using UnityEngine;
using Damages;

namespace Units
{
    public class Unit
    {
        public bool IsDead => _currentHP <= 0;
        public Vector3 Position => View.transform.position;
        public UnitView View { get; private set; }
        public TeamEnum Team { get; private set; }
        public float HPPercentage => _currentHP / _data.HP;

        private UnitData _data;
        private Queue<Command> _commands;
        private Command _currentCommand;
        private bool _isExecutingCommands = false;

        private float _currentHP;

        public Unit(UnitData data, TeamEnum team)
        {
            _data = data;
            _commands = new Queue<Command>();
            Team = team;
            _currentHP = _data.HP;
        }
        
        public void InjectView(UnitView view)
        {
            View = view;
            view.SetMaxSpeed(_data.Speed);
            view.SetColor(_data.Color);
        }

        public void Update()
        {
            if (IsDead)
                return;
            
            if (_currentCommand != null)
                _currentCommand.Update();
        }

        public void Die()
        {
            View.Die();
            if (_currentCommand != null)
                _currentCommand.Dispose();
            _currentCommand = null;
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

            if (_commands.Count == 0)
            {
                AddCommand(new AttackCommand(this, dmg.source));
                ExecuteCommands();
            }
        }

        public void Attack(Unit target)
        {
            if (IsDead)
                return;
            
            View.RotateOn(target.View);
            View.PerformAttackAnimation((units) =>
            {
                foreach (var unit in units)
                {
                    unit.Model.GetDamage(new Damage() { source = this, damage = _data.Damage });
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
            _commands.Enqueue(command);
        }

        public void ExecuteCommands()
        {
            if (IsDead)
                return;
            if (_isExecutingCommands)
                return;
            
            _isExecutingCommands = true;
            ContinueCommands();
        }

        public void ClearCommands()
        {
            if (_currentCommand == null)
                return;
            
            _currentCommand.OnDone -= ExecuteCommands;
            foreach (var command in _commands)
                command.Dispose();
            _commands.Clear();
            _currentCommand.Dispose();
            _currentCommand = null;
            _isExecutingCommands = false;
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
                return;
            }
            
            _currentCommand = _commands.Dequeue();
            _currentCommand.OnDone += ContinueCommands;
            _currentCommand.Execute();
        }
    }

    public enum TeamEnum
    {
        Player,
        EnemyAI
    }
}

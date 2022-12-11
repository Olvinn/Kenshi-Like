using System;
using System.Collections.Generic;
using Units.Commands;
using UnityEngine;

namespace Units
{
    public class Unit
    {
        public bool IsDead => _currentHP <= 0;
        public Vector3 Position => View.transform.position;
        public UnitView View { get; private set; }

        public float Damage => _stats.Damage;

        private UnitStats _stats;
        private Queue<Command> _commands;
        private Command _currentCommand;
        private bool _isExecutingCommands = false;

        private float _currentHP;

        public Unit(UnitStats stats)
        {
            _stats = stats;
            _commands = new Queue<Command>();

            _currentHP = _stats.HP;
        }
        
        public void InjectView(UnitView view)
        {
            View = view;
        }

        public void Die()
        {
            View.Die();
        }

        public void GetDamage(float dmg)
        {
            _currentHP -= dmg;
            if (_currentHP <= 0)
                Die();
        }

        public void Attack(Unit target, Action afterAttack)
        {
            View.DoAttackAnimation(() =>
            {
                target.GetDamage(Damage);
                afterAttack?.Invoke();
            });
        }

        public void MoveTo(Vector3 destination)
        {
            View.MoveTo(destination);
        }

        public void AddCommand(Command command)
        {
            _commands.Enqueue(command);
        }

        public void ExecuteCommands()
        {
            if (_isExecutingCommands)
                return;
            
            _isExecutingCommands = true;
            ContinueCommands();
        }

        private void ContinueCommands()
        {
            if (_currentCommand != null)
                _currentCommand.Dispose();
            
            if (_commands.Count == 0)
            {
                _isExecutingCommands = false;
                return;
            }
            
            _currentCommand = _commands.Dequeue();
            _currentCommand.OnDone += ContinueCommands;
            _currentCommand.Execute();
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
    }
}

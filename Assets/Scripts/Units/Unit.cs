using System;
using System.Collections.Generic;
using System.Linq;
using Units.Commands;
using UnityEngine;
using Damages;
using Data;
using Interfaces;
using Units.Commands.ComplexCommands;
using Units.Views;
using Random = UnityEngine.Random;

namespace Units
{
    public class Unit: IUnit
    {
        public event Action<Unit> onDie;
        public bool IsDead => _currentHP <= 0;
        public Vector3 Position => view.position;
        public Transform transform => view.transform;
        public UnitView view { get; private set; }
        public TeamEnum team { get; private set; }
        public float HPPercentage => _currentHP / data.GetParameter(ParametersType.HealthPoints);
        public Character data;
        
        private LinkedList<Command> _commands;
        private Command _currentCommand;
        private bool _isExecutingCommands = false;
        private float _currentHP;
        private bool _isBusy = false;
        private float _attackDelay = 0f;
        private float _savedTime;

        public Unit(Character data)
        {
            this.data = data;
            _commands = new LinkedList<Command>();
            _currentHP = this.data.GetParameter(ParametersType.HealthPoints);
        }
        
        public void Update()
        {
            if (IsDead)
                return;

            float delta = Time.time - _savedTime;

            if (_attackDelay > 0)
                _attackDelay -= delta;
            
            if (!_isExecutingCommands && !_isBusy)
                ProcessSense();

            if (_currentCommand != null)
                _currentCommand.Update();
            
            if (_commands.Count > 0 && !_isExecutingCommands)
                ExecuteCommands();

            if (view.GroundType == GroundType.Water)
            {
                view.SetMaxSpeed(2f);
                view.Swim();
            }
            else
            {
                view.SetMaxSpeed(data.GetParameter(ParametersType.Speed));
                view.Run();
            }

            _savedTime = Time.time;
        }
        
        public void InjectView(UnitView view)
        {
            this.view = view;
            view.SetMaxSpeed(data.GetParameter(ParametersType.Speed));
            view.SetAppearance(data.appearance);
            view.OnHit = OnCompleteAttack;
        }

        public List<string> GetListOfCommands()
        {
            List<string> result = new List<string>();
            if (_currentCommand != null)
                result.Add(_currentCommand.CommandName);
            if (_commands.Count != 0)
                result.AddRange(_commands.Select((x) => x.CommandName));
            return result;
        }

        public void Die()
        {
            view.Die();
            ClearCommandQueue();
            onDie?.Invoke(this);
        }

        /// <summary>
        /// Detect attackers desire to hit
        /// </summary>
        public void PreGetDamage(IUnit attacker)
        {
            if (view.IsDodging() || view.IsBlocking())
                return;

            if (Random.Range(0f, 1f) < data.GetParameter(ParametersType.DodgeChance))
                view.Dodge();
            else 
            if (Random.Range(0f, 1f) < data.GetParameter(ParametersType.BlockChance))
                view.Block();
        }

        public void Attack(IKillable target)
        {
            if (IsDead || !CanAttack(target))
                return;

            _attackDelay = data.GetParameter(ParametersType.AttackDelay);
            view.RotateOn(target.transform);
            view.StartHit(data.GetParameter(ParametersType.AttackRate), null);
        }

        /// <summary>
        /// Proceed hit and pass damage if unit doesn't blocking or dodging 
        /// </summary>
        /// <param name="dmg"></param>
        public void GetDamage(Damage dmg)
        {
            if (IsDead)
                return;
            
            if (dmg.source == this || dmg.source.team == team)
                return;
            
            if (!view.IsDodging() && !view.IsBlocking())
            {
                view.GetDamage();
                _currentHP -= dmg.damage;
                if (_currentHP <= 0)
                    Die();
            }

            if (view.IsBlocking())
            {
                view.BlockComplete();
            }

            if (_currentCommand == null || _currentCommand.Type == CommandType.Attack)
            {
                var c = _currentCommand as FightCommand;
                if (c == null)
                    return;
                
                if (Vector3.Distance(c.Target.transform.position, Position) > Vector3.Distance(dmg.source.Position, Position))
                    AddReactionCommand(new FightCommand(dmg.source, false));
            }
        }
        
        public void Follow(Transform destination)
        {
            if (IsDead)
                return;
            view.MoveTo(destination, 1);
        }

        public void Move(Vector3 destination)
        {
            if (IsDead)
                return;
            view.MoveTo(destination, 1);
        }

        public void EnqueueCommand(Command command)
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
                    ClearCommandQueue();

                _commands.AddLast(command);
            }
            else
            {
                _currentCommand = command;
                _currentCommand.OnDone += ContinueCommands;
                _currentCommand.ExecuteBy(this);
            }
        }

        public bool CanAttack(IKillable unit)
        {
            return _attackDelay <= 0 && view.CanAttack(unit.transform);
        }
        
        private void OnCompleteAttack(List<UnitView> units)
        {
            if (units != null)
            {
                foreach (var unit in units)
                {
                    unit.Model.GetDamage(new Damage()
                        { source = this, damage = data.GetParameter(ParametersType.Damage) });
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

        public void ClearCommandQueue()
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
            this.team = team;
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
                _currentCommand.ExecuteBy(this);
            }
        }

        private void ProcessSense()
        {
            _isBusy = true;
            
            float d = float.MaxValue;
            Unit enemy = null;
            
            var list = view.Sensed.Select(x => x.Model).ToList();
            
            foreach (var view in list)
            {
                if (view != this && !view.IsDead && view.team != team)
                {
                    float temp = Vector3.Distance(Position, view.Position);
                    if (temp < d)
                    {
                        enemy = view;
                        d = temp;
                    }
                }
            }

            if (enemy != null)
            {
                AddReactionCommand(new FightCommand(enemy, false));
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

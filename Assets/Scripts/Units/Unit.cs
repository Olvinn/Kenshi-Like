using System;
using System.Collections.Generic;
using System.Linq;
using Units.Commands;
using UnityEngine;
using Damages;
using Data;
using Units.Commands.ComplexCommands;
using Units.Views;
using Random = UnityEngine.Random;

namespace Units
{
    public class Unit: IDamageable
    {
        public event Action<Unit> OnDie;
        public bool IsDestroyed => IsDead;
        public bool IsDead => _currentHP <= 0;
        public Vector3 Position => View.Position;
        public Transform ViewTransform => View.transform;
        public UnitView View { get; private set; }
        public TeamEnum Team { get; private set; }
        public float HPPercentage => _currentHP / _data.GetParameter(ParametersType.HealthPoints);

        private Character _data;
        private LinkedList<Command> _commands;
        private Command _currentCommand;
        private List<Unit> _noticedAttackers;
        private bool _isExecutingCommands = false;

        private float _currentHP;
        private bool _isBusy = false;
        private float _attackDelay = 0f;
        private float _savedTime;

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
            _noticedAttackers = new List<Unit>();
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

            foreach (var attacker in _noticedAttackers)
            {
                if (attacker.IsDead)
                {
                    _noticedAttackers.Remove(attacker);
                    break;
                }
            }
            
            if (!_isExecutingCommands && !_isBusy)
                ProcessSense();

            if (_currentCommand != null)
                _currentCommand.Update();
            
            if (_commands.Count > 0 && !_isExecutingCommands)
                ExecuteCommands();

            if (View.GroundType == GroundType.Water)
            {
                View.SetMaxSpeed(2f);
                View.Swim();
            }
            else
            {
                View.SetMaxSpeed(_data.GetParameter(ParametersType.Speed));
                View.Run();
            }

            _savedTime = Time.time;
        }

        public void Die()
        {
            View.Die();
            ClearCommands();
            OnDie?.Invoke(this);
        }

        /// <summary>
        /// Enlist attacker to proceed later
        /// </summary>
        /// <param name="attacker"></param>
        public void OnPreAttackBy(Unit attacker)
        {
            if (!_noticedAttackers.Contains(attacker))
                _noticedAttackers.Add(attacker);
        }

        /// <summary>
        /// Remove attacker from noticed attackers
        /// </summary>
        /// <param name="attacker"></param>
        public void OnPostAttackBy(Unit attacker)
        {
            if (!_noticedAttackers.Contains(attacker))
                _noticedAttackers.Remove(attacker);
        }

        /// <summary>
        /// Detect attackers desire to hit
        /// </summary>
        public void OnPreHitBy(Unit attacker)
        {
            if (View.IsDodging() || View.IsBlocking())
                return;

            if (Random.Range(0f, 1f) < _data.GetParameter(ParametersType.DodgeChance))
                View.Dodge();
            else 
            if (Random.Range(0f, 1f) < _data.GetParameter(ParametersType.BlockChance))
                View.Block();
        }

        /// <summary>
        /// Proceed hit and pass damage if unit doesn't blocking or dodging 
        /// </summary>
        /// <param name="dmg"></param>
        public void OnHitWith(Damage dmg)
        {
            if (IsDead)
                return;
            
            if (dmg.source == this || dmg.source.Team == Team)
                return;
            
            if (!View.IsDodging() && !View.IsBlocking())
            {
                View.GetDamage();
                _currentHP -= dmg.damage;
                if (_currentHP <= 0)
                    Die();
            }

            if (_currentCommand == null || _currentCommand.Type == CommandType.Attack)
            {
                var c = _currentCommand as FightCommand;
                if (c == null)
                    return;
                
                if (Vector3.Distance(c.Target.ViewTransform.position, Position) > Vector3.Distance(dmg.source.Position, Position))
                    AddReactionCommand(new FightCommand(dmg.source, false));
            }
        }

        public void Attack(IDamageable target, Action callback = null)
        {
            if (IsDead || !CanHit(target))
                return;

            _attackDelay = _data.GetParameter(ParametersType.AttackDelay);
            View.RotateOn(target.ViewTransform);
            View.StartHit(_data.GetParameter(ParametersType.AttackRate), callback);
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
                _currentCommand.ExecuteBy(this);
            }
        }

        public bool CanHit(IDamageable unit)
        {
            return _attackDelay <= 0 && View.CanAttack(unit.ViewTransform);
        }
        
        private void OnHit(List<UnitView> units)
        {
            if (units != null)
            {
                foreach (var unit in units)
                {
                    unit.Model.OnHitWith(new Damage()
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
                _currentCommand.ExecuteBy(this);
            }
        }

        private void ProcessSense()
        {
            _isBusy = true;
            
            float d = float.MaxValue;
            Unit enemy = null;

            List<Unit> list = null;
            if (_noticedAttackers.Count > 0)
                list = _noticedAttackers;
            else
                list = View.Sensed.Select(x => x.Model).ToList();
            
            foreach (var view in list)
            {
                if (view != this && !view.IsDead && view.Team != Team)
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

    public interface IDamageable
    {
        bool IsDestroyed { get; }
        Transform ViewTransform { get; }
        void OnPreAttackBy(Unit attacker);
        void OnPreHitBy(Unit attacker);
        void OnHitWith(Damage damage);
        void OnPostAttackBy(Unit attacker);
    }
}

using System.Collections.Generic;
using Damages;
using Interfaces;
using Units.Commands;
using Units.Views;
using UnityEngine;

namespace Units.Squads
{
    public class Squad : IMoving, ICommandExecutor
    {
        private Unit _leader;
        private List<Unit> _followers;
        public Transform ViewTransform => _leader.transform;
        public UnitView View => _leader.view;
        public bool IsDead => false;

        public Squad()
        {
            _followers = new List<Unit>();
        }

        public void AddUnits(List<Unit> units)
        {
            foreach (var unit in units)
            {
                AddUnit(unit);
            }
        }

        public void AddUnit(Unit unit)
        {
            if (unit == null)
                return;
            if (_leader == null)
                _leader = unit;
            else if (!_followers.Contains(unit))
                _followers.Add(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            if (_followers.Contains(unit))
                _followers.Remove(unit);
            if (_leader == unit)
            {
                _leader = null;
                ReAssignLeader();
            }
        }

        public bool HasUnit(Unit unit)
        {
            return _followers.Contains(unit);
        }

        public void SetLeader(Unit unit)
        {
            AddUnit(_leader);
            _leader = unit;
        }

        private void ReAssignLeader()
        {
            if (_followers.Count > 0)
            {
                _leader = _followers[0];
                _followers.Remove(_leader);
            }
        }

        public void PreGetDamage(IUnit attacker)
        {
            
        }

        public void GetDamage(Damage damage)
        {
            
        }
        
        public void Move(Vector3 pos)
        {
            
        }

        public void Follow(Transform target)
        {
            
        }

        public void Attack(IKillable target)
        {
            
        }

        public void EnqueueCommand(Command command)
        {
            
        }

        public void ExecuteCommands()
        {
            
        }

        public void ClearCommandQueue()
        {
            
        }
    }
}

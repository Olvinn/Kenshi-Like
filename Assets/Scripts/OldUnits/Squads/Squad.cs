using System.Collections.Generic;
using Interfaces;
using OldUnits.Commands;
using UnityEngine;

namespace OldUnits.Squads
{
    public class Squad : ISquad
    {
        private ISquadMember _leader;
        private List<ISquadMember> _followers;

        public Squad()
        {
            _followers = new List<ISquadMember>();
        }

        public void AddMembers(IEnumerable<ISquadMember> units)
        {
            foreach (var unit in units)
            {
                AddMember(unit);
            }
        }

        public void AddMember(ISquadMember unit)
        {
            if (unit == null)
                return;
            if (_leader == null)
                _leader = unit;
            else if (!_followers.Contains(unit))
                _followers.Add(unit);
        }

        public void RemoveMember(ISquadMember unit)
        {
            if (_followers.Contains(unit))
                _followers.Remove(unit);
            if (_leader == unit)
            {
                _leader = null;
                ReAssignLeader();
            }
        }

        public void RemoveAllMembers()
        {
            _leader = null;
            _followers.Clear();
        }

        private void SetLeader(ISquadMember unit)
        {
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
        
        public void Move(Vector3 pos)
        {
            
        }

        public void Follow(Transform target)
        {
            
        }

        public bool CanAttack(IKillable target)
        {
            return true;
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

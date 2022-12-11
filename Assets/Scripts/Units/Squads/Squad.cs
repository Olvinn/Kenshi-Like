using System.Collections.Generic;
using UnityEngine;

namespace Units.Squads
{
    public class Squad
    {
        private List<Unit> _units;

        public Squad()
        {
            _units = new List<Unit>();
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
            if (!_units.Contains(unit))
                _units.Add(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            if (_units.Contains(unit))
                _units.Remove(unit);
        }

        public bool HasUnit(Unit unit)
        {
            return _units.Contains(unit);
        }
    }
}

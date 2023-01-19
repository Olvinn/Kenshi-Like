using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace UI
{
    public class SquadView : Widget
    {
        [SerializeField] private Transform viewParent;

        private Dictionary<Unit, Portrait> _portraits;

        private void Start()
        {
            _portraits = new Dictionary<Unit, Portrait>();
        }

        public void AddUnit(Unit unit)
        {
            if (_portraits.ContainsKey(unit))
                return;
            
            _portraits.Add(unit, ObjectLoader.Instance.GetObject(ObjectTypes.UIPortrait) as Portrait);
            var p = _portraits[unit];
            p.transform.SetParent(viewParent);
            p.transform.localScale = Vector3.one;
            p.onClick += () => { Debug.Log(unit);};
        }
        
        public void AddUnits(List<Unit> units)
        {
            foreach (var unit in units)
                AddUnit(unit);
        }

        public void ClearUnits()
        {
            foreach (var portrait in _portraits.Values)
                portrait.Unload();
            _portraits.Clear();
        }
    }
}

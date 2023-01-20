using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace UI
{
    public class SquadView : Widget
    {
        public Action<Unit> onClicked;
        
        [SerializeField] private Transform viewParent;
        [SerializeField] private PortraitMaker portraitMaker;

        private Dictionary<Unit, Portrait> _portraits;

        private void Awake()
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
            portraitMaker.GetPortrait(unit.data.appearance, p.SetImage);
            p.onClick += () => { onClicked?.Invoke(unit); };
        }

        public void RemoveUnit(Unit unit)
        {
            if (!_portraits.ContainsKey(unit))
                return;
            
            _portraits[unit].Unload();
            _portraits.Remove(unit);
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

        public void SelectUnit(Unit unit)
        {
            _portraits[unit].Select();
        }

        public void DeselectUnit(Unit unit)
        {
            _portraits[unit].Deselect();
        }

        public void DeselectAll()
        {
            foreach (var p in _portraits.Values)
            {
                p.Deselect();
            }
        }
    }
}

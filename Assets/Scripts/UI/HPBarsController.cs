using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace UI
{
    public class HPBarsController : MonoBehaviour
    {
        [SerializeField] private Transform parent;
        [SerializeField] private HPBar prefab;

        private Dictionary<Unit, HPBar> _units;

        public void SetUpUnits(List<Unit> units)
        {
            if (_units == null)
                _units = new Dictionary<Unit, HPBar>();
            else
            {
                foreach (var unit in _units.Keys)
                {
                    Destroy(_units[unit]);
                }
                _units.Clear();
            }
            
            foreach (var unit in units)
            {
                _units.Add(unit, Instantiate(prefab, parent));
            }
        }
        
        private void LateUpdate()
        {
            if(_units == null || _units.Count == 0)
                return;

            foreach (var unit in _units.Keys)
            {
                if (unit.HPPercentage <= 0)
                {
                    _units[unit].gameObject.SetActive(false);
                    continue;
                }

                _units[unit].transform.position = Camera.main.WorldToScreenPoint(unit.Position + Vector3.up);
                _units[unit].UpdateHp(unit.HPPercentage);
            }
        }
    }
}

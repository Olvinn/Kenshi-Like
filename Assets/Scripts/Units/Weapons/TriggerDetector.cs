using System.Collections.Generic;
using System.Linq;
using Units.Views;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units.Weapons
{
    public class TriggerDetector : MonoBehaviour
    {
        public List<UnitView> Views => _views.ToList();
        [FormerlySerializedAs("collider")] [SerializeField] private Collider col;
        private List<UnitView> _views;

        private void Awake()
        {
            _views = new List<UnitView>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var unit = other.GetComponent<UnitView>();
            if (unit && unit.Model is { IsDead: false })
            {
                _views.Add(unit);
                unit.Model.OnDie += RemoveUnit;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var unit = other.GetComponent<UnitView>();
            if (unit)
                RemoveUnit(unit.Model);
        }

        void RemoveUnit(Unit unit)
        {
            if (unit == null)
            {
                foreach (var view in _views)
                {
                    if (view.Model == null)
                    {
                        _views.Remove(view);
                        break;
                    }
                }
            }
            else
            {
                if (_views.Contains(unit.View))
                {
                    _views.Remove(unit.View);
                    unit.OnDie -= RemoveUnit;
                } 
            }
        }

        private void OnEnable()
        {
            col.enabled = true;
        }

        private void OnDisable()
        {
            col.enabled = false;
        }
    }
}

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
                unit.Model.onDie += RemoveUnit;
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
                if (_views.Contains(unit.view))
                {
                    _views.Remove(unit.view);
                    unit.onDie -= RemoveUnit;
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
            _views.Clear();
        }
    }
}

using System.Collections.Generic;
using Inputs;
using Units;
using Units.Commands;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private Dictionary<UnitView, Unit> _allControlledUnits, _enemies;
        private List<Unit> _selected;

        private void Awake()
        {
            _allControlledUnits = new Dictionary<UnitView, Unit>();
            _enemies = new Dictionary<UnitView, Unit>();
            _selected = new List<Unit>();

            InputController.Instance.OnShiftRMB += ShiftRightMouseButtonClick;
            InputController.Instance.OnShiftLMB += ShiftLeftMouseButtonClick;
            InputController.Instance.OnRMB += RightMouseButtonClick;
            InputController.Instance.OnLMB += LeftMouseButtonClick;
        }

        public void AddControlledUnit(Unit unit)
        {
            _allControlledUnits.Add(unit.View, unit);
        }

        public void AddEnemyUnit(Unit unit)
        {
            _enemies.Add(unit.View, unit);
        }

        public void SelectUnit(Unit unit)
        {
            if (_allControlledUnits.ContainsKey(unit.View) && !_selected.Contains(unit))
            {
                unit.View.Select();
                _selected.Add(unit);
                unit.View.SetProvoked();
            }
        }

        public void DeselectUnit(Unit unit)
        {
            if (_selected.Contains(unit))
            {
                unit.View.Deselect();
                _selected.Remove(unit);
                unit.View.SetIdle();
            }
        }

        public void DeselectAll()
        {
            foreach (var unit in _selected)
            {
                unit.View.Deselect();
                unit.View.SetIdle();
            }

            _selected.Clear();
        }

        private void ShiftRightMouseButtonClick(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var view = hit.collider.GetComponent<UnitView>();
                if (view != null && _enemies.ContainsKey(view))
                {
                    foreach (var unit in _selected)
                    {
                        unit.AddCommand(new AttackCommand(unit, _enemies[view]));
                        unit.ExecuteCommands();
                    }
                }
                else
                {
                    foreach (var unit in _selected)
                    {
                        unit.AddCommand(new MovementCommand(unit, hit.point));
                        unit.ExecuteCommands();
                    }
                }
            }
        }
        
        private void ShiftLeftMouseButtonClick(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                var view = hit.collider.GetComponent<UnitView>();
                if (view != null && _allControlledUnits.ContainsKey(view))
                    SelectUnit(_allControlledUnits[view]);
            }
        }

        private void RightMouseButtonClick(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var view = hit.collider.GetComponent<UnitView>();
                if (view != null && _enemies.ContainsKey(view))
                {
                    foreach (var unit in _selected)
                    {
                        unit.ClearCommands();
                        unit.AddCommand(new AttackCommand(unit, _enemies[view]));
                        unit.ExecuteCommands();
                    }
                }
                else
                {
                    foreach (var unit in _selected)
                    {
                        unit.ClearCommands();
                        unit.MoveTo(hit.point);
                    }
                }
            }
        }

        private void LeftMouseButtonClick(Ray ray)
        {
            DeselectAll();
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                var view = hit.collider.GetComponent<UnitView>();
                if (view != null && _allControlledUnits.ContainsKey(view))
                    SelectUnit(_allControlledUnits[view]);
            }
        }
    }
}

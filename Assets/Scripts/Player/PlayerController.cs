using System;
using System.Collections.Generic;
using Inputs;
using Units;
using Units.Commands;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private List<Unit> _units;
        private List<Unit> _selected;

        private void Start()
        {
            _units = new List<Unit>();
            _selected = new List<Unit>();

            InputController.Instance.OnShiftRMB += ShiftRightMouseButtonClick;
            InputController.Instance.OnShiftLMB += ShiftLeftMouseButtonClick;
            InputController.Instance.OnRMB += RightMouseButtonClick;
            InputController.Instance.OnLMB += LeftMouseButtonClick;
            InputController.Instance.OnBoxSelect += BoxSelect;
        }

        public void AddControlledUnit(Unit unit)
        {
            _units.Add(unit);
        }

        public void SelectUnit(Unit unit)
        {
            if (_units.Contains(unit) && !_selected.Contains(unit) && !unit.IsDead)
            {
                unit.View.Select();
                _selected.Add(unit);
            }
        }

        public void DeselectUnit(Unit unit)
        {
            if (_selected.Contains(unit))
            {
                unit.View.Deselect();
                _selected.Remove(unit);
            }
        }

        public void DeselectAll()
        {
            foreach (var unit in _selected)
            {
                unit.View.Deselect();
                unit.View.PerformIdleAnimation();
            }

            _selected.Clear();
        }

        private void ShiftRightMouseButtonClick(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var view = hit.collider.GetComponent<UnitView>();
                if (view != null && !view.Model.IsDead)
                {
                    if (view.Model.Team == TeamEnum.Player)
                    {
                        foreach (var unit in _selected)
                        {
                            unit.AddCommand(new FollowCommand(unit, view.Model));
                            unit.ExecuteCommands();
                        }
                    }
                    else
                    {
                        foreach (var unit in _selected)
                        {
                            unit.AddCommand(new AttackCommand(unit, view.Model));
                            unit.ExecuteCommands();
                        }
                    }
                }
                else
                {
                    foreach (var unit in _selected)
                    {
                        unit.AddCommand(new MoveCommand(unit, hit.point));
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
                if (view != null && _units.Contains(view.Model))
                    SelectUnit(view.Model);
            }
        }

        private void RightMouseButtonClick(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var view = hit.collider.GetComponent<UnitView>();
                if (view != null && !view.Model.IsDead)
                {
                    if (view.Model.Team == TeamEnum.Player)
                    {
                        foreach (var unit in _selected)
                        {
                            unit.ClearCommands();
                            unit.AddCommand(new FollowCommand(unit, view.Model));
                            unit.ExecuteCommands();
                        }
                    }
                    else
                    {
                        foreach (var unit in _selected)
                        {
                            unit.ClearCommands();
                            unit.AddCommand(new AttackCommand(unit, view.Model));
                            unit.ExecuteCommands();
                        }
                    }
                }
                else
                {
                    foreach (var unit in _selected)
                    {
                        unit.ClearCommands();
                        unit.AddCommand(new MoveCommand(unit, hit.point));
                        unit.ExecuteCommands();
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
                if (view != null && _units.Contains(view.Model))
                    SelectUnit(view.Model);
            }
        }

        private void BoxSelect(Vector2 from, Vector2 to)
        {
            DeselectAll();
            Vector2 pos = new Vector2(to.x < from.x ? to.x : from.x, to.y < from.y ? to.y : from.y);
            Vector2 size = new Vector2(Mathf.Abs(to.x - from.x), Mathf.Abs(to.y - from.y));
            foreach (var unit in _units)
            {
                Vector3 screen = Camera.main.WorldToScreenPoint(unit.Position);
                if (screen.x > pos.x && screen.y > pos.y && screen.x < pos.x + size.x && screen.y < pos.y + size.y)
                    SelectUnit(unit);
            }
        }
    }
}

using System.Collections.Generic;
using Inputs;
using Units;
using Units.Commands;
using Units.Views;
using UnityEngine;

namespace Players
{
    public class PlayerController : MonoBehaviour, IPlayer
    {
        [SerializeField] private LayerMask workWithMask;
        
        private List<Unit> _units;
        private List<Unit> _selected;

        private TeamEnum _team;

        private void Awake()
        {
            _units = new List<Unit>();
            _selected = new List<Unit>();
        }

        private void Start()
        {
            InputController.Instance.OnShiftRMB += ShiftRightMouseButtonClick;
            InputController.Instance.OnShiftLMB += ShiftLeftMouseButtonClick;
            InputController.Instance.OnRMB += RightMouseButtonClick;
            InputController.Instance.OnLMB += LeftMouseButtonClick;
            InputController.Instance.OnBoxSelect += BoxSelect;
        }

        public void AddUnit(Unit unit)
        {
            _units.Add(unit);
            unit.SetTeam(_team);
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
                unit.View.Deselect();

            _selected.Clear();
        }

        public void SetTeam(TeamEnum team)
        {
            _team = team;
            foreach (var unit in _units)
            {
                unit.SetTeam(team);
            }
        }

        private void ShiftRightMouseButtonClick(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, workWithMask))
            {
                var view = hit.collider.GetComponent<UnitView>();
                if (view != null && !view.Model.IsDead)
                {
                    if (view.Model.Team == TeamEnum.Player)
                    {
                        foreach (var unit in _selected)
                        {
                            unit.AddCommand(new FollowCommand(unit, view.Model, true));
                        }
                    }
                    else
                    {
                        foreach (var unit in _selected)
                        {
                            unit.AddCommand(new AttackCommand(unit, view.Model, true));
                        }
                    }
                }
                else
                {
                    foreach (var unit in _selected)
                    {
                        unit.AddCommand(new MoveCommand(unit, hit.point, true));
                    }
                }
            }
        }
        
        private void ShiftLeftMouseButtonClick(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, workWithMask))
            {
                var view = hit.collider.GetComponent<UnitView>();
                if (view != null && _units.Contains(view.Model))
                    SelectUnit(view.Model);
            }
        }

        private void RightMouseButtonClick(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit,1000f,  workWithMask.value))
            {
                // GameObject go = new GameObject();
                // var lr = go.AddComponent<LineRenderer>();
                // lr.SetPositions(new []{ray.origin, hit.point});
                // lr.material = new Material(Shader.Find("Diffuse"));
                var view = hit.collider.GetComponent<UnitView>();
                if (view != null && !view.Model.IsDead)
                {
                    if (view.Model.Team == TeamEnum.Player)
                    {
                        // lr.material.color = Color.blue;
                        foreach (var unit in _selected)
                        {
                            unit.ClearCommands();
                            unit.AddCommand(new FollowCommand(unit, view.Model, true));
                        }
                    }
                    else
                    {
                        // lr.material.color  = Color.red;
                        foreach (var unit in _selected)
                        {
                            unit.ClearCommands();
                            unit.AddCommand(new AttackCommand(unit, view.Model,true));
                        }
                    }
                }
                else
                {
                    // lr.material.color  = Color.green;
                    foreach (var unit in _selected)
                    {
                        unit.ClearCommands();
                        unit.AddCommand(new MoveCommand(unit, hit.point,true));
                    }
                }
            }
        }

        private void LeftMouseButtonClick(Ray ray)
        {
            DeselectAll();
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, workWithMask.value))
            {
                var view = hit.collider.GetComponent<UnitView>();
                if (view != null && _units.Contains(view.Model))
                    SelectUnit(view.Model);
            }
        }

        private void BoxSelect(Vector2 pos, Vector2 size)
        {
            DeselectAll();
            foreach (var unit in _units)
            {
                Vector3 screen = Camera.main.WorldToScreenPoint(unit.Position);
                if (screen.x > pos.x && screen.y > pos.y && screen.x < pos.x + size.x && screen.y < pos.y + size.y)
                    SelectUnit(unit);
            }
        }
    }
}

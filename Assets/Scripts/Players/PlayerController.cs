using System.Collections.Generic;
using Data;
using Inputs;
using Units;
using Units.Commands;
using Units.Commands.ComplexCommands;
using Units.Commands.SimpleCommands;
using Units.Views;
using UnityEngine;

namespace Players
{
    public class PlayerController : MonoBehaviour, IPlayer
    {
        [SerializeField] private LayerMask workWithMask;
        
        public List<Unit> units;
        private List<Unit> _selected;

        private TeamEnum _team;

        private void Awake()
        {
            units = new List<Unit>();
            _selected = new List<Unit>();
        }

        private void Start()
        {
            InputController.Instance.OnShiftRMB += ShiftRightMouseButtonClick;
            InputController.Instance.OnShiftLMB += ShiftLeftMouseButtonClick;
            InputController.Instance.OnRMB += RightMouseButtonClick;
            InputController.Instance.OnLMB += LeftMouseButtonClick;
            InputController.Instance.OnBoxSelect += BoxSelect;
            InputController.Instance.OnShiftBoxSelect += ShiftBoxSelect;
        }

        public void AddUnit(Unit unit)
        {
            units.Add(unit);
            unit.SetTeam(_team);
        }

        public void SelectUnit(Unit unit)
        {
            if (units.Contains(unit) && !_selected.Contains(unit) && !unit.IsDead)
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
            foreach (var unit in units)
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
                            Vector3 offset = Random.insideUnitCircle * (_selected.Count * .25f);
                            offset.z = offset.y;
                            offset.y = 0;
                            unit.AddCommand(new FollowCommand(view.Model, offset, true));
                        }
                    }
                    else
                    {
                        foreach (var unit in _selected)
                        {
                            unit.AddCommand(new FightCommand(view.Model, true));
                        }
                    }
                }
                else
                {
                    foreach (var unit in _selected)
                    {
                        unit.AddCommand(new MoveCommand(hit.point, Vector3.zero, GameContext.Instance.Constants.MovingStopDistance, true));
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
                if (view != null && units.Contains(view.Model))
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
                            Vector3 offset = Random.insideUnitCircle * (_selected.Count * .25f);
                            offset.z = offset.y;
                            offset.y = 0;
                            unit.AddCommand(new FollowCommand(view.Model, offset,true));
                        }
                    }
                    else
                    {
                        // lr.material.color  = Color.red;
                        foreach (var unit in _selected)
                        {
                            unit.ClearCommands();
                            unit.AddCommand(new FightCommand(view.Model,true));
                        }
                    }
                }
                else
                {
                    // lr.material.color  = Color.green;
                    foreach (var unit in _selected)
                    {
                        unit.ClearCommands();
                        unit.AddCommand(new MoveCommand(hit.point, Vector3.zero, GameContext.Instance.Constants.MovingStopDistance, true));
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
                if (view != null && units.Contains(view.Model))
                    SelectUnit(view.Model);
            }
        }

        private void BoxSelect(Vector2 pos, Vector2 size)
        {
            DeselectAll();
            foreach (var unit in units)
            {
                Vector3 screen = Camera.main.WorldToScreenPoint(unit.Position);
                if (screen.x > pos.x && screen.y > pos.y && screen.x < pos.x + size.x && screen.y < pos.y + size.y)
                    SelectUnit(unit);
            }
        }

        private void ShiftBoxSelect(Vector2 pos, Vector2 size)
        {
            foreach (var unit in units)
            {
                Vector3 screen = Camera.main.WorldToScreenPoint(unit.Position);
                if (screen.x > pos.x && screen.y > pos.y && screen.x < pos.x + size.x && screen.y < pos.y + size.y)
                    SelectUnit(unit);
            }
        }
    }
}

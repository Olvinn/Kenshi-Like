using System;
using System.Collections.Generic;
using Data;
using Inputs;
using Units;
using Units.Commands.ComplexCommands;
using Units.Commands.SimpleCommands;
using Units.Views;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Players
{
    public class PlayerController : MonoBehaviour, IPlayer
    {
        [SerializeField] private LayerMask workWithMask;

        public Action<Unit> onSelect, onDeselect;
        public Action onClearSelection;
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
            InputController.Instance.OnRMB += RightMouseButtonClick;
            InputController.Instance.OnLMB += LeftMouseButtonClick;
            InputController.Instance.OnBoxSelect += BoxSelect;
        }

        public bool IsUnitSelected(Unit unit)
        {
            return _selected.Contains(unit);
        }

        public void AddUnit(Unit unit)
        {
            units.Add(unit);
            unit.SetTeam(_team);
        }

        public void SelectUnit(Unit unit, bool additive)
        {
            if (!additive)
                DeselectAll();
            
            if (units.Contains(unit) && !unit.IsDead)
            {
                unit.View.Select();
                _selected.Add(unit);
                onSelect?.Invoke(unit);
            }
        }

        public void DeselectUnit(Unit unit)
        {
            if (_selected.Contains(unit))
            {
                unit.View.Deselect();
                _selected.Remove(unit);
                onDeselect?.Invoke(unit);
            }
        }

        public void DeselectAll()
        {
            foreach (var unit in _selected)
                unit.View.Deselect();

            _selected.Clear();
            onClearSelection?.Invoke();
        }

        public void SetTeam(TeamEnum team)
        {
            _team = team;
            foreach (var unit in units)
            {
                unit.SetTeam(team);
            }
        }

        private void RightMouseButtonClick(Ray ray)
        {
            if (InputController.Instance.isAdditiveModifierApplied)
            {
                ShiftRightMouseButtonClick(ray);
                return;
            }
            
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit,1000f,  workWithMask.value))
            {
                var view = hit.collider.GetComponent<UnitView>();
                if (view != null && !view.Model.IsDead)
                {
                    if (view.Model.Team == TeamEnum.Player)
                    {
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
                        foreach (var unit in _selected)
                        {
                            unit.ClearCommands();
                            unit.AddCommand(new FightCommand(view.Model,true));
                        }
                    }
                }
                else
                {
                    foreach (var unit in _selected)
                    {
                        unit.ClearCommands();
                        unit.AddCommand(new MoveCommand(hit.point, Vector3.zero, GameContext.Instance.Constants.MovingStopDistance, true));
                    }
                }
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

        private void LeftMouseButtonClick(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, workWithMask.value))
            {
                var view = hit.collider.GetComponent<UnitView>();
                if (view != null && units.Contains(view.Model))
                    SelectUnit(view.Model, InputController.Instance.isAdditiveModifierApplied);
            }
        }

        private void BoxSelect(Vector2 pos, Vector2 size)
        {
            if (InputController.Instance.isAdditiveModifierApplied)
            {
                ShiftBoxSelect(pos, size);
                return;
            }
            
            DeselectAll();
            foreach (var unit in units)
            {
                Vector3 screen = Camera.main.WorldToScreenPoint(unit.Position);
                if (screen.x > pos.x && screen.y > pos.y && screen.x < pos.x + size.x && screen.y < pos.y + size.y)
                    SelectUnit(unit, true);
            }
        }

        private void ShiftBoxSelect(Vector2 pos, Vector2 size)
        {
            foreach (var unit in units)
            {
                Vector3 screen = Camera.main.WorldToScreenPoint(unit.Position);
                if (screen.x > pos.x && screen.y > pos.y && screen.x < pos.x + size.x && screen.y < pos.y + size.y)
                    SelectUnit(unit, true);
            }
        }
    }
}

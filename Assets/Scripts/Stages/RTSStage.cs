using Inputs;
using Players;
using UI;
using Units;
using UnityEngine;

namespace Stages
{
    public class RTSStage : Stage
    {
        [SerializeField] private SquadView squadsView;

        [SerializeField] private PlayerController player;

        public override void Open()
        {
            base.Open();
            
            squadsView.Show();
            squadsView.AddUnits(player.units);
            squadsView.onClicked += OnUnitPortraitClicked;
            player.onSelect += squadsView.SelectUnit;
            player.onDeselect += squadsView.DeselectUnit;
            player.onClearSelection += squadsView.DeselectAll;
            foreach (var unit in player.units)
                unit.OnDie += OnUnitDie;
        }

        public override void Close()
        {
            base.Close();
            
            squadsView.Hide();
            squadsView.ClearUnits();
            squadsView.onClicked -= OnUnitPortraitClicked;
            player.onSelect -= squadsView.SelectUnit;
            player.onDeselect -= squadsView.DeselectUnit;
            player.onClearSelection -= squadsView.DeselectAll;
            foreach (var unit in player.units)
                unit.OnDie -= OnUnitDie;
        }

        private void OnUnitDie(Unit unit)
        {
            squadsView.RemoveUnit(unit);
        }

        private void OnUnitPortraitClicked(Unit unit)
        {
            if (player.IsUnitSelected(unit) && InputController.Instance.isAdditiveModifierApplied)
                player.DeselectUnit(unit);
            else
                player.SelectUnit(unit, InputController.Instance.isAdditiveModifierApplied);
        }
    }
}

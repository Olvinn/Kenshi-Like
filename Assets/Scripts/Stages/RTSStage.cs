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
            foreach (var unit in player.units)
                unit.OnDie += OnUnitDie;
        }

        public override void Close()
        {
            base.Close();
            
            squadsView.Hide();
            squadsView.ClearUnits();
            foreach (var unit in player.units)
                unit.OnDie -= OnUnitDie;
        }

        public void OnUnitDie(Unit unit)
        {
            squadsView.RemoveUnit(unit);
        }
    }
}

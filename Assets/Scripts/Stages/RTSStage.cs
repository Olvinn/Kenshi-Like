using Players;
using UI;
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
        }

        public override void Close()
        {
            base.Close();
            
            squadsView.Hide();
            squadsView.ClearUnits();
        }
    }
}

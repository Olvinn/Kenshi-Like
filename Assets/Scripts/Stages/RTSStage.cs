using Players;
using UI;
using UnityEngine;

namespace Stages
{
    public class RTSStage : Stage
    {
        [SerializeField] private Widget squadsView;

        [SerializeField] private PlayerController player;

        public override void Open()
        {
            base.Open();
            
            // squadsView.Show();
        }

        public override void Close()
        {
            base.Close();
            
            // squadsView.Hide();
        }
    }
}

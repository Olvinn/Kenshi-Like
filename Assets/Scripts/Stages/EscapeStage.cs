using System;
using Connections;
using Connections.Commands;
using UI;
using UnityEngine;

namespace Stages
{
    public class EscapeStage : Stage
    {
        public Action OnContinue;
        
        [SerializeField] private Widget escapeMenu;

        public override void Open()
        {
            base.Open();
            
            escapeMenu.Show();
            
            CommandDispatcher.Instance.Handle(new SetTimeScaleCommand() { timeScale = 0f });
        }

        public override void Close()
        {
            base.Close();
            
            escapeMenu.Hide();
            
            CommandDispatcher.Instance.Handle(new SetTimeScaleCommand() { timeScale = 1f });
        }

        public void Continue()
        {
            OnContinue?.Invoke();
        }

        public void Quit()
        {
            CommandDispatcher.Instance.Handle(new QuitGameCommand());
        }

        public void LoadMainMenu()
        {
            CommandDispatcher.Instance.Handle(new LoadSceneCommand() {isAsync = true, scene = 1});
        }
    }
}

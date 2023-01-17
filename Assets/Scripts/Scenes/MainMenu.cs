using Connections;
using Data;
using TMPro;
using Units.Views;
using UnityEngine;

namespace Scenes
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private UnitView mainMenuUnit;
        [SerializeField] private TextMeshProUGUI version;

        private void Start()
        {
            mainMenuUnit.SetAppearance(Appearance.GetRandomAppearance(GameContext.Instance.Colors));
            version.text = $"v. {Application.version}";
        }

        public void StartDemo()
        {
            CommandDispatcher.Instance.Handle(new LoadSceneCommand() {isAsync = true, scene = 2});
        }

        public void Exit()
        {
            CommandDispatcher.Instance.Handle(new QuitGameCommand());
        }
    }
}

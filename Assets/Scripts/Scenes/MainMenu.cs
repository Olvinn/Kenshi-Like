using System;
using Data;
using Units.Views;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private UnitView mainMenuUnit;

        private void Start()
        {
            mainMenuUnit.SetAppearance(Appearance.GetRandomAppearance(GameContext.Instance.Colors));
        }

        public void StartDemo()
        {
            SceneManager.LoadScene(2);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}

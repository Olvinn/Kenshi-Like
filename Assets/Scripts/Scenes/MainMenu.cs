using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes
{
    public class MainMenu : MonoBehaviour
    {
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

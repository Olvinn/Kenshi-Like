using Connections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        CommandDispatcher.Instance.RegisterHandler<LoadSceneCommand>(LoadScene);
        CommandDispatcher.Instance.RegisterHandler<QuitGameCommand>(QuitGame);
        CommandDispatcher.Instance.RegisterHandler<SetTimeScaleCommand>(SetTimeScale);
    }

    private void OnDisable()
    {
        CommandDispatcher.Instance.RemoveHandler<LoadSceneCommand>(LoadScene);
        CommandDispatcher.Instance.RemoveHandler<QuitGameCommand>(QuitGame);
        CommandDispatcher.Instance.RemoveHandler<SetTimeScaleCommand>(SetTimeScale);
    }

    void LoadScene(LoadSceneCommand data)
    {
        if (data.isAsync)
        {
            var j = SceneManager.LoadSceneAsync(data.scene);
            j.completed += (x) => OnSceneLoaded();
        }
        else
        {
            SceneManager.LoadScene(data.scene);
            OnSceneLoaded();
        }
    }

    void OnSceneLoaded()
    {
        LightProbes.TetrahedralizeAsync();
        SetTimeScale(new SetTimeScaleCommand() { timeScale = 1 });
    }

    void QuitGame(QuitGameCommand data)
    {
        Application.Quit();
    }

    void SetTimeScale(SetTimeScaleCommand data)
    {
        Time.timeScale = data.timeScale;
    }
}

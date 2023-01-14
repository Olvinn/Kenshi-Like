using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    [SerializeField] private List<GameObject> permanent;

    private void Start()
    {
        GameObject parent = new GameObject("--- Permanent ---");
        DontDestroyOnLoad(parent);
        foreach (var go in permanent)
        {
            go.transform.SetParent(parent.transform);
            DontDestroyOnLoad(go);
        }
        
        SceneManager.LoadSceneAsync(1);
    }
}

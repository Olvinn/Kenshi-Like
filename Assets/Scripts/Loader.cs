using System.Collections.Generic;
using Connections;
using Connections.Commands;
using UnityEngine;

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
        
        CommandDispatcher.Instance.Handle(new LoadSceneCommand() { isAsync = true, scene = 1});
    }
}

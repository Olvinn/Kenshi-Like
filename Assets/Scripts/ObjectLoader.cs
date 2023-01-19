using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class ObjectLoader : MonoBehaviour
{
    public static ObjectLoader Instance { get; private set; }

    [Header("UI"), SerializeField] private Portrait portraitPrefab;

    private Dictionary<ObjectTypes, Pool> _pools;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _pools = new Dictionary<ObjectTypes, Pool>();

        var poolsGO = new GameObject("--- Pools ---");
        DontDestroyOnLoad(poolsGO);
        
        var temp = new GameObject("Portraits");
        temp.transform.SetParent(poolsGO.transform);
        _pools.Add(ObjectTypes.UIPortrait, new Pool(new List<IPoolable>() { portraitPrefab }, temp.transform, maxCount: 32));
    }

    public IPoolable GetObject(ObjectTypes objType)
    {
        return _pools[objType].GetObject();
    }
}

public enum ObjectTypes
{
    UIPortrait
}

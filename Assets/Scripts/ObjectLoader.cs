using System.Collections.Generic;
using OldUnits.Views;
using UI;
using UnityEngine;

public class ObjectLoader : MonoBehaviour
{
    public static ObjectLoader Instance { get; private set; }

    [Header("UI"), SerializeField] private Portrait portraitPrefab;
    [Header("Units"), SerializeField] private OldUnits.Views.UnitView unitView;

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
        
        CreatePool(ObjectTypes.UIPortrait, portraitPrefab, 32, poolsGO.transform);
        CreatePool(ObjectTypes.UnitView, unitView, 1000, poolsGO.transform);
    }

    public IPoolable GetObject(ObjectTypes objType)
    {
        return _pools[objType].GetObject();
    }

    private void CreatePool(ObjectTypes type, IPoolable prefab, int maxCount, Transform parent)
    {
        var temp = new GameObject(type.ToString());
        temp.transform.SetParent(parent);
        _pools.Add(type, new Pool(new List<IPoolable>() { prefab }, temp.transform, maxCount: maxCount));
    }
}

public enum ObjectTypes
{
    UIPortrait,
    UnitView
}

using System;
using System.Collections.Generic;
using Connections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pool
{
    private Queue<IPoolable> _pool;
    private LinkedList<IPoolable> _workers;
    private Transform _parent;
    private List<IPoolable> _prefabs;
    private int _maxCount;

    public Pool(List<IPoolable> prefabs, Transform parent, int initPoolSize = 0, int maxCount = 1000)
    {
        _prefabs = prefabs ?? throw new ArgumentException("Pool: prefabs list cannot be null");
        _pool = new Queue<IPoolable>();
        _workers = new LinkedList<IPoolable>();
        _maxCount = maxCount;
        _parent = parent;
        initPoolSize = Mathf.Clamp(initPoolSize, 0, _maxCount);

        for (int i = 0; i < initPoolSize; i++)
        {
            var obj = GameObject.Instantiate(_prefabs[i % _prefabs.Count].gameObject).GetComponent<IPoolable>();
            obj.gameObject.transform.SetParent(_parent);
            _pool.Enqueue(obj);
        }

        CommandDispatcher.Instance.RegisterHandler<ClearCache>(Clear);
    }

    public IPoolable GetObject()
    {
        if (_workers.Count >= _maxCount)
            return null;
        IPoolable obj;
        if (_pool.Count == 0)
        {
            obj = GameObject.Instantiate(_prefabs[Random.Range(0, _prefabs.Count) % _prefabs.Count].gameObject).GetComponent<IPoolable>();
            obj.gameObject.transform.SetParent(_parent);
            _pool.Enqueue(obj);
        }
        obj = _pool.Dequeue();
        obj.onUnload += PullObject;
        obj.Load();
        _workers.AddLast(obj);
        return obj;
    }

    public void PullObject(IPoolable obj)
    {
        obj.onUnload -= PullObject;
        obj.gameObject.transform.SetParent(_parent);
        _workers.Remove(obj);
        _pool.Enqueue(obj);
    }

    private void Clear(ClearCache command)
    {
        while (_workers.Count > 0)
        {
            var t = _workers.First.Value;
            _workers.RemoveFirst();
            GameObject.Destroy(t.gameObject);
        }

        while (_pool.Count > 0)
        {
            GameObject.Destroy(_pool.Dequeue().gameObject);
        }
    }
}

public interface IPoolable
{
    GameObject gameObject { get; }
    
    event Action<IPoolable> onUnload;

    void Unload();

    void Load();
}

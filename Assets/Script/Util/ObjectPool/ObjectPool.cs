using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private T _prefab;
    private Queue<T> _pool = new Queue<T>();
    private Transform _parent;

    public ObjectPool(T prefab, int initialSize = 10, Transform parent = null)
    {
        this._prefab = prefab;
        this._parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            AddObjectToPool();
        }
    }

    private T AddObjectToPool()
    {
        T obj = Object.Instantiate(_prefab, _parent);
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
        return obj;
    }

    public T Get()
    {
        if (_pool.Count == 0)
        {
            AddObjectToPool();
        }

        T obj = _pool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }
}
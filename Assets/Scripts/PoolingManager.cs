using UnityEngine;
using System;
using System.Collections.Generic;

public class PoolingManager : MonoBehaviour
{

    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

    void Awake()
    {
        ServiceLocator.Instance.Register(typeof(PoolingManager), gameObject);
    }

    void OnDestroy()
    {
        ServiceLocator.Instance.Unregister(typeof(PoolingManager));
    }

    public GameObject SpawnObject(string objectName, GameObject prefab)
    {
        if (!pools.ContainsKey(objectName))
        {
            ObjectPool pool = new ObjectPool(prefab);
            pools.Add(objectName, pool);
        }

        GameObject obj = pools[objectName].Spawn();

        return obj;
    }
}

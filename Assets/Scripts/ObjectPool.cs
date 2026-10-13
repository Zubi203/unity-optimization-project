using UnityEngine;
using System;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    private GameObject prefab;

    private List<GameObject> cachedObjects = new List<GameObject>();

    public ObjectPool(GameObject obj)
    {
        prefab = obj;
    }

    private GameObject CreateNew()
    {
        GameObject obj = Instantiate(prefab);
        cachedObjects.Add(obj);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Spawn()
    {
        GameObject obj = cachedObjects.Find(x => x.activeSelf == false);

        if (obj == null)
        {
            obj = CreateNew();
        }

        obj.SetActive(true);

        return obj;
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;

[DefaultExecutionOrder(-1)]
public class ServiceLocator : Singleton<ServiceLocator>
{
    private Dictionary<Type, GameObject> services = new Dictionary<Type, GameObject>();

    public override void Awake()
    {
        services.Clear();
        base.Awake();
    }

    public void Register(Type type, GameObject obj)
    {
        if (!services.ContainsKey(type))
        {
            services.Add(type, obj);
        }
        else
        {
            services[type] = obj;
        }
    }

    public void Unregister(Type type)
    {
        if (services.ContainsKey(type))
        {
            services.Remove(type);
        }
    }

    public GameObject GetService(Type type)
    {
        if(services.ContainsKey(type))
        {
            if(services[type] != null)
            {
                return services[type];
            }
            
            Debug.LogError("Service " + nameof(type) + " has not been registered!");
            return null;
        }

        Debug.LogError("Service " + nameof(type) + " not found!");
        return null;

        
    }

    public bool TryGetService(Type type)
    {
        if (services.ContainsKey(type) && services[type] != null)
        {
            return true;
        }

        return false;
    }

}

using UnityEngine;

public class EntityTracker : MonoBehaviour
{
    private int entityCount = 0;

    void Awake()
    {
        ServiceLocator.Instance.Register(typeof(EntityTracker), gameObject);
    }

    void OnDestroy()
    {
        ServiceLocator.Instance.Unregister(typeof(EntityTracker));
    }

    public void IncrementEntities()
    {
        entityCount += 1;
    }

    public void DecrementEntities()
    {
        if (entityCount > 0)
        {
            entityCount -= 1;
        }
    }

    public int GetEntityCount()
    {
        return entityCount;
    }
}

using UnityEngine;

public class Entity : MonoBehaviour
{
    private EntityTracker entityTracker
    {
        get
        {
            return ServiceLocator.Instance.GetService(typeof(EntityTracker)).GetComponent<EntityTracker>();
        }

    }

    void OnEnable()
    {
        entityTracker.IncrementEntities();
        OnEnableLogic();
    }

    void OnDisable()
    {
        entityTracker.DecrementEntities();
        OnDisableLogic();
    }

    public virtual void OnEnableLogic()
    {

    }

    public virtual void OnDisableLogic()
    {
        
    }
}

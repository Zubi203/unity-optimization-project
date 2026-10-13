using UnityEngine;
using System.Collections;

public class Projectile : Entity
{
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 4f;
    private bool isPooled = false;

    private Rigidbody rb;
    private TrailRenderer trail;

    ToolkitManager toolkitManager
    {
        get
        {
            return ServiceLocator.Instance.GetService(typeof(ToolkitManager)).GetComponent<ToolkitManager>();
        }
    }

    PoolingManager poolingManager
    {
        get
        {
            return ServiceLocator.Instance.GetService(typeof(PoolingManager)).GetComponent<PoolingManager>();
        }
    }

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody>();
    }

    public void Setup(Vector2 dir, bool pooled)
    {
        if (rb == null){return;}
        Vector3 movementVector = new Vector3(dir.x, 0f, dir.y);
        movementVector.Normalize();
        rb.linearVelocity = movementVector * moveSpeed;
        isPooled = pooled;
        StartCoroutine(TryDestroyObject());
    }

    IEnumerator TryDestroyObject()
    {
        yield return new WaitForSeconds(lifeTime);

        DestroyObject();
    }

    void DestroyObject()
    {

        if (destroyEffect != null)
        {
            if (toolkitManager.objectPooling)
            {
                GameObject effect = poolingManager.SpawnObject("projectileEffect", destroyEffect);
                effect.transform.position = transform.position;
                effect.transform.rotation = Quaternion.identity;

                ParticleSystem particles = effect.GetComponent<ParticleSystem>();
                var mainModule = particles.main;
                mainModule.stopAction = ParticleSystemStopAction.Disable;
            }
            else
            {
                GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);

                ParticleSystem particles = effect.GetComponent<ParticleSystem>();
                var mainModule = particles.main;
                mainModule.stopAction = ParticleSystemStopAction.Destroy;
            }
        }

        if(isPooled)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.TryGetComponent<Health>(out Health healthComponent))
        {
            healthComponent.TakeDamage(damage);
        }
    
        DestroyObject();
    }

    public override void OnEnableLogic()
    {
        StartCoroutine(EnableTrail());
    }

    IEnumerator EnableTrail()
    {
        yield return new WaitForSeconds(0.01f);

        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }
    }

    public override void OnDisableLogic()
    {
        if (trail != null)
        {
            trail.emitting = false;
            trail.Clear();
        }
    }
}

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

    void Awake()
    {
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
            GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
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
}

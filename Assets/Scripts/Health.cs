using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private GameObject destroyEffect;

    public bool isPooled = false;
    public event Action<Enemy> healthDepleted;

    private int _health;
    private int currentHealth
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            _health = Mathf.Clamp(_health, 0, maxHealth);
            if (_health <= 0)
            {
                Die();
            }
        }
    }

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
    
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
    }

    private void Die()
    {
        if (TryGetComponent<Enemy>(out Enemy enemy))
        {
            healthDepleted?.Invoke(enemy);
        }
        DestroyEffect();
        if(isPooled)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void DestroyEffect()
    {
        if (destroyEffect == null){return;}

        if (toolkitManager.objectPooling)
        {
            GameObject effect = poolingManager.SpawnObject("enemyDestroy", destroyEffect);
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
}

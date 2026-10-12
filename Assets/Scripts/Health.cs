using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;

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
                Die(false);
            }
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

    private void Die(bool isPooled)
    {
        if(isPooled)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Entity
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float primaryShootRate = 0.3f;
    private float primaryShootTimer = 0.0f;
    [SerializeField] private float secondaryShootRate = 0.8f;
    private float secondaryShootTimer = 0.0f;
    [SerializeField] private int primaryBulletsPerShot = 8;
    [SerializeField] private int secondaryBulletsPerShot = 8;

    private Rigidbody rb;
    private Vector2 moveDir = Vector2.zero;
    private Vector2 shootDir = Vector2.one;
    [SerializeField] private Transform playerModel;
    [SerializeField] private Transform muzzle;

    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector2 movementVector = moveDir.normalized * moveSpeed;
        rb.linearVelocity = new Vector3(movementVector.x, rb.linearVelocity.y, movementVector.y);
        TryShootPrimary();
        TryShootSecondary();
        
        float angle = Mathf.Atan2(shootDir.x, shootDir.y) * Mathf.Rad2Deg;
        playerModel.transform.eulerAngles = Vector3.up * angle;
    }

    
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();

        if(moveDir != Vector2.zero)
        {
            shootDir = moveDir;
        }
    }

    void TryShootSecondary()
    {
        if (projectilePrefab == null){return;}

        if (secondaryShootTimer > 0.0f)
        {
            secondaryShootTimer -= Time.deltaTime;
            return;
        }
        secondaryShootTimer = secondaryShootRate;
        

        for (int i = 0; i < secondaryBulletsPerShot; i++)
        {
            Vector3 randomDir = new Vector3(shootDir.x, 0f, shootDir.y);
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f);
            randomDir = randomRotation * randomDir;
            SpawnBullet(new Vector2(randomDir.x, randomDir.z));
        }
        
    }

    void TryShootPrimary()
    {
        if (projectilePrefab == null){return;}

        if (primaryShootTimer > 0.0f)
        {
            primaryShootTimer -= Time.deltaTime;
            return;
        }
        primaryShootTimer = primaryShootRate;
        

        for (int i = 0; i < primaryBulletsPerShot; i++)
        {
            Vector3 randomDir = new Vector3(shootDir.x, 0f, shootDir.y);
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(-20f, 20f), 0f);
            randomDir = randomRotation * randomDir;
            SpawnBullet(new Vector2(randomDir.x, randomDir.z));
        }
        
    }

    void SpawnBullet(Vector2 dir)
    {
        if(muzzle == null){return;}
        GameObject proj = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Setup(dir, false);
    }
}

using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;

public class Enemy : Entity
{
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 6f;
    [SerializeField] private float pathPointReachedDistance = 0.5f;

    private int framesPerUpdate = 10;
    private int frameCounter = 0;

    public List<Vector3> path;
    private GameObject target;
    private Rigidbody rb;
    Vector2 moveDir = Vector2.zero;
    Vector2 lookDir;
    Vector3 targetEulerAngles;

    EnemyManager enemyManager
    {
        get
        {
            if (ServiceLocator.Instance.TryGetService(typeof(EnemyManager)))
            {
                return ServiceLocator.Instance.GetService(typeof(EnemyManager)).GetComponent<EnemyManager>();
            }

            return null;
        }
    }

    ToolkitManager toolkitManager
    {
        get
        {
            return ServiceLocator.Instance.GetService(typeof(ToolkitManager)).GetComponent<ToolkitManager>();
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        target = enemyManager.GetPlayer();
        InvokeRepeating(nameof(UpdatePath), 0.0f, 0.5f);
    }

    void FixedUpdate()
    {
        if (toolkitManager.reducedUpdateRate)
        {
            if(frameCounter > 0)
            {
                frameCounter -= 1;
                return;
            }
            frameCounter = framesPerUpdate;
        }

        if(rb == null || target == null){return;}
        if(Vector3.Distance(target.transform.position, transform.position) <= stoppingDistance){return;}

        MoveToNextPathPoint();
        MoveEnemy();

        if (animator != null)
        {
            animator.SetBool("IsMoving", rb.linearVelocity.magnitude > 0.0);
        }
    }

    void MoveEnemy()
    {
        rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.y) * moveSpeed;
        transform.eulerAngles = targetEulerAngles;
    }

    void MoveToNextPathPoint()
    {
        if (!toolkitManager.simplifiedPathing)
        {
            if (path.Count <= 0){return;}

            Vector3 dir = (path[0] - transform.position);
            moveDir = new Vector2(dir.x, dir.z);
            moveDir.Normalize();

            if (Vector2.Distance(new Vector2(path[0].x, path[0].z), new Vector2(transform.position.x, transform.position.z)) < pathPointReachedDistance)
            {
                path.RemoveAt(0);
            }
        }
        else
        {
            if (target == null){return;}
            Vector3 dir = (target.transform.position - transform.position);
            moveDir = new Vector2(dir.x, dir.z);
            moveDir.Normalize();
        }

        if(moveDir != Vector2.zero)
        {
            lookDir = moveDir;
        }
        float angle = Mathf.Atan2(lookDir.x, lookDir.y) * Mathf.Rad2Deg;
        targetEulerAngles = new Vector3(0f, angle, 0f);
    }

    void UpdatePath()
    {
        if (!toolkitManager.simplifiedPathing)
        {
            if (gameObject.activeSelf == false){return;}
            if (target == null){return;}

            NavMeshPath navMeshPath = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, target.transform.position, NavMesh.AllAreas, navMeshPath);

            path = navMeshPath.corners.ToList();
        }
    }

    
}

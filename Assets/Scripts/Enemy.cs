using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;

public class Enemy : Entity
{
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 6f;
    [SerializeField] private float pathPointReachedDistance = 0.5f;

    private List<Vector3> path;
    private GameObject target;
    private Rigidbody rb;
    Vector2 moveDir = Vector2.zero;
    Vector2 lookDir;

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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        target = enemyManager.GetPlayer();
        InvokeRepeating(nameof(UpdatePath), 0.0f, 0.5f);
    }

    void Update()
    {
        if(rb == null || target == null){return;}
        if(Vector3.Distance(target.transform.position, transform.position) <= stoppingDistance){return;}

        MoveToNextPathPoint();
        if(moveDir != Vector2.zero)
        {
            lookDir = moveDir;
        }
        rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.y) * moveSpeed;

        float angle = Mathf.Atan2(lookDir.x, lookDir.y) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.up * angle;
    }

    void MoveToNextPathPoint()
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

    void UpdatePath()
    {
        if (target == null){return;}

        NavMeshPath navMeshPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, target.transform.position, NavMesh.AllAreas, navMeshPath);

        path = navMeshPath.corners.ToList();
    }

    public override void OnEnableLogic()
    {
        if (spawnEffect == null){return;}

        GameObject effect = Instantiate(spawnEffect, transform.position, Quaternion.identity);
    }

    public override void OnDisableLogic()
    {
        if (destroyEffect == null){return;}

        GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);//
    }
}

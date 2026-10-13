using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;


public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private float spawnInterval = 0.5f;
    private float spawnIntervalTimer = 0.0f;
    [SerializeField] private int enemiesPerSpawnCall = 10;
    [SerializeField] private float minSpawnDistanceFromPlayer = 20f;
    [SerializeField] private float maxSpawnDistanceFromPlayer = 60f;

    private int framesPerUpdate = 10;
    private int frameCounter = 0;

    private int entityLimit = 1000;
    private List<Enemy> enemyList = new List<Enemy>();


    private GameObject player = null;

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

    EntityTracker entityTracker
    {
        get
        {
            return ServiceLocator.Instance.GetService(typeof(EntityTracker)).GetComponent<EntityTracker>();
        }
    }

    void Awake()
    {
        ServiceLocator.Instance.Register(typeof(EnemyManager), gameObject);
    }

    void OnDestroy()
    {
        if (ServiceLocator.Instance)
        {
            ServiceLocator.Instance.Unregister(typeof(EnemyManager));
        }
    }

    
    void Start()
    {
        player = FindAnyObjectByType<PlayerController>().gameObject;
    }

    void FixedUpdate()
    {
        TrySpawnEnemies();

        if (toolkitManager.reducedUpdateRate)
        {
            if(frameCounter > 0)
            {
                frameCounter -= 1;
                return;
            }
            frameCounter = framesPerUpdate;
        }

        if (toolkitManager.centralizedUpdateLogic)
        {
            MoveEnemies();
        } 
    }

    void MoveEnemies()
    {
        if(!toolkitManager.burstCompiling)
        {
            foreach (Enemy enemy in enemyList)
            {
                enemy.MoveToNextPathPoint();
                enemy.MoveEnemy();
            }
        }
        else
        {
            if (toolkitManager.simplifiedPathing)
            {
                NativeArray<float3> rbLinearVelocityArray = new NativeArray<float3>(enemyList.Count, Allocator.TempJob);
                NativeArray<float3> eulerTransformArray = new NativeArray<float3>(enemyList.Count, Allocator.TempJob);
                NativeArray<float3> currPositionArray = new NativeArray<float3>(enemyList.Count, Allocator.TempJob);

                for (int i = 0; i <  enemyList.Count; i++)
                {
                    rbLinearVelocityArray[i] = enemyList[i].rb.linearVelocity;
                    eulerTransformArray[i] = enemyList[i].transform.eulerAngles;
                    currPositionArray[i] = enemyList[i].transform.position;
                }

                MoveEnemySimplifiedJob moveEnemyJob = new MoveEnemySimplifiedJob{
                    velocityArray = rbLinearVelocityArray,
                    eulerAnglesArray = eulerTransformArray,
                    targetPosition = player.transform.position,
                    currentPositionArray = currPositionArray,
                    moveSpeed = enemyList[0].moveSpeed,
                };

                JobHandle jobHandle = moveEnemyJob.Schedule(enemyList.Count, 200);
                jobHandle.Complete();

                for (int i = 0; i <  enemyList.Count; i++)
                {
                    enemyList[i].rb.linearVelocity = rbLinearVelocityArray[i];
                    enemyList[i].transform.eulerAngles = eulerTransformArray[i];
                }
            }
            else
            {
                NativeArray<float3> rbLinearVelocityArray = new NativeArray<float3>(enemyList.Count, Allocator.TempJob);
                NativeArray<float3> eulerTransformArray = new NativeArray<float3>(enemyList.Count, Allocator.TempJob);
                NativeArray<float3> currPosArray = new NativeArray<float3>(enemyList.Count, Allocator.TempJob);
                NativeArray<float3> targetPosArray = new NativeArray<float3>(enemyList.Count, Allocator.TempJob);
                NativeArray<float> targetDistArray = new NativeArray<float>(enemyList.Count, Allocator.TempJob);


                for (int i = 0; i <  enemyList.Count; i++)
                {
                    if(enemyList[i].path.Count == 0)
                    {
                        continue;
                    }
                    targetPosArray[i] = enemyList[i].path[0];
                    rbLinearVelocityArray[i] = enemyList[i].rb.linearVelocity;
                    eulerTransformArray[i] = enemyList[i].transform.eulerAngles;
                    currPosArray[i] = enemyList[i].transform.position;
                    targetDistArray[i] = 0f;
                }

                MoveEnemyNavMeshJob moveEnemyJob = new MoveEnemyNavMeshJob{
                    velocityArray = rbLinearVelocityArray,
                    eulerAnglesArray = eulerTransformArray,
                    targetPositionArray = targetPosArray,
                    currentPositionArray = currPosArray,
                    distanceFromTargetArray = targetDistArray,
                    moveSpeed = enemyList[0].moveSpeed,
                };

                JobHandle jobHandle = moveEnemyJob.Schedule(enemyList.Count, 200);
                jobHandle.Complete();

                for (int i = 0; i <  enemyList.Count; i++)
                {
                    enemyList[i].rb.linearVelocity = rbLinearVelocityArray[i];
                    enemyList[i].transform.eulerAngles = eulerTransformArray[i];

                    if(enemyList[i].path.Count == 0)
                    {
                        continue;
                    }

                    if(targetDistArray[i] < enemyList[i].pathPointReachedDistance)
                    {
                        enemyList[i].path.RemoveAt(0);
                    }
                }
            }
        }
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    private void TrySpawnEnemies()
    {
        if (player == null || enemyPrefab == null) {return;}

        if (spawnIntervalTimer > 0.0f)
        {
            spawnIntervalTimer -= Time.deltaTime;
            return;
        }
        spawnIntervalTimer = spawnInterval;

        if (entityTracker.GetEntityCount() > entityLimit)
        {
            return;
        }

        
        for(int i = 0; i < enemiesPerSpawnCall; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPoint(player.transform.position, maxSpawnDistanceFromPlayer, minSpawnDistanceFromPlayer);
            if (toolkitManager.objectPooling)
            {
                GameObject enemy = poolingManager.SpawnObject("enemy", enemyPrefab);
                enemy.transform.position = spawnPos;
                enemy.transform.rotation = Quaternion.identity;
                enemyList.Add(enemy.GetComponent<Enemy>());
                if (enemy.TryGetComponent<Health>(out Health health))
                {
                    health.healthDepleted += RemoveEnemyFromList;
                }

                if (enemy.TryGetComponent<Health>(out Health healthComponent))
                {
                    healthComponent.isPooled = true;
                }
            }
            else
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                enemyList.Add(enemy.GetComponent<Enemy>());
                
                if (enemy.TryGetComponent<Health>(out Health healthComponent))
                {
                    healthComponent.isPooled = false;
                }
            }
            

            SpawnEffect(spawnPos);
        }
    }

    private void SpawnEffect(Vector3 spawnPos)
    {
        if (spawnEffect == null){return;}

        if (toolkitManager.objectPooling)
        {
            GameObject effect = poolingManager.SpawnObject("enemySpawn", spawnEffect);
            effect.transform.position = spawnPos;
            effect.transform.rotation = Quaternion.identity;

            ParticleSystem particles = effect.GetComponent<ParticleSystem>();
            var mainModule = particles.main;
            mainModule.stopAction = ParticleSystemStopAction.Disable;
        }
        else
        {
            GameObject effect = Instantiate(spawnEffect, spawnPos, Quaternion.identity);
            ParticleSystem particles = effect.GetComponent<ParticleSystem>();
            var mainModule = particles.main;
            mainModule.stopAction = ParticleSystemStopAction.Destroy;
        }
    }

    private Vector3 GetRandomSpawnPoint(Vector3 originPos, float maxDistance, float minDistance)
    {
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * maxDistance + originPos;
        while(Vector3.Distance(randomDir, originPos) < minDistance)
        {
            randomDir = UnityEngine.Random.insideUnitSphere * maxDistance + originPos;
        }

        NavMeshHit hit;
        
        if (NavMesh.SamplePosition(randomDir, out hit, maxDistance, NavMesh.AllAreas))
        {
            return new Vector3(hit.position.x, hit.position.y + 1f, hit.position.z);
        }

        return originPos;
    }

    public void RemoveEnemyFromList(Enemy enemy)
    {
        if (enemy.TryGetComponent<Health>(out Health health))
        {
            health.healthDepleted -= RemoveEnemyFromList;
        }
        enemyList.Remove(enemy);
    }
}

[BurstCompile]
public struct MoveEnemySimplifiedJob: IJobParallelFor
{
    public NativeArray<float3> velocityArray;
    public NativeArray<float3> eulerAnglesArray;

    public float3 targetPosition;
    public NativeArray<float3> currentPositionArray;
    public float moveSpeed;

    public void Execute(int index)
    {
        float3 dir = math.normalizesafe((targetPosition - currentPositionArray[index]));
        velocityArray[index] = new float3(dir.x, velocityArray[index].y, dir.z) * moveSpeed;
        float angle = math.degrees(math.atan2(dir.x, dir.z));
        eulerAnglesArray[index] = new float3(0f, angle, 0f);
    }
}

[BurstCompile]
public struct MoveEnemyNavMeshJob: IJobParallelFor
{
    public NativeArray<float3> velocityArray;
    public NativeArray<float3> eulerAnglesArray;

    public NativeArray<float3> targetPositionArray;
    public NativeArray<float3> currentPositionArray;
    public NativeArray<float> distanceFromTargetArray;
    public float moveSpeed;

    public void Execute(int index)
    {
        float3 dir = math.normalizesafe((targetPositionArray[index] - currentPositionArray[index]));
        velocityArray[index] = new float3(dir.x, velocityArray[index].y, dir.z) * moveSpeed;
        float angle = math.degrees(math.atan2(dir.x, dir.z));
        eulerAnglesArray[index] = new float3(0f, angle, 0f);

        distanceFromTargetArray[index] = math.distance(targetPositionArray[index], currentPositionArray[index]);
    }
}

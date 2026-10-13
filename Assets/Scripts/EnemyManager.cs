using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;


public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private float spawnInterval = 0.5f;
    private float spawnIntervalTimer = 0.0f;
    [SerializeField] private int enemiesPerSpawnCall = 10;
    [SerializeField] private float minSpawnDistanceFromPlayer = 20f;
    [SerializeField] private float maxSpawnDistanceFromPlayer = 60f;

    private int entityLimit = 2000;
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

        if (entityTracker.GetEntityCount() >= entityLimit)
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
        Vector3 randomDir = Random.insideUnitSphere * maxDistance + originPos;
        while(Vector3.Distance(randomDir, originPos) < minDistance)
        {
            randomDir = Random.insideUnitSphere * maxDistance + originPos;
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

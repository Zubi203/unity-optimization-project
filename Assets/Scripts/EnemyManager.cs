using UnityEngine;
using UnityEngine.AI;


public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 0.5f;
    private float spawnIntervalTimer = 0.0f;
    [SerializeField] private int enemiesPerSpawnCall = 10;
    [SerializeField] private float minSpawnDistanceFromPlayer = 20f;
    [SerializeField] private float maxSpawnDistanceFromPlayer = 60f;


    private GameObject player = null;

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

    void Update()
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

        for(int i = 0; i < enemiesPerSpawnCall; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, GetRandomSpawnPoint(player.transform.position, maxSpawnDistanceFromPlayer, minSpawnDistanceFromPlayer), Quaternion.identity);
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
}

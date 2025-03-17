using UnityEngine;

[System.Serializable]
public struct EnemySpawnData
{
    [Tooltip("Enemy prefab to spawn")]
    public GameObject enemyPrefab;

    [Tooltip("Spawn chance/weight for this enemy type preset")]
    public float spawnChance;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("List of enemy prefabs and their spawn chances")]
    [SerializeField] private EnemySpawnData[] enemySpawnData;

    [Tooltip("Time between enemy spawns")]
    [SerializeField] private float spawnRate = 2f;

    [Tooltip("Enemies will spawn this distance away from the player's current position")]
    [SerializeField] private float spawnDistance = 10f;

    [Tooltip("If true, starts spawning automatically on start")]
    [SerializeField] private bool spawnOnStart = true;

    [Header("References")]
    [Tooltip("Reference to the player's transform")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("Reference to the player's PlayerMoney component")]
    [SerializeField] private PlayerMoney playerMoney;

    private float spawnTimer = 0f;
    private bool isSpawning = false;

    private void Start()
    {
        if (spawnOnStart)
        {
            isSpawning = true;
            spawnTimer = 0f;
        }
    }

    private void Update()
    {
        if (!isSpawning) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {

        // Calculate the total spawn weight
        float totalWeight = 0f;
        foreach (var enemyData in enemySpawnData)
        {
            totalWeight += enemyData.spawnChance;
        }

    
        // Generate a random value between 0 and totalWeight.
        float randomValue = Random.Range(0f, totalWeight);

        // Determine which enemy to spawn based on the weighted chances.
        GameObject selectedEnemy = null;
        foreach (var enemyData in enemySpawnData)
        {
            if (randomValue < enemyData.spawnChance)
            {
                selectedEnemy = enemyData.enemyPrefab;
                break;
            }
            randomValue -= enemyData.spawnChance;
        }


        // Determine spawn position relative to the player.
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = playerTransform.position + (Vector3)(randomDirection * spawnDistance);

        // Instantiate the selected enemy.
        GameObject enemy = Instantiate(selectedEnemy, spawnPosition, Quaternion.identity);

        // Assign the player's PlayerMoney reference to the HealthController on the new enemy.
        HealthController healthController = enemy.GetComponent<HealthController>();
        if (healthController != null)
        {
            healthController.playerMoney = playerMoney;
            Debug.Log("Assigned PlayerMoney to new enemy: " + enemy.name);
        }
        else
        {
            Debug.LogError("Enemy prefab missing HealthController!");
        }
    }

    // Public method to stop spawning at runtime.
    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void increaseSpawnRate() {
        spawnRate -= 1f;
        Debug.Log("Spawn rate: " + spawnRate);
    }
    
    public void increaseHealth() {
        Debug.Log("Current enemy health: ");
    }
}

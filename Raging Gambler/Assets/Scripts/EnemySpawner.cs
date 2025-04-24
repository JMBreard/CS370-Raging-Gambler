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

    [Tooltip("Enemy Health Adder (to be altered with wager)")]
    [SerializeField] public int enemyHealthIncreaser = 0;

    [Tooltip("Time between enemy spawns")]
    [SerializeField] public float spawnInterval = 2f;

    [Tooltip("Enemies will spawn this distance away from the player's current position")]
    [SerializeField] private float spawnDistance = 10f;

    [Tooltip("If true, starts spawning automatically on start")]
    [SerializeField] private bool spawnOnStart = true;

    [Header("References")]
    [Tooltip("Reference to the player's transform")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("Reference to the player's PlayerMoney component")]
    [SerializeField] private PlayerMoney playerMoney;
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private float distanceFromPlayer;

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
        if (spawnTimer >= spawnInterval)
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
        Vector3 spawnPosition = spawnLocation.position + (Vector3)(randomDirection * spawnDistance);
        while ((spawnPosition.x <= playerTransform.position.x + distanceFromPlayer && spawnPosition.x >= playerTransform.position.x - distanceFromPlayer) && (spawnPosition.y <= playerTransform.position.y + distanceFromPlayer && spawnPosition.y >= playerTransform.position.y - distanceFromPlayer))
        {
            Debug.Log("Too Close to player, Picking new location");
            randomDirection = Random.insideUnitCircle.normalized;
            spawnPosition = spawnLocation.position + (Vector3)(randomDirection * spawnDistance);
        }

        // Instantiate the selected enemy.
        GameObject enemy = Instantiate(selectedEnemy, spawnPosition, Quaternion.identity);

        // Apply health multiplier to the instaniated enemy's health controller
        HealthController hc = enemy.GetComponent<HealthController>();
        hc.maxHealth += enemyHealthIncreaser;
        hc.currentHealth = hc.maxHealth;


        // Assign the player's PlayerMoney reference to the HealthController on the new enemy.
        HealthController healthController = enemy.GetComponent<HealthController>();
        if (healthController != null)
        {
            healthController.playerMoney = playerMoney;
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

    public void increaseSpawnRate()
    {
        if (spawnInterval <= 0.5f)
        {
            GambleManager.instance.SetCanBuy("Enemy: population buff", false);
            Debug.Log("Spawn rate cannot be decreased further. Current spawn rate: " + spawnInterval);
            return;
        }

        spawnInterval -= 0.5f;
        // handles edge case of being able to buy an extra debuff when you can't anymore
        if (spawnInterval <= 0.5f)
        {
            GambleManager.instance.SetCanBuy("Enemy: population buff", false);
            Debug.Log("Spawn rate cannot be decreased further. Current spawn rate: " + spawnInterval);
            return;
        }
        Debug.Log("Current spawn rate: " + spawnInterval);

    }

    public void DecreaseSpawnRate()
    {
        spawnInterval += 0.5f;
        if (spawnInterval > 0.5f)
        {
            GambleManager.instance.SetCanBuy("Enemy: population buff", true);
        }
    }

    public void addEnemyHealth()
    {
        enemyHealthIncreaser += 1;
        Debug.Log("Current enemy health is increased by: " + enemyHealthIncreaser);
    }

    public void SubtractEnemyHealth()
    {
        enemyHealthIncreaser -= 1;
    }

    // getters for spawn rate and the enemy health increaser
    public float GetSpawnRate() => spawnInterval;
    public int GetEnemyHealthBuff() => enemyHealthIncreaser;
}

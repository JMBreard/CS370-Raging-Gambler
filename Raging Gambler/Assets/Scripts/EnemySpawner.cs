using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("Reference to the enemy prefab t spawn")]
    [SerializeField] private GameObject _enemyPrefab;

    [Tooltip("Time between enemy spawns")]
    [SerializeField] private float spawnRate = 2f;

    [Tooltip("Enemies will spawn this distance away from teh player's current position")]
    [SerializeField] private float _spawnDistance = 10f;

    [Tooltip("If true, starts spawning automatically on start")]
    [SerializeField] private bool _spawnOnStart = true;

    [Header("References")]
    [Tooltip("Reference to the player's transform")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("Reference to the player's PlayerMoney component")]
    [SerializeField] private PlayerMoney playerMoney;  // Reference to the player's PlayerMoney

    private float spawnTimer = 0f;
    private bool isSpawning = false;

    private void Start()
    {
        isSpawning = true;
        spawnTimer = 0f;
    }

    private void Update()
    {
        if (!isSpawning) return;

        // Increment the timer
        spawnTimer += Time.deltaTime;

        // Spawn enemy and reset timer when _spawnTimer > _spawnRate
        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    // Instantiates an enemy at a random position around the player, at _spawnDistance
    private void SpawnEnemy()
    {

        // Random direction around the player
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // spawnPosition = direction of random direction and distance of _spawnDistance
        Vector3 spawnPosition = playerTransform.position + (Vector3)(randomDirection * _spawnDistance);

        // Spawn the enemy
        GameObject enemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);

        // Ensure that the enemy has a HealthController component
        HealthController healthController = enemy.GetComponent<HealthController>();

        if (healthController != null)
        {
            // Assign the PlayerMoney reference to the HealthController on the new enemy
            healthController.playerMoney = playerMoney;  // Pass the reference to the player's PlayerMoney

            // Debug log to confirm assignment
            Debug.Log("Assigned PlayerMoney to new enemy: " + enemy.name);
        }
        else
        {
            Debug.LogError("Enemy prefab missing HealthController!");
        }
    }

    // Public method to begin spawning at runtime

    // Public method to stop spawning
    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void increaseSpawnRate() {
        spawnRate -= 1f;
        Debug.Log("Spawn rate: " + spawnRate);
    }
    
    public void increaseHealth() {
        
    }
}

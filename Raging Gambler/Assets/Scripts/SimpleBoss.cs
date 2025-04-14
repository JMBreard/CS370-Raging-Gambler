using UnityEngine;
using System.Collections;

public class SimpleBoss : MonoBehaviour, ProjectileMovement.IDamagable
{
    [Header("Boss Stats")]
    public int maxHealth = 500; // Increased from 150 to make boss much tougher
    private int currentHealth;
    public float moveSpeed = 1.5f; // Slower than normal enemies
    
    [Header("Minion Spawning")]
    public GameObject minionPrefab;
    public float minionSpawnInterval = 25f; // Increased from 10f to spawn minions less frequently
    public int minionsPerSpawn = 3;
    
    [Header("Rewards")]
    public int moneyRewardOnDeath = 200;
    
    // References
    private Transform playerTransform;
    private PlayerMoney playerMoney;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;
    
    // Simplified initialization approach
    private bool isInitialized = false;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindFirstObjectByType<GameManager>();
    }
    
    // Called externally by GameManager when spawning the boss
    public void SetProperlySpawned()
    {
        if (isInitialized) return;
        
        isInitialized = true;
        
        // Make the boss 3x bigger
        transform.localScale = new Vector3(3, 3, 3);
        
        // Find references
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerMoney = FindFirstObjectByType<PlayerMoney>();
        
        // Initialize health
        currentHealth = maxHealth;
        
        // Start minion spawning
        StartCoroutine(SpawnMinionsRoutine());
        
        // Make sure rigid body doesn't use gravity if it has one
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
        }
    }
    
    void Update()
    {
        // Only run logic if properly initialized
        if (!isInitialized || isDead) return;
        
        // Move directly towards player like regular enemies
        if (playerTransform != null)
        {
            // Simple direct movement toward player (like normal enemies)
            Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
    }
    
    // IDamagable implementation
    public int Health
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }
    
    public void Damage()
    {
        TakeDamage(1);
    }
    
    public void TakeDamage(int amount)
    {
        if (!isInitialized || isDead) return;
        
        currentHealth -= amount;
        
        // Flash red when taking damage
        StartCoroutine(FlashDamage());
        
        // Check death
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private IEnumerator FlashDamage()
    {
        if (spriteRenderer == null) yield break;
        
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Reward player
        if (playerMoney != null)
        {
            playerMoney.addMoney(moneyRewardOnDeath);
        }
        
        // Visual effect
        StartCoroutine(DeathEffect());
        
        // Notify GameManager with the specific boss defeated method
        if (gameManager != null)
        {
            gameManager.BossDefeated();
        }
    }
    
    private IEnumerator DeathEffect()
    {
        if (spriteRenderer == null) yield break;
        
        // Simple fade out effect
        for (float t = 1f; t > 0; t -= 0.05f)
        {
            if (spriteRenderer == null || !gameObject) yield break;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, t);
            yield return new WaitForSeconds(0.05f);
        }
        
        // Destroy after animation completes
        Destroy(gameObject);
    }
    
    private IEnumerator SpawnMinionsRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(minionSpawnInterval);
            
            if (isDead || !isInitialized) yield break;
            
            SpawnMinions();
        }
    }
    
    private void SpawnMinions()
    {
        if (!isInitialized || minionPrefab == null || !gameObject) return;
        
        for (int i = 0; i < minionsPerSpawn; i++)
        {
            // Random position around the boss
            Vector2 spawnOffset = Random.insideUnitCircle.normalized * 3f;
            Vector3 spawnPosition = transform.position + new Vector3(spawnOffset.x, spawnOffset.y, 0);
            
            // Instantiate minion
            GameObject minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
            
            // Set minion's player money reference if it has a health controller
            HealthController healthController = minion.GetComponent<HealthController>();
            if (healthController != null && playerMoney != null)
            {
                healthController.playerMoney = playerMoney;
                
                // Make minions die in 2 shots
                healthController.maxHealth = 2;
                healthController.currentHealth = 2;
            }
        }
    }
} 
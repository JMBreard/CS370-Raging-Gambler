using System.Collections;
using UnityEngine;

public class BossEnemy : MonoBehaviour, ProjectileMovement.IDamagable
{
    // Boss Stats
    public int maxHealth = 1000;
    private int currentHealth;
    public HealthBar healthBar;
    
    // Phase Settings
    public enum BossPhase { Phase1, Phase2, Phase3 }
    public BossPhase currentPhase = BossPhase.Phase1;
    public float phase2HealthThreshold = 0.6f; // 60% health
    public float phase3HealthThreshold = 0.3f; // 30% health
    public Color phase1Color = Color.red;
    public Color phase2Color = new Color(1f, 0.5f, 0f); // Orange
    public Color phase3Color = new Color(1f, 0f, 1f); // Purple
    
    // Attack Prefabs
    public GameObject basicProjectilePrefab;
    public GameObject spreadProjectilePrefab;
    public GameObject minionPrefab;

    // Attack Settings
    public float basicAttackCooldown = 2f;
    public float specialAttackCooldown = 5f;
    public int minionsPerSpawn = 2;
    public float projectileSpeed = 5f;
    public int aoeProjectileCount = 8; // Number of projectiles in AoE pattern
    public float aoeRadius = 2f; // Radius of AoE effect
    
    // Gambling Integration
    public int moneyRewardOnDeath = 200;
    public int moneyRewardPerPhase = 50;
    
    // References
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;
    private PlayerMoney playerMoney;
    private GameManager gameManager;
    
    // Movement settings
    public float moveSpeed = 1.5f;
    public float minDistanceFromPlayer = 2f;
    public float maxDistanceFromPlayer = 6f;
    
    // State tracking
    private bool isTransitioning = false;
    private bool isDead = false;
    private bool hasCompletedSpawnEffect = false;
    private bool properlySpawned = false; // Add this flag to control boss behavior

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindFirstObjectByType<GameManager>();
        
        // Disable physics/gravity on awake
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0; // Make sure gravity doesn't affect the boss
            rb.simulated = false; // Start with physics disabled
        }
        
        // Start with boss invisible for spawn effect
        if (spriteRenderer != null)
        {
            Color startColor = spriteRenderer.color;
            startColor.a = 0;
            spriteRenderer.color = startColor;
        }
        
        // IMPORTANT: Disable this boss if it was placed in the scene directly
        // Only bosses spawned by GameManager should be active
        if (gameManager == null || !IsProperBossLevel())
        {
            Debug.Log("Disabling scene-placed boss: Not a boss level or no GameManager found");
            DisableBoss();
        }
    }

    void Start()
    {
        // Log for debugging
        Debug.Log("BossEnemy Start method called");
        
        // Find references
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerMoney = FindFirstObjectByType<PlayerMoney>();
        
        // Initialize health and UI
        currentHealth = maxHealth;
        
        // Debug logs to track initialization
        if (playerTransform == null)
            Debug.LogWarning("Boss couldn't find player transform");
            
        // Start spawn effect
        StartCoroutine(SpawnEffect());
    }

    // Implement IDamagable interface
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
        if (isDead || isTransitioning) return;

        currentHealth -= amount;
        
        // Update health UI if available
        UpdateHealthBar();
        
        // Check phase transitions
        float healthPercentage = (float)currentHealth / maxHealth;
        
        if (healthPercentage <= phase3HealthThreshold && currentPhase != BossPhase.Phase3)
        {
            StartCoroutine(TransitionToPhase(BossPhase.Phase3));
        }
        else if (healthPercentage <= phase2HealthThreshold && currentPhase == BossPhase.Phase1)
        {
            StartCoroutine(TransitionToPhase(BossPhase.Phase2));
        }
        
        // Check death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null) return;
        
        try {
            float healthPercentage = (float)currentHealth / maxHealth;
            Transform barTransform = healthBar.transform.Find("Bar");
            if (barTransform != null)
            {
                barTransform.localScale = new Vector3(healthPercentage, barTransform.localScale.y);
            }
        }
        catch (System.Exception e) {
            Debug.LogError("Error updating health bar: " + e.Message);
        }
    }

    private IEnumerator TransitionToPhase(BossPhase newPhase)
    {
        isTransitioning = true;
        
        // Visual feedback for phase transition
        StartCoroutine(PhaseTransitionEffect());
        
        // Wait for transition effect
        yield return new WaitForSeconds(2f);
        
        // Check if this object still exists
        if (this == null || gameObject == null) yield break;
        
        // Update phase and appearance
        currentPhase = newPhase;
        UpdateAppearanceForPhase();
        
        // Reward player for reaching new phase
        if (playerMoney != null)
        {
            playerMoney.addMoney(moneyRewardPerPhase);
        }
        
        isTransitioning = false;
    }

    private IEnumerator PhaseTransitionEffect()
    {
        if (spriteRenderer == null) yield break;
        
        // Flash effect to indicate phase change
        Color originalColor = spriteRenderer.color;
        
        for (int i = 0; i < 5; i++)
        {
            if (spriteRenderer == null) yield break;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            if (spriteRenderer == null) yield break;
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateAppearanceForPhase()
    {
        if (spriteRenderer == null) return;
        
        switch (currentPhase)
        {
            case BossPhase.Phase1:
                spriteRenderer.color = phase1Color;
                transform.localScale = Vector3.one * 2f;
                break;
                
            case BossPhase.Phase2:
                spriteRenderer.color = phase2Color;
                transform.localScale = Vector3.one * 2.5f;
                basicAttackCooldown = 1.5f; // Speed up attacks
                break;
                
            case BossPhase.Phase3:
                spriteRenderer.color = phase3Color;
                transform.localScale = Vector3.one * 3f;
                basicAttackCooldown = 1f; // Speed up attacks further
                specialAttackCooldown = 3f; // More special attacks
                break;
        }
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
        
        // Notify GameManager using the specific boss defeated method
        if (gameManager != null)
        {
            gameManager.BossDefeated();
        }
    }

    private IEnumerator DeathEffect()
    {
        if (spriteRenderer == null) yield break;
        
        // Death animation effect
        for (float t = 1f; t > 0; t -= 0.05f)
        {
            if (spriteRenderer == null || !gameObject) yield break;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, t);
            transform.localScale *= 0.95f;
            yield return new WaitForSeconds(0.05f);
        }
        
        // Destroy after animation completes
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    #region Attack Patterns

    private IEnumerator BasicAttackRoutine()
    {
        Debug.Log("Boss basic attack routine started");
        
        while (!isDead)
        {
            yield return new WaitForSeconds(basicAttackCooldown);
            
            if (!ShouldProcess()) continue;
            
            FireProjectileAtPlayer();
        }
    }

    private IEnumerator SpecialAttackRoutine()
    {
        // Initial delay before first special attack
        yield return new WaitForSeconds(specialAttackCooldown + 3f);
        
        Debug.Log("Boss special attack routine started");
        
        while (!isDead)
        {
            if (gameObject == null) yield break;
            
            // Only use special attack if spawn effect is complete and not transitioning
            if (hasCompletedSpawnEffect && !isTransitioning)
            {
                try
                {
                    switch (currentPhase)
                    {
                    case BossPhase.Phase1:
                            Debug.Log("Boss using Phase 1 special attack");
                            SpreadAttack(3);
                        break;
                            
                    case BossPhase.Phase2:
                            Debug.Log("Boss using Phase 2 special attack");
                            SpreadAttack(5);
                            TriggerAoEAttack();
                        break;
                            
                    case BossPhase.Phase3:
                            Debug.Log("Boss using Phase 3 special attack");
                            SpreadAttack(8);
                            TriggerAoEAttack();
                            SummonMinions();
                        break;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error in boss special attack: " + e.Message);
                }
            }
            yield return new WaitForSeconds(specialAttackCooldown);
        }
    }

    private void FireProjectileAtPlayer()
    {
        if (!ShouldProcess() || playerTransform == null) return;
        
        Debug.Log("Boss firing projectiles at the player");
        
        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        
        GameObject projectile = Instantiate(basicProjectilePrefab, transform.position, Quaternion.identity);
        
        // Rotate projectile to face the player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // Set projectile movement explicitly
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }
        else
        {
            // If no Rigidbody2D, try to use ProjectileMovement component
            ProjectileMovement pm = projectile.GetComponent<ProjectileMovement>();
            if (pm != null)
            {
                // Manually ensure the projectile faces the player
                projectile.transform.right = direction;
            }
        }
    }

    private void SpreadAttack(int projectileCount)
    {
        if (spreadProjectilePrefab == null || !gameObject) return;
        
        float angleStep = 360f / projectileCount;
        float currentAngle = 0f;
        
        for (int i = 0; i < projectileCount; i++)
        {
            // Calculate direction using cos/sin for circular pattern
            Vector2 direction = new Vector2(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad)
            ).normalized;
            
            GameObject projectile = Instantiate(spreadProjectilePrefab, transform.position, Quaternion.identity);
            
            // Rotate to match direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            // Set velocity
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * projectileSpeed;
            }
            
            currentAngle += angleStep;
        }
    }

    private void TriggerAoEAttack()
    {
        if (playerTransform == null || basicProjectilePrefab == null || !gameObject) return;
        
        // Using the projectile system to create an AoE effect
        for (int i = 0; i < aoeProjectileCount; i++)
        {
            // Calculate positions in a circle
            float angle = (i * 360f / aoeProjectileCount) * Mathf.Deg2Rad;
            Vector3 spawnPosition = playerTransform.position + new Vector3(
                Mathf.Cos(angle) * aoeRadius,
                Mathf.Sin(angle) * aoeRadius,
                0
            );
            
            // Spawn projectile at calculated position
            GameObject projectile = Instantiate(basicProjectilePrefab, spawnPosition, Quaternion.identity);
            
            // Make projectiles move outward from center
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = ((Vector2)spawnPosition - (Vector2)playerTransform.position).normalized;
                rb.linearVelocity = direction * projectileSpeed * 0.5f; // Slower spread for AoE effect
            }
            
            // Rotate projectile to face outward
            float projectileAngle = Mathf.Atan2(Mathf.Sin(angle), Mathf.Cos(angle)) * Mathf.Rad2Deg - 90f;
            projectile.transform.rotation = Quaternion.Euler(0, 0, projectileAngle);
        }
    }

    private void SummonMinions()
    {
        if (minionPrefab == null || !gameObject) {
            Debug.LogError("Can't summon minions - minionPrefab is null or boss is destroyed");
            return;
        }
        
        Debug.Log("Boss summoning " + minionsPerSpawn + " minions");
        
        for (int i = 0; i < minionsPerSpawn; i++)
        {
            // Random position around the boss
            Vector2 spawnOffset = Random.insideUnitCircle.normalized * 2f;
            Vector3 spawnPosition = transform.position + new Vector3(spawnOffset.x, spawnOffset.y, 0);
            
            // Instantiate minion
            GameObject minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
            
            // Set minion's player money reference if it has a health controller
            HealthController healthController = minion.GetComponent<HealthController>();
            if (healthController != null && playerMoney != null)
            {
                healthController.playerMoney = playerMoney;
                // Make minions weaker (die in 2 shots)
                healthController.maxHealth = 2;
                healthController.currentHealth = 2;
                // Mark as boss minion for special damage calculations
                healthController.isBossMinion = true;
                
                Debug.Log("Minion created at " + spawnPosition);
            }
        }
    }

    #endregion

    // Add spawn effect
    private IEnumerator SpawnEffect()
    {
        Debug.Log("Boss spawn effect starting");
        
        // Scale from zero
        transform.localScale = Vector3.zero;
        
        // Disable any Rigidbody2D physics during spawn
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
        }
        
        // Fade in over time
        for (float t = 0; t < 1.0f; t += Time.deltaTime)
        {
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = t;
                spriteRenderer.color = c;
            }
            
            // Grow to full size
            float scale = Mathf.Lerp(0, 2f, t);
            transform.localScale = Vector3.one * scale;
            
            yield return null;
        }
        
        // Ensure final values are set correctly
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1;
            spriteRenderer.color = c;
        }
        
        // Re-enable physics after spawn is complete
        if (rb != null)
        {
            rb.simulated = true;
            rb.gravityScale = 0; // Make sure gravity doesn't affect the boss
        }
        
        // Set initial appearance
        UpdateAppearanceForPhase();
        
        // Start attack patterns
        StartCoroutine(BasicAttackRoutine());
        StartCoroutine(SpecialAttackRoutine());
        
        hasCompletedSpawnEffect = true;
        Debug.Log("Boss spawn effect complete");
    }

    private void Update()
    {
        if (!ShouldProcess() || !hasCompletedSpawnEffect || isDead || isTransitioning) return;
        
        // Simple direct movement toward player (like normal enemies)
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
    }

    // Check if this is a proper boss level (level divisible by 10)
    private bool IsProperBossLevel()
    {
        if (gameManager == null) return false;
        return gameManager.level_counter > 0 && gameManager.level_counter % 10 == 0;
    }
    
    // Disable boss visuals and components if not properly spawned
    private void DisableBoss()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.Sleep();
            rb.simulated = false;
        }
        
        // Prevent this from starting attack routines
        enabled = false;
    }
    
    public void SetProperlySpawned()
    {
        properlySpawned = true;
        enabled = true;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = true;
        }
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
        }
        
        // Resize the boss
        transform.localScale = new Vector3(3, 3, 3);
        
        // Initialize boss
        InitializeBoss();
    }
    
    private void InitializeBoss()
    {
        // Find references
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerMoney = FindFirstObjectByType<PlayerMoney>();
        currentHealth = maxHealth;
        
        // Tag as enemy for compatibility
        gameObject.tag = "Enemy";
        
        // Setup healthbar if assigned
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
        }
        
        // Start attack routines
        StartCoroutine(BasicAttackRoutine());
        StartCoroutine(SpecialAttackRoutine());
        
        // Start periodic minion spawning throughout the fight
        StartCoroutine(PeriodicMinionSpawning());
    }

    // Add a regular minion spawning routine (like SimpleBoss has)
    private IEnumerator PeriodicMinionSpawning()
    {
        // Initial delay before first minion spawn
        yield return new WaitForSeconds(5f);
        
        Debug.Log("Boss periodic minion spawning started");
        
        // Set how often minions spawn
        float minionSpawnInterval = 30f; // Increased from 15f to spawn minions less frequently
        
        while (!isDead)
        {
            if (!ShouldProcess()) 
            {
                yield return new WaitForSeconds(1f);
                continue;
            }
            
            // Spawn minions periodically throughout the fight
            SummonMinions();
            
            // Wait before spawning more
            yield return new WaitForSeconds(minionSpawnInterval);
        }
    }

    // IMPORTANT: Add this check to all methods that generate attacks or move the boss
    private bool ShouldProcess()
    {
        return properlySpawned && !isDead && enabled;
    }
}
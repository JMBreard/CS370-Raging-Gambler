using System.Collections;
using UnityEngine;

public class BossEnemy : MonoBehaviour, ProjectileMovement.IDamagable
{
    [Header("Boss Stats")]
    public int maxHealth = 300;
    private int currentHealth;
    public HealthBar healthBar;
    
    [Header("Phase Settings")]
    public enum BossPhase { Phase1, Phase2, Phase3 }
    public BossPhase currentPhase = BossPhase.Phase1;
    public float phase2HealthThreshold = 0.6f; // 60% health
    public float phase3HealthThreshold = 0.3f; // 30% health
    public Color phase1Color = Color.red;
    public Color phase2Color = new Color(1f, 0.5f, 0f); // Orange
    public Color phase3Color = new Color(1f, 0f, 1f); // Purple
    
    [Header("Attack Prefabs")]
    public GameObject basicProjectilePrefab;
    public GameObject spreadProjectilePrefab;
    public GameObject minionPrefab;
    
    [Header("Attack Settings")]
    public float basicAttackCooldown = 2f;
    public float specialAttackCooldown = 5f;
    public int minionsPerSpawn = 2;
    public float projectileSpeed = 5f;
    public int aoeProjectileCount = 8; // Number of projectiles in AoE pattern
    public float aoeRadius = 2f; // Radius of AoE effect
    
    [Header("Gambling Integration")]
    public int moneyRewardOnDeath = 200;
    public int moneyRewardPerPhase = 50;
    
    // References
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;
    private PlayerMoney playerMoney;
    private GameManager gameManager;
    
    // State tracking
    private bool isTransitioning = false;
    private bool isDead = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        // Find references
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerMoney = FindObjectOfType<PlayerMoney>();
        
        // Initialize health and UI
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
        }
        
        // Set initial appearance
        UpdateAppearanceForPhase();
        
        // Start attack patterns
        StartCoroutine(BasicAttackRoutine());
        StartCoroutine(SpecialAttackRoutine());
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
        if (healthBar != null)
        {
            float healthPercentage = (float)currentHealth / maxHealth;
            Transform barTransform = healthBar.transform.Find("Bar");
            if (barTransform != null)
            {
                barTransform.localScale = new Vector3(healthPercentage, barTransform.localScale.y);
            }
        }
    }

    private IEnumerator TransitionToPhase(BossPhase newPhase)
    {
        isTransitioning = true;
        
        // Visual feedback for phase transition
        StartCoroutine(PhaseTransitionEffect());
        
        // Wait for transition effect
        yield return new WaitForSeconds(2f);
        
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
        // Flash effect to indicate phase change
        Color originalColor = spriteRenderer.color;
        
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateAppearanceForPhase()
    {
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
        
        // Notify GameManager
        if (gameManager != null)
        {
            gameManager.EndBossEncounter();
        }
    }

    private IEnumerator DeathEffect()
    {
        // Death animation effect
        for (float t = 1f; t > 0; t -= 0.05f)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, t);
            transform.localScale *= 0.95f;
            yield return new WaitForSeconds(0.05f);
        }
        
        // Destroy after animation completes
        Destroy(gameObject);
    }

    #region Attack Patterns

    private IEnumerator BasicAttackRoutine()
    {
        while (!isDead)
        {
            if (!isTransitioning)
            {
                switch (currentPhase)
                {
                    case BossPhase.Phase1:
                        FireSingleProjectile();
                        break;
                        
                    case BossPhase.Phase2:
                        FireSingleProjectile();
                        yield return new WaitForSeconds(0.3f);
                        FireSingleProjectile();
                        break;
                        
                    case BossPhase.Phase3:
                        FireSingleProjectile();
                        yield return new WaitForSeconds(0.2f);
                        FireSingleProjectile();
                        yield return new WaitForSeconds(0.2f);
                        FireSingleProjectile();
                        break;
                }
            }
            yield return new WaitForSeconds(basicAttackCooldown);
        }
    }

    private IEnumerator SpecialAttackRoutine()
    {
        // Initial delay before first special attack
        yield return new WaitForSeconds(specialAttackCooldown);
        
        while (!isDead)
        {
            if (!isTransitioning)
            {
                switch (currentPhase)
                {
                    case BossPhase.Phase1:
                        SpreadAttack(3);
                        break;
                        
                    case BossPhase.Phase2:
                        SpreadAttack(5);
                        TriggerAoEAttack();
                        break;
                        
                    case BossPhase.Phase3:
                        SpreadAttack(8);
                        TriggerAoEAttack();
                        SummonMinions();
                        break;
                }
            }
            yield return new WaitForSeconds(specialAttackCooldown);
        }
    }

    private void FireSingleProjectile()
    {
        if (playerTransform == null || basicProjectilePrefab == null) return;

        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        
        GameObject projectile = Instantiate(basicProjectilePrefab, transform.position, Quaternion.identity);
        
        // Rotate projectile to face the player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // Set projectile velocity manually if it doesn't have ProjectileMovement script
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
    }

    private void SpreadAttack(int projectileCount)
    {
        if (spreadProjectilePrefab == null) return;
        
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
                rb.velocity = direction * projectileSpeed;
            }
            
            currentAngle += angleStep;
        }
    }

    private void TriggerAoEAttack()
    {
        if (playerTransform == null || basicProjectilePrefab == null) return;
        
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
                rb.velocity = direction * projectileSpeed * 0.5f; // Slower spread for AoE effect
            }
            
            // Rotate projectile to face outward
            float projectileAngle = Mathf.Atan2(Mathf.Sin(angle), Mathf.Cos(angle)) * Mathf.Rad2Deg - 90f;
            projectile.transform.rotation = Quaternion.Euler(0, 0, projectileAngle);
        }
    }

    private void SummonMinions()
    {
        if (minionPrefab == null) return;
        
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
            }
        }
    }

    #endregion
}
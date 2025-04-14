using System;
using Unity.VisualScripting;
using UnityEngine;

public class HealthController : MonoBehaviour, ProjectileMovement.IDamagable
{
    [Header("Health Settings")]
    [SerializeField] public int maxHealth = 10;
    [SerializeField] public int currentHealth;

    private bool isDead = false;

    public HealthBar healthbar;

    public PlayerMoney playerMoney;

    public event System.EventHandler OnHealthChanged;

    public GameManager gameManager;

    // Flag to identify boss minions
    public bool isBossMinion = false;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        currentHealth = maxHealth;

        // Check if healthbar is assigned and only call Setup for the player
        if (healthbar != null)
        {
            healthbar.Setup(this);  // Assign the health bar only for the player
        }

        // Setup the player money system
        if (playerMoney != null)
        {
            playerMoney.Setup(this);
        }
    }

    // Implement the interface property.
    public int Health
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    // This method will be called when damage is dealt via the interface.
    public void Damage()
    {
        // For boss minions, take fixed damage regardless of damage reduction from wagers
        if (isBossMinion)
        {
            // Guarantee boss minions die in 2 hits max
            TakeDamage(Mathf.Max(currentHealth / 2, 1));
            return;
        }
        
        TakeDamage(1);
    }


    public void TakeDamage(int amount)
    {
        if (isDead)
            return; // Do not process damage if already dead

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0; // Prevent health from going below zero
            Die();
        }

        if (OnHealthChanged != null)
        {
            OnHealthChanged(this, EventArgs.Empty); //The "this" passed is the enemy at first
        }
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // When the enemy dies, reward the player with money
        if (playerMoney != null)
        {
            playerMoney.addMoney(gameManager.level_counter * 5);
        }
        if (CompareTag("Player"))
        {
            // Player death handling (only deactivate if game is truly over)
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.GameOver(); // GameOver() is in GameManager.cs
                // Only disable player if game is truly over
                // Don't disable the player as it causes issues with room transitions
                // Instead, just halt player movement/actions via the GameOver state
                
                // Don't deactivate: gameObject.SetActive(false);
                
                // Still subtract money
                if (playerMoney != null)
                {
                    playerMoney.subtractMoney(playerMoney.money / 2);
                }
            }
        }
        else
        {
            // Enemy (or other damageable) death handling
            if (gameManager != null && !isBossMinion) // Skip EnemiesLeftUpdate for boss minions
            {
                gameManager.EnemiesLeftUpdate(); // Update remaining enemies for win condition
            }
            
            // Destroy the game object
            Destroy(gameObject);
        }
    }

    public void reduceMaxHealth()
    {
        maxHealth -= 1;
        Debug.Log("max health: " + maxHealth);
    }

    public void increaseMaxHealth() {
        maxHealth += 1;
        Debug.Log("max health: " + maxHealth);
    }

    public void Steal()
    {
        int num = playerMoney.money;
        playerMoney.subtractMoney( gameManager.level_counter * 2 );
    }
}

using System;
using Unity.VisualScripting;
using UnityEngine;

public class HealthController : MonoBehaviour, ProjectileMovement.IDamagable
{
    [Header("Health Settings")]
    public int maxHealth;
    public int currentHealth;

    public bool isDead = false;

    public HealthBar healthbar;

    public PlayerMoney playerMoney;

    public event EventHandler OnHealthChanged;

    public GameManager gameManager;

    int DamageAmount = 1;


    void Start()
    {
        gameManager = (GameManager)GameObject.Find("Game Manager").GetComponent("GameManager");
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
        TakeDamage(DamageAmount);
        Debug.Log("Current health: " + currentHealth);
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

    void Die()
    {
        if (isDead)
        {
            return; // Already dead, prevent multiple executions
        }

        isDead = true;

        // When the enemy dies, reward the player with money
        if (playerMoney != null)
        {
            playerMoney.addMoney(gameManager.level_counter * 5);
        }
        if (CompareTag("Player"))
        {
            // Player death handling (disable the player)
            FindFirstObjectByType<GameManager>().GameOver(); // GameOver() is in GameManager.cs
            gameObject.SetActive(false);
            //playerMoney.subtractMoney(playerMoney.money / 2);
        }
        else
        {
            if (CompareTag("Obstacle"))
            {
                gameObject.SetActive(false);
            }
            else
            {
                // Enemy (or other damageable) death handling (destroy the game object)
                Destroy(gameObject);
                gameManager.EnemiesLeftUpdate(); // Update remaining enemies for win condition
            }
        }
    }

    public void reduceMaxHealth()
    {
        if (maxHealth <= 1)
        {
            Debug.Log("Max health is already at minimum.");
            return; 
        }

        maxHealth -= 1;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("current health: " + currentHealth);
        Debug.Log("max health: " + maxHealth);
    }

    public void increaseMaxHealth()
    {
        maxHealth += 1;
        Debug.Log("max health: " + maxHealth);
    }

    public void increaseCurrentHealth()
    {
        if (currentHealth >= maxHealth)
        {
            Debug.Log("Current health is already at max.");
            return; 
        }

        currentHealth += 1;
        Debug.Log("current health: " + currentHealth);
    }

    public void increaseDamage() {
        DamageAmount += 1;
        Debug.Log("Current damage: " + DamageAmount);
    }

    public void DecreaseDamage() {
        if (DamageAmount <= 1) {
            Debug.Log("Damage cannot be reduced further.");
            return; 
        }
        
        DamageAmount -= 1;
        Debug.Log("Current damage: " + DamageAmount);
    }

    public void Steal()
    {
        playerMoney.subtractMoney(gameManager.level_counter * 2);
    }
}

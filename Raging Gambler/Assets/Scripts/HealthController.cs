using System;
using Unity.VisualScripting;
using UnityEngine;

public class HealthController : MonoBehaviour, ProjectileMovement.IDamagable
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    private bool isDead = false;

    public HealthBar healthbar;

    public PlayerMoney playerMoney;

    public event EventHandler OnHealthChanged;


    void Start()
    {
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
        TakeDamage(1);
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
            return; // Already dead, prevent multiple executions

        isDead = true;

        // When the enemy dies, reward the player with money
        if (playerMoney != null)
        {
            playerMoney.addMoney(10);
        }
        if (CompareTag("Player"))
        {
            // Player death handling (disable the player)
            FindFirstObjectByType<GameManager>().GameOver(); // GameOver() is in GameManager.cs
            gameObject.SetActive(false);
            playerMoney.subtractMoney(playerMoney.money / 2);
        }
        else
        {
            // Enemy (or other damageable) death handling (destroy the game object)
            Destroy(gameObject);

        }
    }

    public void reduceMaxHealth() {
        maxHealth -= 1;
        Debug.Log("max health: " + maxHealth);
    }
}

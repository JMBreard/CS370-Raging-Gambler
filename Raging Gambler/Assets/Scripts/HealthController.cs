using UnityEngine;

public class HealthController : MonoBehaviour, ProjectileMovement.IDamagable
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
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
    }

    void Die()
    {
        if (isDead)
            return; // Already dead, prevent multiple executions

        isDead = true;

        if (CompareTag("Player"))
        {
            // Player death handling (disable the player)
            FindFirstObjectByType<GameManager>().GameOver(); // GameOver() is in GameManager.cs
            gameObject.SetActive(false);
        }
        else
        {
            // Enemy (or other damageable) death handling (destroy the game object)
            Destroy(gameObject);
        }
    }
}

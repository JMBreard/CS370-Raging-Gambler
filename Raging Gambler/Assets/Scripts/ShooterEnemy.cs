using UnityEngine;

public class ShooterEnemy : Enemy
{
    [Header("Shooting Settings")]
    [Tooltip("Prefab of the projectile to shoot")]
    [SerializeField] private GameObject projectilePrefab;

    [Tooltip("Time in seconds between shots")]
    [SerializeField] private float shootRate = 3f;

    [Tooltip("Speed at which the projectile is fired")]
    [SerializeField] private float projectileSpeed = 5f;

    [Tooltip("Optional shoot point transform (if not set, enemy's position is used)")]
    [SerializeField] private Transform shootPoint;

    private float shootTimer = 0f;

    protected override void FixedUpdate()
    {
        // Call the default movement inhertited from the Enemy Controller
        base.FixedUpdate();

        // Update shoot timer and fire when ready.
        shootTimer += Time.fixedDeltaTime;
        if (shootTimer >= shootRate)
        {
            shootTimer = 0f;
            ShootAtPlayer();
        }
    }

    private void ShootAtPlayer()
    {

        // Use the shoot point if provided, otherwise default to enemy's position.
        Vector3 spawnPos = shootPoint ? shootPoint.position : transform.position;
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb != null)
        {
            // Calculate the direction vector from the spawn position to the player's position.
            Vector2 direction = ((Vector2)player.position - (Vector2)spawnPos).normalized;

            // Set projectile rotation to face the direction it will travel.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle -= 90f;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
            projectileRb.linearVelocity = direction * projectileSpeed;
        }
    }
}

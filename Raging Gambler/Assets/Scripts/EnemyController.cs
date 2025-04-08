using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public GameManager gameManager;
    protected Transform player;
    protected Rigidbody2D rb;

    private bool contact = false; // Tracks whether enemy is contacting player
    private float dmgTimeInterval = 1.0f; // Deal dmg every time interval (in secs)
    private float dmgTimer = 0.0f; // Tracks contact time
    private Coroutine dmgCoroutine;
    private string enemyType; // Enemy type corresponds with trait action
    public HealthController currentHealth;



    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
    }

    // Set to virtual to allow override in child classes.
    protected virtual void FixedUpdate()
    {
        if (player != null)
        {
            // Move towards the player in a straight line.
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Use the IDamagable interface to damage the player.
            ProjectileMovement.IDamagable playerHealth = collision.gameObject.GetComponent<ProjectileMovement.IDamagable>();
            HealthController playerMoney = collision.gameObject.GetComponent<HealthController>();

            enemyType = gameObject.name;
            contact = true;

            if (playerHealth != null && contact == true)
            {
                Trait(enemyType, playerMoney);
                dmgCoroutine = StartCoroutine(SustainedDamage(playerHealth));
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (dmgCoroutine != null)
            {
                StopCoroutine(dmgCoroutine);
                contact = false;
            }
        }
    }

    private IEnumerator SustainedDamage(ProjectileMovement.IDamagable playerHealth)
    {
        while (contact == true)
        {
            if (Time.time >= dmgTimer)
            {
                playerHealth.Damage();
                dmgTimer = Time.time + dmgTimeInterval;
            }
            yield return null;
        }
    }

    public int CriticalHit()
    {
        // critChance is a % (between 0 and 100)
        int critChance = UnityEngine.Random.Range(1, 101);
        int num = UnityEngine.Random.Range(1, 101);
        int multiplier = 1; // Default to one for now
        if (num <= critChance)
        {
            return gameManager.level_counter * multiplier; // Deals damage proportional to the level
        }
        return 0; // Deals no damage
    }

    private void Trait(string enemyType, HealthController playerMoney)
    {
        if (enemyType.Contains("Fast"))
        {
            playerMoney.Steal();
        }
        if (enemyType.Contains("Shooter"))
        {
            int damage = CriticalHit();
            playerMoney.TakeDamage(damage);
        }
    }
}

using System.Collections;
using Unity.VisualScripting;
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
    private SpriteRenderer spriteRenderer;



    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Set to virtual to allow override in child classes.
    protected virtual void FixedUpdate()
    {
        if (player != null)
        {
            
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            RaycastHit2D hit = Physics2D.CircleCast(rb.position, 0.4f, direction, 1f, LayerMask.GetMask("Obstacles"));

            if (hit.collider != null) {
            // Steer to the right of the obstacle
            Vector2 avoidDir = Vector2.Perpendicular(hit.normal).normalized;
            rb.MovePosition(rb.position + avoidDir * speed * Time.fixedDeltaTime);
            } else {
            // Go straight toward player
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
            }

            if (direction.x == 0) {
                return;
            }else if (direction.x < 0){
                spriteRenderer.flipX = true;
            } else {
                spriteRenderer.flipX = false;
            }

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
        // critChance scales up to 33% at level 17.
        int critChance = gameManager.level_counter * 2;
        if (critChance > 33)
        {
            critChance = 33;
        }
        int num = UnityEngine.Random.Range(1, 101);
        int multiplier = 1; // Default to one for now: don't know if it needs to be less or more
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

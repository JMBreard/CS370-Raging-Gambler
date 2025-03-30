using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    protected Transform player; 
    protected Rigidbody2D rb;

    private bool contact = false; // Tracks whether enemy is contacting player
    private float dmgTimeInterval = 1.0f; // Deal dmg every time interval (in secs)
    private float dmgTimer = 0.0f; // Tracks contact time
    private Coroutine dmgCoroutine;

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

            contact = true;
            if (playerHealth != null && contact == true)
            {
                dmgCoroutine = StartCoroutine(SustainedDamage(playerHealth));
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
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
        while(contact == true)
        {
            if (Time.time >= dmgTimer)
            {
                playerHealth.Damage();
                dmgTimer = Time.time + dmgTimeInterval;
            }
            yield return null;
        }
    }
}

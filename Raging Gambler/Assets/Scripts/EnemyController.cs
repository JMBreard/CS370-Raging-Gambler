using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    Transform player;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // Move towards the player
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Use the IDamagable interface to damage the player.
            ProjectileMovement.IDamagable playerHealth = collision.gameObject.GetComponent< ProjectileMovement.IDamagable>();
            if (playerHealth != null)
            {
                playerHealth.Damage();
            }
        }
    }
}

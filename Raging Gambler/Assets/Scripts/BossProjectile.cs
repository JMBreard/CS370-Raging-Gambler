using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 1f;
    public float speed = 10f;
    public float lifetime = 5f;
    public bool trackingProjectile = false;
    public float trackingStrength = 0.5f;
    
    [Header("Visual Effects")]
    public Color projectileColor = Color.red;
    public float pulseRate = 1f;
    public bool rotateProjectile = true;
    public float rotationSpeed = 360f;
    
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 direction;
    private float timer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        // Set initial color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = projectileColor;
        }
    }
    
    void Start()
    {
        // Find player for tracking if needed
        if (trackingProjectile)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        
        // Initial direction is set by boss when instantiating
        if (rb.velocity.magnitude < 0.1f)
        {
            // Default movement if no velocity was set
            rb.velocity = transform.up * speed;
        }
        
        // Set lifetime
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        // Tracking behavior
        if (trackingProjectile && playerTransform != null)
        {
            // Get direction to player
            Vector2 targetDirection = ((Vector2)playerTransform.position - rb.position).normalized;
            
            // Gradually rotate velocity towards player
            rb.velocity = Vector2.Lerp(rb.velocity.normalized, targetDirection, trackingStrength * Time.deltaTime) * speed;
            
            // Rotate sprite to match direction
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else if (rotateProjectile)
        {
            // Simple rotation for non-tracking projectiles
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        
        // Visual pulsing effect
        if (spriteRenderer != null && pulseRate > 0)
        {
            float pulse = 0.75f + Mathf.PingPong(Time.time * pulseRate, 0.5f);
            spriteRenderer.color = new Color(projectileColor.r, projectileColor.g, projectileColor.b, pulse);
        }
        
        // Increment timer
        timer += Time.deltaTime;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit a player
        if (other.CompareTag("Player"))
        {
            ProjectileMovement.IDamagable playerHealth = other.GetComponent<ProjectileMovement.IDamagable>();
            if (playerHealth != null)
            {
                playerHealth.Damage();
            }
            
            // Destroy projectile
            Destroy(gameObject);
        }
        else if (other.CompareTag("Obstacle"))
        {
            // Hit an obstacle, destroy projectile
            Destroy(gameObject);
        }
    }
} 
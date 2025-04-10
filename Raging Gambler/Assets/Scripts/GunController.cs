using UnityEngine;

public class GunController : MonoBehaviour
{
    // Reference to the player's transform.
    public Transform player;

    // Reference to the main camera to convert mouse coordinates.
    private Camera mainCamera;

    // Reference to the SpriteRenderer for flipping the gun.
    private SpriteRenderer gunSpriteRenderer;

    // Reference to the player's SpriteRenderer to get the player's width.
    private SpriteRenderer playerSpriteRenderer;

    // Optional: A manual offset multiplier for further adjustment.
    public float offsetMultiplier = 1.0f;

    // Reference to the bullet spawn object, which is a child of the weapon.
    public Transform bulletSpawn;

    // Store the gun's initial local position relative to the player.
    private Vector3 initialLocalPosition;

    // Store bullet spawn's initial local position relative to the gun.
    private Vector3 bulletSpawnInitialLocalPosition;

    // Adjust this value to compensate for the -1 y-offset when the gun flips.
    public float bulletSpawnYOffsetWhenFlipped = 1f;

    void Start()
    {
        // Set the player if not manually set.
        if (player == null)
            player = transform.parent;

        // Get main camera and SpriteRenderers.
        mainCamera = Camera.main;
        gunSpriteRenderer = GetComponent<SpriteRenderer>();

        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer == null)
        {
            Debug.LogError("Player SpriteRenderer not found on the player object.");
        }

        // Save the gun's starting local position (relative to the player).
        initialLocalPosition = transform.localPosition;

        // Save the bullet spawn's original local position (relative to the gun).
        if (bulletSpawn != null)
        {
            bulletSpawnInitialLocalPosition = bulletSpawn.localPosition;
        }
        else
        {
            Debug.LogWarning("Bullet spawn transform is not assigned.");
        }
    }

    void Update()
    {
        // 1. Get the mouse position in screen coordinates and convert to world coordinates.
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;  // Ensure we're working in 2D.

        // 2. Calculate the direction vector from the player to the mouse.
        Vector2 direction = mouseWorldPos - player.position;

        // 3. Compute the angle in degrees.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 4. Rotate the gun so it points toward the mouse.
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 5. Calculate the gun's base position as the player's position plus the original offset.
        Vector3 newPosition = player.position + initialLocalPosition;

        // 6. Check if the mouse is to the left of the player.
        if (direction.x < 0)
        {
            // Method 1: Flip the gun sprite along the Y-axis.
            gunSpriteRenderer.flipY = true;

            // 7. Calculate the player's width from its bounding box (in world units).
            float playerWidth = playerSpriteRenderer.bounds.size.x;

            // 8. Offset the gun to the left by player's width * offsetMultiplier.
            newPosition += Vector3.left * playerWidth/2 * offsetMultiplier;

            // 9. Adjust the bullet spawn object's y-position so it doesn't shift undesirably.
            // Here we add bulletSpawnYOffsetWhenFlipped to restore its intended offset.
            if(bulletSpawn != null)
            {
                bulletSpawn.localPosition = new Vector3(
                    bulletSpawnInitialLocalPosition.x,
                    bulletSpawnInitialLocalPosition.y - bulletSpawnYOffsetWhenFlipped ,
                    bulletSpawnInitialLocalPosition.z);
            }
        }
        else
        {
            // When aiming right, reset the flip.
            gunSpriteRenderer.flipY = false;

            // Reset the bullet spawn's local position to its original value.
            if(bulletSpawn != null)
            {
                bulletSpawn.localPosition = bulletSpawnInitialLocalPosition;
            }
        }

        // 10. Update the gun's world position.
        transform.position = newPosition;
    }
}

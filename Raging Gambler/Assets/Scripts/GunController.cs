using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform player;
    private Camera mainCamera;
    private SpriteRenderer gunSpriteRenderer;
    private SpriteRenderer playerSpriteRenderer;
    public float offsetMultiplier = 1.0f;

    // Store the gun's initial local position relative to the player.
    private Vector3 initialLocalPosition;

    void Start()
    {
        // If the player reference is not set, try to find it in the parent.
        if (player == null)
            player = transform.parent;

        
        mainCamera = Camera.main;
        
        gunSpriteRenderer = GetComponent<SpriteRenderer>();

        // Get the player's SpriteRenderer.
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer == null)
        {
            Debug.LogError("Player SpriteRenderer not found on the player object.");
        }

        // Save the gun's starting local position relative to the player.
        initialLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // Get the mouse position in screen coordinates and convert to world coordinates.
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        // Calculate the direction vector from the player to the mouse.
        Vector2 direction = mouseWorldPos - player.position;

        // Compute angle in degrees.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the gun so it points toward the mouse.
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Calculate the gun's base position as the player's position plus the  intial offset.
        Vector3 newPosition = player.position + initialLocalPosition;

        // Check if the mouse is to the left of the player.
        if (direction.x < 0)
        {
            // Flip the gun sprite along the Y-axis.
            gunSpriteRenderer.flipY = true;

            // Calculate the player's width from its bounding box (in world units).
            float playerWidth = playerSpriteRenderer.bounds.size.x;

            // Offset the gun to the left by player's width * offsetMultiplier.
            newPosition += Vector3.left * playerWidth/2 * offsetMultiplier;
        }
        else
        {
            // When aiming right, reset the flip.
            gunSpriteRenderer.flipY = false;
        }
        transform.position = newPosition;
    }
}

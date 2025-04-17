using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform player;
    private Camera mainCamera;
    private SpriteRenderer gunSpriteRenderer;
    private SpriteRenderer playerSpriteRenderer;
    public float offsetMultiplier = 1.0f;
    public Transform bulletSpawn;

   
    private Vector3 initialLocalPosition; //Gun's inital local position relative to player
    private Vector3 bulletSpawnInitialLocalPosition; // Bullet spawn's intial local position relative to gun
    public float bulletSpawnYOffsetWhenFlipped = 1f;

    void Start()
    {
        player = transform.parent;

        // Get main camera and SpriteRenderers.
        mainCamera = Camera.main;
        gunSpriteRenderer = GetComponent<SpriteRenderer>();
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        // Save the gun's starting local position (relative to the player)
        initialLocalPosition = transform.localPosition;

        // Save the bullet spawn's original local position (relative to the gun)
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
        // 1. Get the mouse position in screen coordinates and convert to world coordinates
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector2 direction = mouseWorldPos - player.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //Rotate the gun so it points toward the mouse.
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Vector3 newPosition = player.position + initialLocalPosition;

        //if mouse is left of the player
        if (direction.x < 0)
        {
            //Flip gun and player sprite along y-axis and x-axis respectively
            gunSpriteRenderer.flipY = true;
            playerSpriteRenderer.flipX = true;

            float playerWidth = playerSpriteRenderer.bounds.size.x;

            //Offset the gun to the left by player's width * offsetMultiplier
            newPosition += Vector3.left * playerWidth/2 * offsetMultiplier;

            //Adjust the bullet spawn object's y-position so it doesn't shift
            //Add bulletSpawnYOffsetWhenFlipped to restore offset
            if(bulletSpawn != null)
            {
                bulletSpawn.localPosition = new Vector3(bulletSpawnInitialLocalPosition.x, bulletSpawnInitialLocalPosition.y - bulletSpawnYOffsetWhenFlipped, bulletSpawnInitialLocalPosition.z);
            }
        }
        else
        {
            //When aiming right, reset the flip
            gunSpriteRenderer.flipY = false;
            playerSpriteRenderer.flipX = false;
            // Reset the bullet spawn's local position to its original value.
                bulletSpawn.localPosition = bulletSpawnInitialLocalPosition;
        }

        //Update the gun's world position
        transform.position = newPosition;
    }
}

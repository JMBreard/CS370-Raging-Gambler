using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;


public class CrosshairController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The SpriteRenderer component on your crosshair GameObject")]
    [SerializeField] private SpriteRenderer crosshairRenderer;
    [Tooltip("Default look for the crosshair (from your sliced sheet)")]
    [SerializeField] private Sprite defaultSprite;
    [Tooltip("Alternate look when you shoot (from your sliced sheet)")]
    [SerializeField] private Sprite shootSprite;
    [SerializeField] private GameObject pausePanel;  // your Pause Panel GameObject

    [Header("Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color shootColor   = Color.red;

    [Header("Cursor Animation")]
    [SerializeField][Range(0.01f, 0.5f)] private float flashDuration = 0.1f;

    private void Awake()
    {
        // Hide default cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None; // Curosor is not locked to game screen
        crosshairRenderer.sprite = defaultSprite;
        crosshairRenderer.color  = defaultColor;
    }

    private void OnEnable()
    {
        PlayerController.OnShoot += HandleOnShoot;
    }

    private void OnDisable()
    {
        PlayerController.OnShoot -= HandleOnShoot;
    }

    private void Update()
    {

    // Check if mouse is over UI first
    bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    if (overUI)
    {
        // Show defualt cursor and hide custom sprite
        Cursor.visible = true;
        crosshairRenderer.enabled = false;
        return;
    } else {
        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.None; 
        crosshairRenderer.enabled = true;
    }

        // Show/hide depending on whether Canvas/PauseMenu/Panel is active
        bool paused = pausePanel != null ? pausePanel.activeSelf : false;
        crosshairRenderer.enabled = !paused;
        if (!paused){
            Vector3 mouseScreen = Input.mousePosition;
            mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreen);
            crosshairRenderer.transform.position = worldPos;
        }
    }

    private void HandleOnShoot()
    {
        
        StopAllCoroutines(); // Stop alll coroutines so flashes don’t overlap or queue up

        // Kick off a new FlashCrosshair coroutine:
        //  - Immediately change the crosshair to the “shoot” color
        //  - Wait for flashDuration seconds
        //  - Revert back to the default color
        StartCoroutine(FlashCrosshair());
    }

    private IEnumerator FlashCrosshair()
    {
        // swap to shooting sprite + color
        crosshairRenderer.sprite = shootSprite;
        crosshairRenderer.color  = shootColor;

        yield return new WaitForSeconds(flashDuration);

        // revert to default sprite + color
        crosshairRenderer.sprite = defaultSprite;
        crosshairRenderer.color  = defaultColor;
    }
}

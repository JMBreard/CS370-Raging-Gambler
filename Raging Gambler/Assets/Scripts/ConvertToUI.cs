using UnityEngine;

public class ConvertToUIElement : MonoBehaviour
{
    public GameObject canvas; // The Canvas to parent the object to

    void Start()
    {
        // Convert the object to a UI object
        ConvertToUI();
    }

    void ConvertToUI()
    {
        // Make sure the object has a CanvasRenderer
        if (gameObject.GetComponent<CanvasRenderer>() == null)
        {
            gameObject.AddComponent<CanvasRenderer>();
        }

        // Replace the regular Transform with RectTransform if it doesn't already have one
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }

        // Set the parent of the object to the Canvas
        gameObject.transform.SetParent(canvas.transform, false);  // 'false' to keep the local position

        // Adjust RectTransform properties
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f); 
        rectTransform.pivot = new Vector2(0.5f, 0.5f); 
    }
}

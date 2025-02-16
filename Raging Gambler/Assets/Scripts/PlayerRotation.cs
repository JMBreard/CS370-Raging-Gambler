using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        // Gets position of cursor
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Gets direction facing the cursor
        Vector2 forwardDirection = cursorPosition - transform.position;

        // Get the angle of the direction the cursor is facing
        float angle = Vector2.SignedAngle(Vector2.up, forwardDirection);
        // Sets the players rotation to face the cursor
        transform.eulerAngles = new Vector3(0, 0, angle);
    }
}

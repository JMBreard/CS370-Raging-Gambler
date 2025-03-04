using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    private float currentPosX = 1.5f;
    private float currentPosY = 0;
    private Vector3 velocity = Vector3.zero;

    private void Update()
    { //Moves the position of the camera to a specific point
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, currentPosY, transform.position.z), ref velocity, speed);
    }

    public void MoveToNewRoom(Transform newRoom)
    { //Sets a new X and Y position for the camera based on the new room given
        currentPosX = newRoom.position.x + 1.5f;
        currentPosY = newRoom.position.y;
    }
}

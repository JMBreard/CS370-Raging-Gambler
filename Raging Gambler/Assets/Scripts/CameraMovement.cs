using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float speed;

     public Transform target;
     public float smoothing = 5f;
     private Vector3 offset;

    private float currentPosX = 1.5f;
    private float currentPosY = 0;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        offset = transform.position - target.position;
    }

    // private void Update()
    // { //Moves the position of the camera to a specific point
    //     transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, currentPosY, transform.position.z), ref velocity, speed);
    // }

    private void LateUpdate()
    {
        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.fixedDeltaTime);
    }
    

    public void MoveToNewRoom(Transform newRoom)
    { //Sets a new X and Y position for the camera based on the new room given
        currentPosX = newRoom.position.x + 1.5f;
        currentPosY = newRoom.position.y;
    }
}

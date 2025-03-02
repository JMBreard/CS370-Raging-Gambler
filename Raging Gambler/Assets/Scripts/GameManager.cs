using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;

    public Camera mainCamera;

    public GameObject currentRoom;
    public GameObject nextRoom;

    private GameObject currentDoor;
    private GameObject nextDoor;

    public GameObject[] currentRoomDoors;
    public GameObject[] nextRoomDoors;

    private int moveRoomX = 23;
    private int moveRoomY = 10;

    private int comeFromRoom = 3;

    Vector3 newPos;

    private void Awake()
    {
        Time.timeScale = 1.0f;
        gameOverUI.SetActive(false);
    }

    public void GameOver()
    {
        // Activate Game Over UI
        gameOverUI.SetActive(true);
        // Pause the game
        Time.timeScale = 0f;
    }

    //TODO- reset money once money system is made
    public void Restart()
    {
        // Reset time scale
        Time.timeScale = 1f;
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void moveToNextRoom()
    {
        int currentDoorIndex = Random.Range(0, currentRoomDoors.Length);
        while(currentDoorIndex == comeFromRoom)
        {
            currentDoorIndex = Random.Range(0, currentRoomDoors.Length);
        }
        currentDoor = currentRoomDoors[currentDoorIndex];
        currentDoor.gameObject.SetActive(false);
        newPos = nextRoom.transform.position;
        nextDoor = null;
        switch(currentDoorIndex) 
        {
            case 0:
                Debug.Log("Left Door Open");
                newPos.x -= moveRoomX;
                nextDoor = nextRoomDoors[1];
                break;
            case 1:
                Debug.Log("Right Door Open");
                newPos.x += moveRoomX;
                nextDoor = nextRoomDoors[0];
                break;
            case 2:
                Debug.Log("Top Door Open");
                newPos.y += moveRoomY;
                nextDoor = nextRoomDoors[3];
                break;
            case 3:
                Debug.Log("Bottom Door Open");
                newPos.y -= moveRoomY;
                nextDoor = nextRoomDoors[2];
                break;
        }
        nextRoom.transform.position = newPos;
        nextRoom.gameObject.SetActive(true);
        nextDoor.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            moveToNextRoom();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            this.transform.position = newPos;
            currentDoor.gameObject.SetActive(true);
            nextDoor.gameObject.SetActive(true);
            
        }
    }
}

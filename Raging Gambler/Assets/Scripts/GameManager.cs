using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;

    public CameraMovement mainCamera; //Controls the camera movement

    public GameObject currentRoom; //The whole current room
    public GameObject nextRoom; //The next room

    private GameObject currentDoor; //The current door selected
    private GameObject nextDoor; //The door from the next room selected

    public GameObject[] currentRoomDoors; //An array of all of the doors in current room
    public GameObject[] nextRoomDoors; //An array for all of the doors in the next room

    private int moveRoomX = 23; //How much to move a room in the X axis
    private int moveRoomY = 10; //How much to move a room in the Y axis

    private int comeFromRoom = 3; //Initially sets the room the player comes from as the bottom door
    private int currentDoorIndex;
    private bool movingRooms;

    Vector3 newPos;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;

    public GameObject enemySpawner;

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
        movingRooms = true;
        currentDoorIndex = Random.Range(0, currentRoomDoors.Length); // Picks a random door
        while(currentDoorIndex == comeFromRoom)
        { //If the index is the door the player came from, pick a different door
            currentDoorIndex = Random.Range(0, currentRoomDoors.Length);
        }
        currentDoor = currentRoomDoors[currentDoorIndex]; //Get the door object
        currentDoor.gameObject.SetActive(false); //Turn the door off
        newPos = nextRoom.transform.position; //Initialize the next position of the room
        nextDoor = null; //Intialize the next door
        switch(currentDoorIndex) 
        { //Depending on which door was picked, the switch sets the new position and door, as well as where the player is coming from
            case 0:
                Debug.Log("Left Door Open");
                newPos.x -= moveRoomX; //Moves the next room to the left
                nextDoor = nextRoomDoors[1]; //Sets the door from the next room
                comeFromRoom = 1; //Sets what direction the player came from
                break;
            case 1:
                Debug.Log("Right Door Open");
                newPos.x += moveRoomX; //Moves the next room to the right
                nextDoor = nextRoomDoors[0];
                comeFromRoom = 0;
                break;
            case 2:
                Debug.Log("Top Door Open");
                newPos.y += moveRoomY; //Moves the next room up
                nextDoor = nextRoomDoors[3];
                comeFromRoom = 3;
                break;
            case 3:
                Debug.Log("Bottom Door Open");
                newPos.y -= moveRoomY; //Moves the next room down
                nextDoor = nextRoomDoors[2];
                comeFromRoom = 2;
                break;
        }
        nextRoom.transform.position = newPos; //Sets the transformation of the next room to whatever direction was picked
        nextRoom.gameObject.SetActive(true); //Turns on the next room
        nextDoor.gameObject.SetActive(false); //Turns off the next door
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T) && !movingRooms) //To test the room generation
        {
            moveToNextRoom();
        }
        if (remainingTime > 1)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime < 1 && !movingRooms)
        {
            remainingTime = 0;
            enemySpawner.gameObject.SetActive(false);
            moveToNextRoom();
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            currentDoor.gameObject.SetActive(true); //Turns on the doors so the room closes
            nextDoor.gameObject.SetActive(true);
            mainCamera.MoveToNewRoom(nextRoom.transform); //Moves the camera to the new room
            currentRoom.transform.position = newPos; //Moves the old current room to the new room
            GameObject oldCurrentRoom = currentRoom; //Swaps the current room and next room as the next room is the new current room
            currentRoom = nextRoom;
            nextRoom = oldCurrentRoom;
            GameObject[] oldDoors = currentRoomDoors; //Swaps the door arrays between the current and next room
            currentRoomDoors = nextRoomDoors;
            nextRoomDoors = oldDoors;
            newPos.x += 1.5f;
            this.transform.position = newPos;
        }
    }

    public void startRoom()
    {
        //I NEED TO CREATE A SCREEN THAT SHOWS WHAT THE NEXT ROOM WILL BE BEFORE THE SHOP
        //THIS WILL RANDOMIZE THE DIFFERENT POSSIBILITIES AND OPTIONS FOR THE TIME LENGTH
        enemySpawner.gameObject.SetActive(true);
        remainingTime = 10;
        GambleManager.instance.ToggleShop();
        movingRooms = false;
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public ObsctacleSpawner obsctacleSpawner; // Have obstacles move with the room 

    private int moveRoomX = 23; //How much to move a room in the X axis
    private int moveRoomY = 10; //How much to move a room in the Y axis

    private int comeFromRoom = 3; //Initially sets the room the player comes from as the bottom door
    private int currentDoorIndex;
    private bool movingRooms;

    Vector3 newPos;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;
    [SerializeField] TextMeshProUGUI remainingEnemies;
    public GameObject enemySpawner;

    [SerializeField] int enemiesNeeded;

    private bool timeRoom;
    private bool enemyRoom;

    [SerializeField] GameObject nextRoomUI;
    [SerializeField] TextMeshProUGUI roomType;
    [SerializeField] TextMeshProUGUI winCondition;

    private bool firstRoom = true;

    private int level_counter = 0;
    [SerializeField] int time_difficulty;
    [SerializeField] int enemy_count_difficulty;

    [SerializeField] public PlayerController pc;

    private bool mouseToggle;

    private void Awake()
    {
        Time.timeScale = 1.0f;
        gameOverUI.SetActive(false);
        pickRoomCondition();
        startRoom();
        firstRoom = false;
        pc = (PlayerController) GameObject.FindWithTag("Player").GetComponent("PlayerController");
    }

    public void GameOver()
    {
        // Activate Game Over UI
        gameOverUI.SetActive(true);
        // Pause the game
        Time.timeScale = 0f;
    }

    public void KillAll()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
    }

    public void ObstacleWipe()
    {
            foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            Destroy(obstacle);
        }
    }

    public void EnemiesLeftUpdate()
    {
        if (enemyRoom)
        {
            enemiesNeeded--;
            Debug.Log("Enemies remaining: " + enemiesNeeded);

            remainingEnemies.text = "Remaining: " + enemiesNeeded;

            if (enemiesNeeded == 0)
            {
                Win();
            }
        }
    }

    private void Win() // move to next room is the win condition
    {
        enemySpawner.gameObject.SetActive(false);
        remainingEnemies.gameObject.SetActive(false);
        enemyRoom = false;
        moveToNextRoom();
    }

    public void Restart()
    {
        // Reset time scale
        Time.timeScale = 1f;
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void pickRoomCondition()
    {
        int roomCondition = Random.Range(1, 3);
        switch (roomCondition)
        {
            case 1:
                //TIME
                timeRoom = true;
                // remainingTime = 10;
                roomType.text = "Survive for:";
                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);
                winCondition.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                break;
            case 2:
                //ENEMY
                enemyRoom = true;
                // enemiesNeeded = 3; // Reset count
                roomType.text = "Defeat:";
                winCondition.text = (enemiesNeeded) + " enemies";
                break;
        }
    }

    public void moveToNextRoom()
    {
        KillAll();
        Increment();

        movingRooms = true;
        currentDoorIndex = Random.Range(0, currentRoomDoors.Length); // Picks a random door
        while (currentDoorIndex == comeFromRoom)
        { //If the index is the door the player came from, pick a different door
            currentDoorIndex = Random.Range(0, currentRoomDoors.Length);
        }
        currentDoor = currentRoomDoors[currentDoorIndex]; //Get the door object
        currentDoor.gameObject.SetActive(false); //Turn the door off
        newPos = nextRoom.transform.position; //Initialize the next position of the room
        nextDoor = null; //Intialize the next door
        switch (currentDoorIndex)
        { //Depending on which door was picked, the switch sets the new position and door, as well as where the player is coming from
            case 0:
                Debug.Log("Left Door Open");
                newPos.x -= moveRoomX; //Moves the next room to the left
                nextDoor = nextRoomDoors[1]; //Sets the door from the next room
                comeFromRoom = 1; //Sets what direction the player came from
                obsctacleSpawner.NewRoomObstacles( newPos ); // Makes obstacles in new room
                break;
            case 1:
                Debug.Log("Right Door Open");
                newPos.x += moveRoomX; //Moves the next room to the right
                nextDoor = nextRoomDoors[0];
                comeFromRoom = 0;
                obsctacleSpawner.NewRoomObstacles( newPos ); // Makes obstacles in new room
                break;
            case 2:
                Debug.Log("Top Door Open");
                newPos.y += moveRoomY; //Moves the next room up
                nextDoor = nextRoomDoors[3];
                comeFromRoom = 3;
                obsctacleSpawner.NewRoomObstacles( newPos ); // Makes obstacles in new room
                break;
            case 3:
                Debug.Log("Bottom Door Open");
                newPos.y -= moveRoomY; //Moves the next room down
                nextDoor = nextRoomDoors[2];
                comeFromRoom = 2;
                obsctacleSpawner.NewRoomObstacles( newPos ); // Makes obstacles in new room
                break;
        }
        nextRoom.transform.position = newPos; //Sets the transformation of the next room to whatever direction was picked
        nextRoom.gameObject.SetActive(true); //Turns on the next room
        nextDoor.gameObject.SetActive(false); //Turns off the next door
    }

    private void Increment()
    {
        level_counter += 1;
        remainingTime += time_difficulty * level_counter;
        enemiesNeeded += enemy_count_difficulty * level_counter;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !movingRooms) //To test the room generation
        {
            moveToNextRoom();
        }
        if (timeRoom && !movingRooms && remainingTime > 1)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (timeRoom && remainingTime < 1 && !movingRooms)
        {
            Debug.Log("TIME ROOM WON");
            remainingTime = 0;
            enemySpawner.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
            timeRoom = false;
            moveToNextRoom();
            // Win();
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if(isMouseOverUIIgnore() && !mouseToggle)
        {
            mouseToggle = true;
            pc.toggleShooting();
        }
        if(!isMouseOverUIIgnore() && mouseToggle && Time.timeScale != 0)
        {
            mouseToggle = false;
            pc.toggleShooting();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            pickRoomCondition();
            nextRoomUI.gameObject.SetActive(true);
            pc.toggleShooting();
            pc.toggleMovement();
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
            newPos.x -= 1.5f;
        }
    }

    public void moveToShop()
    {
        nextRoomUI.gameObject.SetActive(false);
        GambleManager.instance.ToggleShop();
    }
    public void startRoom()
    {
        //I NEED TO CREATE A SCREEN THAT SHOWS WHAT THE NEXT ROOM WILL BE BEFORE THE SHOP
        //THIS WILL RANDOMIZE THE DIFFERENT POSSIBILITIES AND OPTIONS FOR THE TIME LENGTH
        enemySpawner.gameObject.SetActive(true);
        if (timeRoom)
        {
            timerText.gameObject.SetActive(true);
        }
        else if (enemyRoom)
        {
            remainingEnemies.gameObject.SetActive(true);
            remainingEnemies.text = "Remaining: " + (enemiesNeeded);
        }
        if (!firstRoom)
        {
            pc.toggleShooting();
            pc.toggleMovement();
            GambleManager.instance.ToggleShop();
        }
        movingRooms = false;
    }

    private bool isMouseOverUIIgnore()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.GetComponent<Ignore>() == null)
            {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }
        return raycastResultList.Count > 0;
    }
}

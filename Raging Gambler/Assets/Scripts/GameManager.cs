using System.Collections;
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

    private float moveRoomX = 18; //How much to move a room in the X axis
    private float moveRoomY = 11.5f; //How much to move a room in the Y axis

    private int comeFromRoom = 3; //Initially sets the room the player comes from as the bottom door
    private int currentDoorIndex;
    private bool movingRooms;

    Vector3 newPos;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;
    [SerializeField] TextMeshProUGUI remainingEnemies;
    public GameObject enemySpawner;
    public EnemySpawner enemySpawnerComponent;

    [SerializeField] int enemiesNeeded;

    public HealthController healthController;

    private bool timeRoom;
    private bool enemyRoom;

    [SerializeField] GameObject nextRoomUI;
    [SerializeField] TextMeshProUGUI roomType;
    [SerializeField] TextMeshProUGUI winCondition;

    private bool firstRoom = true;

    public int level_counter = 0;
    [SerializeField] int time_difficulty;
    [SerializeField] int enemy_count_difficulty;

    [SerializeField] public PlayerController pc;

    private bool mouseToggle;

    public PlayerMoney playerMoney;

    public GambleManager gambleManager;
    public RewardManager rewardManager;

    public ScoreManager scoreManager;
    public TextMeshProUGUI gameOverScore;
    public TMP_InputField userName;

    [SerializeField] public bool leaderBoardDebug;

    public bool gamePaused = false;
    public GameObject pauseMenu;
    [SerializeField] public bool tutorial;
    public GameObject tutorialStart;
    public GameObject tutorialGameOverScreen;
    public TextMeshProUGUI tutorialScore;
    private bool menuShowing = false;
    public PauseMenu pm;

    [SerializeField] AudioSource buttonClicked;
    [SerializeField] AudioSource doorOpen;
    [SerializeField] AudioSource[] enemyDeathList;
    [SerializeField] AudioSource gameOver;
    [SerializeField] AudioSource stealMoney;
    [SerializeField] AudioSource roomWin;
    bool exit = false;

    private void Awake()
    {
        enemySpawner.gameObject.SetActive(false);
        if (tutorial)
        {
            tutorialStart.gameObject.SetActive(true);
            pc.toggleShooting();
            pc.toggleMovement();
            level_counter = 1;
            Time.timeScale = 0f;
            menuShowing = true;
            scoreManager = (ScoreManager)GameObject.Find("Score Manager").GetComponent("ScoreManager");
            return;
        }
        Time.timeScale = 1.0f;
        gameOverUI.SetActive(false);
        pickRoomCondition();
        startRoom();
        firstRoom = false;
        pc = (PlayerController)GameObject.FindWithTag("Player").GetComponent("PlayerController");
        if (!leaderBoardDebug)
        {
            scoreManager = (ScoreManager)GameObject.Find("Score Manager").GetComponent("ScoreManager");
        }
    }
    public void StartTutorial()
    {
        if (!pm.gamePaused && !gamePaused)
        {
            Debug.Log("Tutorial Started");
            buttonClicked.Play();
            Time.timeScale = 1.0f;
            menuShowing = false;
            gameOverUI.SetActive(false);
            tutorialStart.gameObject.SetActive(false);
            enemySpawner.SetActive(true);
            pc = (PlayerController)GameObject.FindWithTag("Player").GetComponent("PlayerController");
            pc.toggleShooting();
            pc.toggleMovement();
        }
    }
    public void playStealSound()
    {
        stealMoney.Play();
    }
    public void GameOver()
    {
        scoreManager.mainGameMusic.Stop();
        gameOver.Play();
        if (tutorial)
        {
            tutorialGameOver();
            return;
        }
        if (!leaderBoardDebug)
        {
            gameOverScore.text = "Final Score: " + playerMoney.money;
        }
        KillAll();
        // Activate Game Over UI
        gameOverUI.SetActive(true);
        // Pause the game
        Time.timeScale = 0f;
    }

    public void tutorialGameOver()
    {
        tutorialScore.text = "Final Score: " + playerMoney.money;
        KillAll();
        tutorialGameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void KillAll()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
    }
    AudioSource pickRandomEnemyDeathSound()
    {
        int soundIndex = Random.Range(0, enemyDeathList.Length);
        return enemyDeathList[soundIndex];
    }

    public void EnemiesLeftUpdate()
    {
        pickRandomEnemyDeathSound().Play();
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
        gameOver.Stop();
        scoreManager.mainGameMusic.Play();
        buttonClicked.Play();
        StartCoroutine(WaitForSoundToFinish());
    }

    public void Exit()
    {
        gameOver.Stop();
        scoreManager.mainGameMusic.Play();
        buttonClicked.Play();
        exit = true;
        StartCoroutine(WaitForSoundToFinish());
    }
    IEnumerator WaitForSoundToFinish()
    {
        yield return new WaitUntil(() => !buttonClicked.isPlaying);
        if (tutorial)
        {
            if (exit)
            {
                SceneManager.LoadScene("Title Scene");
            }
            else
            {
                SceneManager.LoadScene("Tutorial Scene");
            }

        }
        else
        {
            if (userName.text == "")
            {
                userName.text = "Player " + scoreManager.playerCount;
                scoreManager.playerCount++;
            }
            scoreManager.addEntry(playerMoney.money, userName.text);
            SceneManager.LoadScene("Title Scene");
        }
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
        doorOpen.Play();
        roomWin.Play();
        nextRoom.transform.position = newPos; //Sets the transformation of the next room to whatever direction was picked
        nextRoom.gameObject.SetActive(true); //Turns on the next room
        nextDoor.gameObject.SetActive(false); //Turns off the next door

        obsctacleSpawner.NewRoomObstacles(newPos); // Makes obstacles in new room

        // if a gamble is picked, add wager money from gamble to player's money
        playerMoney = FindAnyObjectByType<PlayerMoney>();
        gambleManager = FindAnyObjectByType<GambleManager>();
        int incrementMoney = 0, wagerIndex = 0;
        foreach (Wagers wager in gambleManager.wagers)
        {
            incrementMoney += wager.reward * gambleManager.WagerCounts[wagerIndex];
            if (gambleManager.WagerCounts[wagerIndex] > 0)
            {
                for (int i = 0; i < gambleManager.WagerCounts[wagerIndex]; i++)
                {
                    if (wagerIndex == 0) { enemySpawnerComponent.DecreaseSpawnRate(); } // enemy population reset
                    if (wagerIndex == 1) { enemySpawnerComponent.SubtractEnemyHealth(); } // enemy health reset
                    if (wagerIndex == 2) { healthController.DecreaseDamage(); } // enemy dmg reset
                }
            }
            wagerIndex++;
        }
        for (int i = 0; i < gambleManager.WagerCounts.Length; i++) { gambleManager.WagerCounts[i] = 0; }
        // Debug.Log(gambleManager.WagerCounts);
        playerMoney.addMoney(incrementMoney);
        rewardManager.HealthRegen(); // Gives back health equivalent to enemy's base damage stat
        if (level_counter % 5 == 0) // Increases enemy base stats: HP +1 and Damage +1 & Player base max HP +1 every 5 levels
        {
            gambleManager.ScaleEnemies();
            healthController.increaseMaxHealth();
        }
    }

    private void Increment()
    {
        level_counter += 1;
        remainingTime += time_difficulty * level_counter;
        enemiesNeeded += enemy_count_difficulty * level_counter;
    }

    private void Update()
    {
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
        if (!tutorial)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        if (!menuShowing && isMouseOverUIIgnore() && !mouseToggle)
        {
            mouseToggle = true;
            pc.toggleShooting();
        }
        if (!menuShowing && !isMouseOverUIIgnore() && mouseToggle && Time.timeScale != 0)
        {
            mouseToggle = false;
            pc.toggleShooting();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && gamePaused == false)
        {
            buttonClicked.Play();
            Time.timeScale = 0;
            gamePaused = true;
            pm.gamePaused = true;
            pauseMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && gamePaused == true)
        {
            buttonClicked.Play();
            Time.timeScale = 1;
            gamePaused = false;
            pm.gamePaused = false;
            pauseMenu.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector3 gmOffset = transform.position - currentRoom.transform.position; //GM's distance from room

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

            transform.position = currentRoom.transform.position + gmOffset;

        }
    }

    public void moveToShop()
    {
        buttonClicked.Play();
        nextRoomUI.gameObject.SetActive(false);
        if (level_counter % 3 == 0)
        {
            RewardManager.instance.ToggleShop();
        }
        else
        {
            GambleManager.instance.ToggleShop();
        }
    }
    public void startRoom()
    {
        enemySpawner.gameObject.SetActive(true);
        if (tutorial)
        {
            buttonClicked.Play();
            return;
        }
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
            buttonClicked.Play();
            pc.toggleShooting();
            pc.toggleMovement();
            if (level_counter % 3 == 0)
            {
                RewardManager.instance.ToggleShop();
            }
            else
            {
                GambleManager.instance.ToggleShop();
            }
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

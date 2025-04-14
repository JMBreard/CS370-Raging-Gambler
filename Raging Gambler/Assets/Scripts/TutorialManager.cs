using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public ObsctacleSpawner obsctacleSpawner;
    public GameObject enemySpawner;
    private bool mouseToggle;
    public PlayerMoney playerMoney;
    public TextMeshProUGUI gameOverScore;
    public bool gamePaused = false;
    public GameObject pauseMenu;
    [SerializeField] public PlayerController pc;

    public void Awake()
    {
        Time.timeScale = 1.0f;
        gameOverUI.SetActive(false);
        pc = (PlayerController)GameObject.FindWithTag("Player").GetComponent("PlayerController");
    }
    public void Update()
    {
        if (isMouseOverUIIgnore() && !mouseToggle)
        {
            mouseToggle = true;
            pc.toggleShooting();
        }
        if (!isMouseOverUIIgnore() && mouseToggle && Time.timeScale != 0)
        {
            mouseToggle = false;
            pc.toggleShooting();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && gamePaused == false)
        {
            Time.timeScale = 0;
            gamePaused = true;
            pauseMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && gamePaused == true)
        {
            Time.timeScale = 1;
            gamePaused = false;
            pauseMenu.SetActive(false);
        }
    }
    public void GameOver()
    {
        KillAll();
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }
    public void KillAll()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
    }
    public void startRoom()
    {
        enemySpawner.gameObject.SetActive(true);
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

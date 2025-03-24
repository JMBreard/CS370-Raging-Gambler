using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class PauseMenu : MonoBehaviour
{
    bool gamePaused = false;
    [SerializeField] GameObject pauseMenu;

    [SerializeField] PlayerController pc;

    bool onPause = false;


    private void Awake()
    {
        pc = (PlayerController)GameObject.FindWithTag("Player").GetComponent("PlayerController");
    }

    /*
    void Update()
    {
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
    */

    public void Pause()
    {
        pauseMenu.SetActive(true);
        gamePaused = true;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        pc.toggleShooting();
        onPause = false;
        gamePaused = false;
        pauseMenu.SetActive(false);
    }

    public void Restart()
    {
        Destroy(GameObject.FindWithTag("Player"));
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        SceneManager.LoadScene("Title Scene");
    }

   /* private void Update()
    {
        if(isMouseOverUI() && !onPause && !gamePaused)
        {
            onPause = true;
            pc.toggleShooting();
        }
        else if(!isMouseOverUI() && onPause && !gamePaused)
        {
            onPause = false;
            pc.toggleShooting();
        }
    }

    private bool isMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }*/

}
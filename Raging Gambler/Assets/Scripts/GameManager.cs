using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;

    private void Awake()
    {
        Time.timeScale = 1.0f;
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
}

using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public ObsctacleSpawner obsctacleSpawner;
    public GameObject enemySpawner;
    private bool mouseToggle;
    public PlayerMoney playerMoney;
    public TextMeshProUGUI gameOverScore;

    public void Awake()
    {
        Time.timeScale = 1.0f;
        gameOverUI.SetActive(false);
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
}

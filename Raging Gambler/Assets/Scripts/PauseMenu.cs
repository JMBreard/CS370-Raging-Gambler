using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public bool gamePaused = false;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject playerStatsPanel;
    [SerializeField] GameManager gm;

    [Header("Player Stats UI")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI maxHealthText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI reloadText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI enemyDamageText;
    public TextMeshProUGUI enemyPopulationText;
    public TextMeshProUGUI enemyHealthText;

    public void Pause()
    {
        UpdateStats();
        if(!gamePaused)
        {
            pauseMenu.SetActive(true);
            playerStatsPanel.SetActive(true);
            gamePaused = true;
            gm.gamePaused = true;
            Time.timeScale = 0;
        }
        else
        {
            pauseMenu.SetActive(false);
            playerStatsPanel.SetActive(false);
            gamePaused = false;
            gm.gamePaused = false;
            Time.timeScale = 1;
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        gamePaused = false;
        gm.gamePaused = false;
        pauseMenu.SetActive(false);
        playerStatsPanel.SetActive(false);
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

    public void UpdateStats()
    {
        // display player stats
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            HealthController health = player.GetComponent<HealthController>();
            PlayerController controller = player.GetComponent<PlayerController>();

            if (health != null)
            {
                healthText.text = $"Health: {health.currentHealth}";
                maxHealthText.text = $"Max Health: {health.maxHealth}";
            }

            if (controller != null)
            {
                speedText.text = $"Current speed: {controller.GetSpeed()}";
                reloadText.text = $"Reload Time: {controller.GetReloadTime()}";
                ammoText.text = $"Current max Ammo: {controller.GetMaxAmmo()}";
            }
        }

        // display enemy
         EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            enemyPopulationText.text = $"Enemy spawn rate: {spawner.spawnRate}s";
            // enemyHealthText.text = $"Enemy health: +{spawner.enemyHealthIncreaser}";
        }

        GambleManager enemy = FindAnyObjectByType<GambleManager>()?.GetComponent<GambleManager>();
        if (enemy != null)
        {
            enemyDamageText.text = $"Enemy DMG: {enemy.dmg_ctr}";
        }
        else
        {
            enemyDamageText.text = "Enemy DMG: ?";
        }
    }
    
}
using System.Collections;
using UnityEngine;

public class BossRoomManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    public GameObject bossUIPrefab;
    public GameObject[] doors; // All doors in the boss room
    
    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public int bossLevelInterval = 10; // Every X levels, spawn a boss
    public int bossReward = 200; // Money reward for defeating the boss
    
    [Header("Room Settings")]
    public float doorClosedDuration = 30f; // How long doors stay closed
    public bool doorsLockedDuringFight = true;
    public GameObject victoryGatePrefab; // Gate to next level that appears on victory
    public Transform victoryGateSpawnPoint;
    
    // State tracking
    private BossEnemy currentBoss;
    private BossUI bossUI;
    private int currentLevel = 1;
    private bool bossDefeated = false;
    
    void Start()
    {
        // Find game manager if not assigned
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        
        // Initialize room state
        SetDoorsActive(true);
    }
    
    public void InitiateEncounter(int level)
    {
        currentLevel = level;
        
        // Only spawn boss on specified intervals
        if (level % bossLevelInterval == 0)
        {
            StartCoroutine(BossEncounterSequence());
        }
    }
    
    private IEnumerator BossEncounterSequence()
    {
        // Lock doors
        if (doorsLockedDuringFight)
        {
            SetDoorsActive(false);
        }
        
        // Short delay for drama
        yield return new WaitForSeconds(1.5f);
        
        // Spawn boss
        GameObject bossObject = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        currentBoss = bossObject.GetComponent<BossEnemy>();
        
        // Scale boss based on level
        ScaleBossDifficulty();
        
        // Create UI
        GameObject uiObject = Instantiate(bossUIPrefab, GameObject.Find("Canvas").transform);
        bossUI = uiObject.GetComponent<BossUI>();
        bossUI.boss = currentBoss;
        bossUI.Show();
        
        // Register for boss defeated event
        StartCoroutine(MonitorBossHealth());
    }
    
    private IEnumerator MonitorBossHealth()
    {
        // Wait until boss is defeated or null
        while (currentBoss != null && currentBoss.Health > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        // If boss was defeated (not just destroyed)
        if (currentBoss == null || currentBoss.Health <= 0)
        {
            OnBossDefeated();
        }
    }
    
    private void OnBossDefeated()
    {
        bossDefeated = true;
        
        // Hide boss UI
        if (bossUI != null)
        {
            bossUI.Hide();
            Destroy(bossUI.gameObject, 1f);
        }
        
        // Unlock doors
        SetDoorsActive(true);
        
        // Spawn gate to next level
        if (victoryGatePrefab != null && victoryGateSpawnPoint != null)
        {
            Instantiate(victoryGatePrefab, victoryGateSpawnPoint.position, Quaternion.identity);
        }
        
        // Notify game manager
        if (gameManager != null)
        {
            // End encounter and check for level progression
            gameManager.EndBossEncounter();
        }
    }
    
    private void ScaleBossDifficulty()
    {
        if (currentBoss == null) return;
        
        // Scale boss stats based on level
        int bossStage = currentLevel / bossLevelInterval;
        
        // Health scaling
        currentBoss.maxHealth = Mathf.RoundToInt(currentBoss.maxHealth * (1 + 0.5f * bossStage));
        
        // Reward scaling
        currentBoss.moneyRewardOnDeath = bossReward * (1 + bossStage / 2);
    }
    
    private void SetDoorsActive(bool active)
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
            {
                door.SetActive(active);
            }
        }
    }
    
    // Call this when player enters the boss room
    public void OnPlayerEnterRoom()
    {
        // If boss wasn't already defeated
        if (!bossDefeated && currentLevel % bossLevelInterval == 0)
        {
            InitiateEncounter(currentLevel);
        }
    }
} 
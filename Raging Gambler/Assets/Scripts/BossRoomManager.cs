using System.Collections;
using UnityEngine;

/*
 * DEPRECATED: This component is kept for compatibility with existing scenes.
 * Boss functionality has been moved to GameManager for simplicity.
 * The boss prefab reference is still used, but the spawning logic has been moved.
 */
public class BossRoomManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    public GameObject[] doors; // All doors in the boss room
    
    [Header("Boss Settings")]
    public GameObject bossPrefab; // Assign a prefab with IDamagable component
    public int bossLevelInterval = 10; // Every X levels, spawn a boss
    public int bossReward = 200; // Money reward for defeating the boss
    
    [Header("Room Settings")]
    public float doorClosedDuration = 30f; // How long doors stay closed
    public bool doorsLockedDuringFight = true;
    
    // State tracking
    private GameObject bossObject;
    private ProjectileMovement.IDamagable bossDamagable;
    private int currentLevel = 1;
    private bool bossDefeated = false;
    
    void Start()
    {
        // Find game manager if not assigned
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
        
        // Initialize room state
        SetDoorsActive(true);
    }
    
    // Called by GameManager to check if boss should spawn
    public void InitiateEncounter(int level)
    {
        Debug.Log("Checking boss encounter for level " + level);
        currentLevel = level;
        
        // Only spawn boss on specified intervals AND ensure level is actually at or above the interval
        // Prevent boss from spawning at level 0
        if (level % bossLevelInterval == 0 && level >= bossLevelInterval)
        {
            Debug.Log("Starting boss encounter at level " + level);
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
        
        // Find player for random spawning
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (playerTransform == null)
        {
            Debug.LogError("Player not found when spawning boss");
            yield break;
        }
        
        // Calculate random spawn position around player
        Vector3 spawnPosition;
        float spawnDistance = 10f; // Distance from player
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        spawnPosition = playerTransform.position + (Vector3)(randomDirection * spawnDistance);
        
        // Spawn boss at random position
        bossObject = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        
        if (bossObject == null)
        {
            Debug.LogError("Failed to instantiate boss");
            yield break;
        }
        
        bossDamagable = bossObject.GetComponent<ProjectileMovement.IDamagable>();
        
        if (bossDamagable == null)
        {
            Debug.LogError("Boss prefab doesn't implement IDamagable interface");
            Destroy(bossObject);
            yield break;
        }
        
        // Mark this boss as properly spawned
        SimpleBoss simpleBoss = bossObject.GetComponent<SimpleBoss>();
        if (simpleBoss != null)
        {
            simpleBoss.SetProperlySpawned();
        }
        
        // Set boss properties based on level
        ScaleBossDifficulty();
        
        // Register for boss defeated event
        StartCoroutine(MonitorBossHealth());
    }
    
    private IEnumerator MonitorBossHealth()
    {
        // Wait until boss is defeated or null
        while (bossObject != null && bossDamagable != null && bossDamagable.Health > 0)
        {
            yield return new WaitForSeconds(0.5f);
            
            // Check if reference is still valid
            if (bossObject == null)
                break;
        }
        
        // If boss was defeated (not just destroyed)
        OnBossDefeated();
    }
    
    private void OnBossDefeated()
    {
        bossDefeated = true;
        
        // Unlock doors
        SetDoorsActive(true);
        
        // Notify game manager
        if (gameManager != null)
        {
            // End encounter and check for level progression
            gameManager.EndBossEncounter();
        }
    }
    
    private void ScaleBossDifficulty()
    {
        if (bossDamagable == null) return;
        
        // Scale boss stats based on level
        int bossStage = Mathf.Max(1, currentLevel / bossLevelInterval);
        
        // Attempt to scale boss health based on level
        // For MonoBehaviour, use SendMessage to call a method for scaling
        MonoBehaviour bossMono = bossObject.GetComponent<MonoBehaviour>();
        if (bossMono != null)
        {
            bossMono.SendMessage("ScaleBoss", bossStage, SendMessageOptions.DontRequireReceiver);
        }
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
        if (!bossDefeated)
        {
            // Use the current level from Game Manager if available
            if (gameManager != null)
            {
                currentLevel = gameManager.level_counter;
            }
            
            // Only initiate encounter if we're at the right level (multiple of interval and >= interval)
            if (currentLevel % bossLevelInterval == 0 && currentLevel >= bossLevelInterval)
            {
                InitiateEncounter(currentLevel);
            }
        }
    }
} 
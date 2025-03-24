using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossUI : MonoBehaviour
{
    [Header("References")]
    public BossEnemy boss;
    public Slider bossHealthSlider;
    public GameObject phasePanel;
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI bossNameText;
    
    [Header("Settings")]
    public string bossName = "DUNGEON KEEPER";
    public Color phase1TextColor = Color.white;
    public Color phase2TextColor = new Color(1f, 0.7f, 0);
    public Color phase3TextColor = new Color(1f, 0, 0);
    public float phaseNotificationDuration = 3f;
    
    [Header("Animation Settings")]
    public float healthBarFillSpeed = 5f;
    public float textFadeSpeed = 2f;
    
    private BossEnemy.BossPhase currentPhase;
    private Coroutine phaseNotificationCoroutine;
    
    void Start()
    {
        // Initialize UI elements
        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }
        
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = 1f;
        }
        
        if (phasePanel != null)
        {
            phasePanel.SetActive(false);
        }
        
        // Find boss if not assigned
        if (boss == null)
        {
            boss = FindObjectOfType<BossEnemy>();
        }
        
        // Register to phase change events
        if (boss != null)
        {
            currentPhase = boss.currentPhase;
            UpdatePhaseUI();
        }
    }
    
    void Update()
    {
        // Update health bar if boss is assigned
        if (boss != null && bossHealthSlider != null)
        {
            float targetValue = (float)boss.Health / boss.maxHealth;
            bossHealthSlider.value = Mathf.Lerp(bossHealthSlider.value, targetValue, Time.deltaTime * healthBarFillSpeed);
            
            // Check for phase changes
            if (currentPhase != boss.currentPhase)
            {
                currentPhase = boss.currentPhase;
                ShowPhaseTransition();
                UpdatePhaseUI();
            }
        }
    }
    
    void UpdatePhaseUI()
    {
        // Update phase indicator color
        if (phaseText != null)
        {
            switch (currentPhase)
            {
                case BossEnemy.BossPhase.Phase1:
                    phaseText.text = "PHASE 1";
                    phaseText.color = phase1TextColor;
                    break;
                case BossEnemy.BossPhase.Phase2:
                    phaseText.text = "PHASE 2";
                    phaseText.color = phase2TextColor;
                    break;
                case BossEnemy.BossPhase.Phase3:
                    phaseText.text = "PHASE 3";
                    phaseText.color = phase3TextColor;
                    break;
            }
        }
    }
    
    void ShowPhaseTransition()
    {
        // Show the phase transition notification
        if (phasePanel != null)
        {
            // Cancel any existing coroutine
            if (phaseNotificationCoroutine != null)
            {
                StopCoroutine(phaseNotificationCoroutine);
            }
            
            // Start new notification
            phaseNotificationCoroutine = StartCoroutine(PhaseNotificationRoutine());
        }
    }
    
    IEnumerator PhaseNotificationRoutine()
    {
        // Setup
        phasePanel.SetActive(true);
        CanvasGroup canvasGroup = phasePanel.GetComponent<CanvasGroup>();
        
        // If no CanvasGroup, add one
        if (canvasGroup == null)
        {
            canvasGroup = phasePanel.AddComponent<CanvasGroup>();
        }
        
        // Fade in
        float timer = 0f;
        while (timer < 0.5f)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / 0.5f);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        
        // Display phase text
        yield return new WaitForSeconds(phaseNotificationDuration);
        
        // Fade out
        timer = 0f;
        while (timer < 1f)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / 1f);
            timer += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        phasePanel.SetActive(false);
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        
        // Initial animations
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = 0f;
        }
        
        StartCoroutine(EntranceAnimation());
    }
    
    IEnumerator EntranceAnimation()
    {
        // Animate elements sliding in
        yield return new WaitForSeconds(1f);
        
        // Show phase notification
        ShowPhaseTransition();
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
} 
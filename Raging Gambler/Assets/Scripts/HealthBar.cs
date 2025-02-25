using UnityEngine;

public class HealthBar : MonoBehaviour
{

    private HealthController healthController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Setup(HealthController healthController)
    {
        this.healthController = healthController;
        healthController.OnHealthChanged += HealthController_OnHealthChanged;
        UpdateHealthBar();
    }

    private void HealthController_OnHealthChanged(object sender, System.EventArgs e)
    {
        UpdateHealthBar();

    }

    private void UpdateHealthBar()
    {
        float healthPercentage = (float)healthController.currentHealth / healthController.maxHealth;
        Transform healthBarTransform = transform.Find("Bar");
        healthBarTransform.localScale = new Vector3(healthPercentage, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
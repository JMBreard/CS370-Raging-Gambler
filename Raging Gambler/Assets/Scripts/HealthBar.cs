using System.Xml.Serialization;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    private HealthController healthController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // // Get the health bar (the actual bar object)
        // Transform healthBarTransform = transform.Find("Bar");

        // // Make sure it's found
        // if (healthBarTransform == null)
        // {
        //     Debug.LogError("No child named 'Bar' found in HealthBar object.");
        // }
        // else
        // {
        //     Setup(healthController, healthBarTransform);
        // }
    }

    public void Setup(HealthController healthController)
    {
        this.healthController = healthController;
        healthController.OnHealthChanged += HealthController_OnHealthChanged;
        UpdateHealthBar();
    }

    private void HealthController_OnHealthChanged(object sender, System.EventArgs e)
    {
        // Transform healthBarTransform = transform.Find("Bar");
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
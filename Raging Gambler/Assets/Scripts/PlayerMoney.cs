using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMoney : MonoBehaviour
{

    public int money;
    public TextMeshProUGUI moneyText;

    private HealthController healthController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        money = 100;
        UpdateMoneyText();
    }

    public void Setup(HealthController healthController)
    {
        this.healthController = healthController;
    }

    public void addMoney(int moneyToAdd)
    {
        Debug.Log("Money Added " +  moneyToAdd);
        money += moneyToAdd;
        UpdateMoneyText();
    }

    public void subtractMoney(int moneyToSubtract)
    {
        if (money - moneyToSubtract < 0)
        {
            Debug.Log("Not enough money.");
        }
        else
        {
            money -= moneyToSubtract;
            UpdateMoneyText();
        }
    }

    // Method to update the UI text displaying the money
    public void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: " + money.ToString();  // Display money in the text
        }
        else
        {
            Debug.LogError("Money Text is not assigned!");
        }
    }
}

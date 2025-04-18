using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;

public class RewardManager : MonoBehaviour
{
    public static RewardManager instance;

    public Rewards[] rewards;
    public PlayerMoney playerMoney;

    // References 
    public GameObject rewardUI;
    public Transform rewardContent;
    public GameObject rewardPrefab;
    public PlayerController player;
    public HealthController health;
    public GameManager gameManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        foreach (Rewards reward in rewards)
        {
            reward.cost = reward.baseCost;
        }

        foreach (Rewards reward in rewards)
        {
            GameObject item = Instantiate(rewardPrefab, rewardContent);

            reward.itemRef = item;

            foreach (Transform child in item.transform)
            {
                if (child.gameObject.name == "Name")
                {
                    child.gameObject.GetComponent<TMP_Text>().text = reward.name.ToString();
                }
                else if (child.gameObject.name == "Cost")
                {
                    child.gameObject.GetComponent<TMP_Text>().text = "Cost: $" + reward.cost.ToString();
                }
                else if (child.gameObject.name == "Image")
                {
                    child.gameObject.GetComponent<Image>().sprite = reward.image;
                }
            }

            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                BuyReward(reward);
            });
        }
    }

    public void UpdateRewardCosts()
    {
        int levelCounter = gameManager.level_counter;

        if (levelCounter <= 0) return;

        foreach (Rewards reward in rewards)
        {
            // Calculate cost based on base cost and scaling with level
            reward.cost = Mathf.RoundToInt(reward.baseCost * (1 + levelCounter * 1.5f / 10));

            // Update UI if the item reference exists
            if (reward.itemRef != null)
            {
                foreach (Transform child in reward.itemRef.transform)
                {
                    if (child.gameObject.name == "Cost")
                    {
                        child.gameObject.GetComponent<TMP_Text>().text = "Cost: $" + reward.cost.ToString();
                    }
                }
            }
        }
    }

    public void BuyReward(Rewards reward)
    {
        int currentMoney = playerMoney.money;
        if (currentMoney >= reward.cost && reward.canBuy)
        {
            playerMoney.subtractMoney(reward.cost);
            playerMoney.UpdateMoneyText();
            ApplyReward(reward);
        }
    }

    public void ApplyReward(Rewards reward)
    {
        switch (reward.name)
        {
            // works
            case "+1 Player Health":
                health.increaseCurrentHealth();
                break;

            // works
            case "+1 Max Health":
                health.increaseMaxHealth();
                break;

            // works
            case "+1 Ammo Count":
                player.increaseMaxAmmoCount();
                break;

            // works
            case "-1 Reload Time":
                player.decreaseReloadTime();
                break;

            // works
            case "+1 Speed":
                player.increaseSpeed();
                break;

            default:
                Debug.Log("no debuff available");
                break;
        }
    }

    public void SetCanBuy(string rewardName, bool value)
    {
        foreach (Rewards reward in rewards)
        {
            if (reward.name == rewardName)
            {
                reward.canBuy = value;
                return;
            }
        }
    }

    public void ToggleShop()
    {
        if (!rewardUI.activeSelf)
        {
            UpdateRewardCosts();
        }
        rewardUI.SetActive(!rewardUI.activeSelf);
    }

}

[System.Serializable]
public class Rewards
{
    public string name;
    public Sprite image;
    [HideInInspector] public int cost;
    public int baseCost;
    public bool canBuy = true;
    [HideInInspector] public GameObject itemRef;
}

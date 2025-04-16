using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (Rewards reward in rewards) 
        {
            GameObject item = Instantiate(rewardPrefab, rewardContent);

            reward.itemRef = item;

            foreach(Transform child in item.transform)
            {
                if (child.gameObject.name == "Name")
                {
                    child.gameObject.GetComponent<TMP_Text>().text = reward.name.ToString();
                } else if (child.gameObject.name == "Cost")
                {
                    child.gameObject.GetComponent<TMP_Text>().text = "Cost: $" + reward.cost.ToString();
                } else if (child.gameObject.name == "Image")
                {
                    child.gameObject.GetComponent<Image>().sprite = reward.image;
                }
            }

            item.GetComponent<Button>().onClick.AddListener(() => {
                BuyReward(reward);
                });
        }        
    }

    public void BuyReward(Rewards reward)
    {
        int currentMoney = playerMoney.money;
        if (currentMoney >= reward.cost && reward.canBuy) {
            playerMoney.subtractMoney(reward.cost);
            playerMoney.UpdateMoneyText();
            ApplyReward(reward);
        }
    }

    public void ApplyReward(Rewards reward) {
        switch(reward.name) {
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

            // bugged, increases the enemy dps instead of player dps
            case "Increased DPS":
                health.increaseDamage();
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
        rewardUI.SetActive(!rewardUI.activeSelf);
    }

}

[System.Serializable]
public class Rewards
{
    public string name;
    public int cost;
    public Sprite image;
    public bool canBuy = true;
    [HideInInspector] public GameObject itemRef;

    // public GameObject enemyPrefab;
    // public GameObject bulletPrefab;
    // public GameObject playerPrefab;
}

    
        
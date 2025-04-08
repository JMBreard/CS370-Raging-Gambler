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
        if (currentMoney >= reward.cost) {
            playerMoney.subtractMoney(reward.cost);
            playerMoney.UpdateMoneyText();
            ApplyReward(reward);
        }
    }

    public void ApplyReward(Rewards reward) {
        switch(reward.name) {
            // 
            case "+1 Player Health":
                health.increaseCurrentHealth();
                break;
            
            // 
            case "+1 Max Health":
                health.increaseMaxHealth();
                break;

            // 
            case "Increased Speed":
                player.increaseSpeed();
                break;

            // 
            case "Decreased Reload Time":
                player.decreaseReloadTime();
                break;

            //
            case "Increased DPS":
                player.increaseSpeed();
                break;

            //
            case "Decrease Time (5s)":
                player.increaseSpeed();
                break;
            
            //
            case "Decrease Time (10s)":
                player.increaseSpeed();
                break;
            
            //
            case "Decrease Time (15s)":
                player.increaseSpeed();
                break;

            //
            case "Decrease Time (20s)":
                player.increaseSpeed();
                break;

            default:
                Debug.Log("no debuff available");
                break;
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
    [HideInInspector] public GameObject itemRef;

    // public GameObject enemyPrefab;
    // public GameObject bulletPrefab;
    // public GameObject playerPrefab;
}

    
        
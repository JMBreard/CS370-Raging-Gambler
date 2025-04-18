using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GambleManager : MonoBehaviour
{

    public static GambleManager instance;

    // public int coins = 100;
    public PlayerMoney playerMoney;

    public Wagers[] wagers;

    // References 
    // public Text coinText;
    public GameObject shopUI;
    public Transform wagerContent;
    public GameObject wagerItem;
    public EnemySpawner enemySpawner;
    public PlayerController player;
    public HealthController health;
    public ProjectileMovement bullet;

    public GameObject NormalEnemyPrefab;
    public GameObject CurvedEnemyPrefab;
    public GameObject ShooterEnemyPrefab;
    public GameManager gameManager;

    public int[] WagerCounts = new int[7];

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

        foreach (Wagers wager in wagers)
        {
            wager.cost = wager.baseCost;
            wager.reward = wager.baseReward;
        }

        foreach (Wagers wager in wagers)
        {
            GameObject item = Instantiate(wagerItem, wagerContent);

            wager.itemRef = item;

            foreach (Transform child in item.transform)
            {
                if (child.gameObject.name == "Wager")
                {
                    child.gameObject.GetComponent<TMP_Text>().text = wager.name.ToString();
                }
                else if (child.gameObject.name == "Cost")
                {
                    child.gameObject.GetComponent<TMP_Text>().text = "Stake: $" + wager.cost.ToString();
                }
                else if (child.gameObject.name == "Reward")
                {
                    child.gameObject.GetComponent<TMP_Text>().text = "Win: $" + wager.reward.ToString();
                }
                else if (child.gameObject.name == "Image")
                {
                    child.gameObject.GetComponent<Image>().sprite = wager.image;
                }
            }

            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                BuyWager(wager);
            });
        }
    }

    public void UpdateWagers()
    {
        int levelCounter = gameManager.level_counter;

        if (levelCounter <= 0) return;

        float scale = 1;
        int num = Random.Range(0, 101);
        if (num < 5)
        {
            scale = 2f;
        }
        else
        {
            scale = 1.2f;
        }

        foreach (Wagers wager in wagers)
        {
            // Calculate cost and reward based on base cost and reward and scaling with level
            wager.cost = Mathf.RoundToInt(wager.baseCost * (1 + levelCounter * 1.1f / 10));
            wager.reward = Mathf.RoundToInt(wager.baseReward * (1 + levelCounter * scale / 10));

            // Update UI if the item reference exists
            if (wager.itemRef != null)
            {
                foreach (Transform child in wager.itemRef.transform)
                {
                    if (child.gameObject.name == "Cost")
                    {
                        child.gameObject.GetComponent<TMP_Text>().text = "Cost: $" + wager.cost.ToString();
                    }
                    else if (child.gameObject.name == "Reward")
                    {
                        child.gameObject.GetComponent<TMP_Text>().text = "Win: $" + wager.reward.ToString();
                    }
                }
            }
        }
    }

    public void BuyWager(Wagers wager)
    {
        int currentMoney = playerMoney.money;
        if (currentMoney >= wager.cost && wager.canBuy)
        {
            playerMoney.subtractMoney(wager.cost);
            playerMoney.UpdateMoneyText();
            ApplyWager(wager);
        }

    }

    public void ApplyWager(Wagers wager)
    {
        switch (wager.name)
        {
            // need to test
            case "Enemy: damage buff":
                health.increaseDamage();
                break;

            // works
            // enemy is EnemySpawner instance
            case "Enemy: population buff":
                enemySpawner.increaseSpawnRate();
                WagerCounts[0] += 1;
                break;

            // works
            // enemy is EnemySpawner instance
            case "Enemy: health buff":
                enemySpawner.addEnemyHealth();
                WagerCounts[1] += 1;
                break;

            // works
            // player is PlayerController instance
            case "Player: reload debuff":
                player.increaseReloadTime();
                WagerCounts[2] += 1;
                break;

            // works
            // player is PlayerController instance
            case "Player: ammo count debuff":
                player.decreaseMaxAmmoCount();
                WagerCounts[3] += 1;
                break;

            // works
            // health is HealthController instance
            case "Player: health debuff":
                health.reduceMaxHealth();
                WagerCounts[4] += 1;
                break;

            // works
            // player is PlayerController instance
            case "Player: speed debuff":
                player.reduceSpeed();
                WagerCounts[5] += 1;
                break;

            // bugged, bullet time doesn't reset after restart
            // bullet is ProjectileMovement instance

            default:
                Debug.Log("no debuff available");
                break;
        }
    }

    public void SetCanBuy(string wagerName, bool value)
    {
        foreach (Wagers wager in wagers)
        {
            if (wager.name == wagerName)
            {
                wager.canBuy = value;
                Debug.Log("canBuy = " + wager.canBuy);
                return;
            }
        }
    }

    public void ToggleShop()
    {
        if (!shopUI.activeSelf)
        {
            UpdateWagers();
        }
        shopUI.SetActive(!shopUI.activeSelf);
    }
}

[System.Serializable]
public class Wagers
{
    public string name;
    public int cost;
    public int baseCost;
    public int reward;
    public int baseReward;
    public Sprite image;
    public bool canBuy = true;
    [HideInInspector] public GameObject itemRef;
}

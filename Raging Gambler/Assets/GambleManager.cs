using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering.Universal;

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
    public EnemySpawner enemy;
    public PlayerController player;
    public HealthController health;
    public ProjectileMovement bullet;

    public GameObject NormalEnemyPrefab;
    public GameObject CurvedEnemyPrefab;
    public GameObject ShooterEnemyPrefab;

    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        // DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        foreach (Wagers wager in wagers) 
        {
            GameObject item = Instantiate(wagerItem, wagerContent);

            wager.itemRef = item;

            foreach(Transform child in item.transform)
            {
                if (child.gameObject.name == "Wager")
                {
                    child.gameObject.GetComponent<TMP_Text>().text = wager.name.ToString();
                } else if (child.gameObject.name == "Cost")
                {
                    child.gameObject.GetComponent<TMP_Text>().text = "Stake: $" + wager.cost.ToString();
                } else if (child.gameObject.name == "Reward")
                {
                    child.gameObject.GetComponent<TMP_Text>().text = "Win: $" + wager.cost.ToString();
                } else if (child.gameObject.name == "Image")
                {
                    child.gameObject.GetComponent<Image>().sprite = wager.image;
                }
            }

            item.GetComponent<Button>().onClick.AddListener(() => {
                BuyWager(wager);
                });
        }
    }

    public void BuyWager(Wagers wager) {
        int currentMoney = playerMoney.money;
        if (currentMoney >= wager.cost) {
            playerMoney.subtractMoney(wager.cost);
            playerMoney.UpdateMoneyText();
            ApplyWager(wager);
        }
        
    }

    public void ApplyWager(Wagers wager) {
        switch(wager.name) {
            // works
            // enemy is EnemySpawner instance
            case "Enemy: population buff":
                enemy.increaseSpawnRate();
                break;

            // bugged: enemy health doesn't reset even after stopping/restarting scene
            // enemy is EnemySpawner instance
            case "Enemy: health buff":
                enemy.setEnemyHealthMultiplier(2);
                HealthController normalEnemyHealth = NormalEnemyPrefab.GetComponent<HealthController>();
                normalEnemyHealth.increaseMaxHealth();
                HealthController curvedEnemyHealth = CurvedEnemyPrefab.GetComponent<HealthController>();
                curvedEnemyHealth.increaseMaxHealth();
                HealthController shooterEnemyHealth = ShooterEnemyPrefab.GetComponent<HealthController>();
                shooterEnemyHealth.increaseMaxHealth();
                break;

            // works
            // player is PlayerController instance
            case "Player: reload debuff":
                player.increaseReloadTime();
                break;
            
            // works
            // player is PlayerController instance
            case "Player: ammo count debuff":
                player.decreaseMaxAmmoCount();
                break;

            // works
            // health is HealthController instance
            case "Player: health debuff":
                health.reduceMaxHealth();
                break;

            // works
            // player is PlayerController instance
            case "Player: speed debuff":
                player.reduceSpeed();
                break;

            // bugged, bullet time doesn't reset after restart
            // bullet is ProjectileMovement instance
            case "Player: range debuff":
                bullet.reduceBulletTime();
                break;
            default:
                Debug.Log("no debuff available");
                break;
        }
    }

    public void ToggleShop() 
    {
        shopUI.SetActive(!shopUI.activeSelf);
    }

    // private void OnGUI() {
    //     coinText.text = "Coins: " + coins.ToString();
    // }
}

[System.Serializable]
public class Wagers {
    public string name;
    public int cost;
    public int reward;
    public Sprite image;
    // [HideInInspector] public int quantity;
    [HideInInspector] public GameObject itemRef;
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GambleManager : MonoBehaviour
{

    public static GambleManager instance;

    // public int coins = 100;

    public Wagers[] wagers;

    // References 
    // public Text coinText;
    public GameObject shopUI;
    public Transform wagerContent;
    public GameObject wagerItem;
    public PlayerController player;

    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
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

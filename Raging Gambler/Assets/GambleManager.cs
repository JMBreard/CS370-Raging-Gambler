using UnityEngine;
using UnityEngine.UI;

public class GambleManager : MonoBehaviour
{

    public static GambleManager instance;

    public int coins = 100;

    // public Upgrade[] upgrades;

    // References 
    public GameObject shopUI;

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

    public void ToggleShop() 
    {
        shopUI.SetActive(!shopUI.activeSelf);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;

public class RewardShopTrigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RewardManager.instance.ToggleShop();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RewardManager.instance.ToggleShop();
        }
    }
}
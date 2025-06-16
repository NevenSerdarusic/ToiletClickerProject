using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private FoodPoolManager foodPoolManager;

    [Header("Shop Setup")]
    [SerializeField] private List<FoodItem> allShopItems;
    [SerializeField] private List<ShopItemUI> shopButtons;
    

    private HashSet<FoodItem> purchasedItems = new HashSet<FoodItem>();

    private void Start()
    {
        PopulateShop();
    }

    private void Update()
    {
        UpdateButtonStates();
    }

    private void PopulateShop()
    {
        var availableItems = new List<FoodItem>(allShopItems);

        foreach (var button in shopButtons)
        {
            if (availableItems.Count == 0) break;

            int index = Random.Range(0, availableItems.Count);
            var item = availableItems[index];
            button.AssignItem(item, TryPurchaseItem);

            availableItems.RemoveAt(index);
        }
    }

    private void UpdateButtonStates()
    {
        foreach (var button in shopButtons)
        {
            if (button.HasItem)
                button.SetInteractable(gameManager.GetTotalCoins() >= button.GetItem().cost);
        }
    }

    private void TryPurchaseItem(FoodItem item, ShopItemUI button)
    {
        if (gameManager.GetTotalCoins() < item.cost) return;

        gameManager.SpendCoins(item.cost);
        purchasedItems.Add(item);

        // Ubacivanje healthy food-a
        foodPoolManager.ReplaceFirstUnhealthyFoodWithHealthy(item);


        // Nova random ponuda
        var remaining = allShopItems.Except(purchasedItems).ToList();
        if (remaining.Count > 0)
        {
            var newItem = remaining[Random.Range(0, remaining.Count)];
            button.AssignItem(newItem, TryPurchaseItem);
        }
        else
        {
            button.Clear(); // shop prazan
        }
    }

}

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
    [SerializeField] private List<FoodItem> allShopItems; //SO Library Healthy Food
    [SerializeField] private List<ShopItemUI> shopButtons; //All available buttons in Shop

    //Track purchased items so that the same item that was just purchased is not added to the shop
    private HashSet<FoodItem> purchasedItems = new HashSet<FoodItem>();

    private void Start()
    {
        PopulateShop();
        gameManager.OnCoinsChanged += HandleCoinsChanged; //Event subscription
        HandleCoinsChanged(gameManager.GetTotalCoins()); //Initial check of interactable buttons in Shop
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

    private void HandleCoinsChanged(int currentCoins)
    {
        UpdateButtonStates();
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
        //if don't have enough coins for the purchase, return it
        if (gameManager.GetTotalCoins() < item.cost) return;

        //if  have enough coins for the purchase, subtract from the total amount
        gameManager.SpendCoins(item.cost);
        purchasedItems.Add(item);

        //Repalcing junk food in foodSlot with healthy food
        foodPoolManager.ReplaceFirstUnhealthyFoodWithHealthy(item);

        //Add new random item in shop, except the same one
        var remaining = allShopItems.Except(purchasedItems).ToList();
        if (remaining.Count > 0)
        {
            var newItem = remaining[Random.Range(0, remaining.Count)];
            button.AssignItem(newItem, TryPurchaseItem);
        }
        else
        {
            button.Clear();
        }
    }

}

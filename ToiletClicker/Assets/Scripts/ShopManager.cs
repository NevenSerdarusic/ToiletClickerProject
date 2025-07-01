using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private FoodPoolManager foodPoolManager;

    [Header("Shop Setup")]
    [SerializeField] private List<FoodItem> allShopItems; //SO Library Healthy Food
    [SerializeField] private List<ShopItemUI> shopButtons; //All available buttons in Shop

    //Track purchased items so that the same item that was just purchased is not added to the shop
    private readonly HashSet<FoodItem> purchasedItems = new HashSet<FoodItem>();

    private void Start()
    {
        PopulateShop();
        gameManager.OnCoinsChanged += HandleCoinsChanged; //Event subscription
        HandleCoinsChanged(gameManager.GetTotalCoins()); //Initial check of interactable buttons in Shop
    }

    private void PopulateShop()
    {
        var displayedItems = new HashSet<FoodItem>();

        //Take all available items except one we already purchased from shop
        var availableItems = allShopItems.Except(purchasedItems).ToList();

        //Order by price of food
        availableItems = availableItems.OrderBy(item => item.cost).ToList();

        for (int i = 0; i < shopButtons.Count; i++)
        {
            FoodItem itemToAssign = null;

            //Find firt one taht is not already been shown
            foreach (var item in availableItems)
            {
                if (!displayedItems.Contains(item))
                {
                    itemToAssign = item;
                    displayedItems.Add(item);
                    break;
                }
            }

            if (itemToAssign != null)
            {
                shopButtons[i].AssignItem(itemToAssign, TryPurchaseItem);
            }
            else
            {
                shopButtons[i].Clear();
            }
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
        if (gameManager.GetTotalCoins() < item.cost) return;

        gameManager.SpendCoins(item.cost);
        purchasedItems.Add(item);

        foodPoolManager.ReplaceFirstUnhealthyFoodWithHealthy(item);

        //Get all currently displayerd items
        var currentlyDisplayed = Enumerable.ToHashSet(
                shopButtons.Where(b => b.HasItem && b != button)
               .Select(b => b.GetItem())
                );

        var remaining = allShopItems
            .Except(purchasedItems)
            .Except(currentlyDisplayed)
            .ToList();

        if (remaining.Count > 0)
        {
            remaining = remaining.OrderBy(i => i.cost).ToList();

            int lowestCost = remaining[0].cost;
            var cheapestItems = remaining.Where(i => i.cost == lowestCost).ToList();

            var newItem = cheapestItems[Random.Range(0, cheapestItems.Count)];
            button.AssignItem(newItem, TryPurchaseItem);
        }
        else
        {
            button.Clear();
        }

        UpdateButtonStates(); //refresh interactability
    }

    public List<FoodItem> GetAllShopItems()
    {
        return allShopItems;
    }
}

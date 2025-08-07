using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private EncryptedDataPoolManager encryptedDataPoolManager;

    [Header("Shop Setup")]
    [SerializeField] private List<CodeFragment> allShopItems; //SO Library Healthy Food
    [SerializeField] private List<ShopItemUI> shopButtons; //All available buttons in Shop

    //Track purchased items so that the same item that was just purchased is not added to the shop
    private readonly HashSet<CodeFragment> purchasedItems = new HashSet<CodeFragment>();

    private void Start()
    {
        PopulateShop();
        gameManager.OnCoinsChanged += HandleCoinsChanged; //Event subscription
        HandleCoinsChanged(gameManager.GetTotalCoins()); //Initial check of interactable buttons in Shop
    }

    private void PopulateShop()
    {
        var displayedItems = new HashSet<CodeFragment>();

        //Take all available items except one we already purchased from shop
        var availableItems = allShopItems.Except(purchasedItems).ToList();

        //Order by price of food
        availableItems = availableItems.OrderBy(item => item.cost).ToList();

        for (int i = 0; i < shopButtons.Count; i++)
        {
            CodeFragment itemToAssign = null;

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

    private void TryPurchaseItem(CodeFragment item, ShopItemUI button)
    {
        if (gameManager.GetTotalCoins() < item.cost) return;

        gameManager.SpendCoins(item.cost);
        purchasedItems.Add(item);
        encryptedDataPoolManager.ReplaceFirstEncryptedCodeWithDecoded(item);

        // Skupi sve trenutno prikazane
        var displayed = shopButtons
            .Where(b => b.HasItem)
            .Select(b => b.GetItem())
            .ToHashSet();

        // Funkcija koja dohva?a sljede?i item
        CodeFragment GetNext()
        {
            return allShopItems
                .Except(purchasedItems)
                .Except(displayed)
                .OrderBy(i => i.cost)
                .FirstOrDefault();
        }

        // Pokušaj na?i nextItem
        var nextItem = GetNext();

        // Ako nema više novih, "resetiraj shop" te ponovno uzmi najjeftinije
        if (nextItem == null)
        {
            purchasedItems.Clear();
            nextItem = GetNext();
        }

        // Ako i dalje nema (vrlo neuobi?ajeno, zna?i da shopButtons > allShopItems), o?isti
        if (nextItem != null)
            button.AssignItem(nextItem, TryPurchaseItem);
        else
            button.Clear();

        UpdateButtonStates();
    }


    public List<CodeFragment> GetAllShopItems()
    {
        return allShopItems;
    }
}

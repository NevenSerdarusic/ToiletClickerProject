using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private FoodPoolManager foodPoolManager;
    [SerializeField] private WeightManager weightManager;
    [SerializeField] private ClickTarget clickTarget;
    [SerializeField] private PreassureSystem preassureSystem;

    [Header("Upgrade Settings")]
    [SerializeField] private List<UpgradeData> allUpgrades; //SO Library Upgrade

    [Header("UI Setup")]
    [SerializeField] private UpgradeItemUI upgradeButtonPrefab;
    [SerializeField] private UpgradeCardUI upgradeCardUIPrefab;
    [SerializeField] private Transform upgradesShopContainer;
    [SerializeField] private Transform upgradesInstructionContainer;
    [SerializeField] private float spacingBetweenInstantiatedPrefabs = 100f;

    //private readonly Dictionary<UpgradeType, Coroutine> activeUpgrades = new();
    private readonly List<UpgradeItemUI> upgradeButtons = new();

    [Header("Event Listener")]
    [SerializeField] private TimeCounterEvent timecounterEvent;

    private void OnEnable()
    {
        timecounterEvent.OnEventRaised += CallAllUpgradeEffects;
    }

    private void OnDisable()
    {
        timecounterEvent.OnEventRaised -= CallAllUpgradeEffects;
    }


    private void Start()
    {
        PopulateUpgrades();
        //gameManager.OnXPChanged += HandleXPChanged; //Event subscription
        //HandleXPChanged(gameManager.GetTotalXP()); //Initial check of interactable buttons in Shop
    }

    private void PopulateUpgrades()
    {
        // O?isti stare buttone
        foreach (Transform child in upgradesShopContainer)
            Destroy(child.gameObject);
        upgradeButtons.Clear();

        int currentXP = gameManager.GetTotalXP();

        // Sortiraj po cijeni (najjeftiniji prvi)
        var sortedUpgrades = allUpgrades.OrderBy(u => u.upgradePrice).ToList();

        for (int i = 0; i < sortedUpgrades.Count; i++)
        {
            var upgrade = sortedUpgrades[i];
            var button = Instantiate(upgradeButtonPrefab, upgradesShopContainer);
           
            // Pomakni poziciju gumba vertikalno
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -i * spacingBetweenInstantiatedPrefabs);

            button.AssignUpgrade(upgrade, TryPurchaseUpgrade);

            bool isAvailable = currentXP >= upgrade.upgradePrice;

            upgradeButtons.Add(button);

            var card = Instantiate(upgradeCardUIPrefab, upgradesInstructionContainer);
            var cardUI = card.GetComponent<UpgradeCardUI>();
            cardUI.AssignUpgrade(upgrade, null);
        }
    }



    //private void HandleXPChanged(int currentXP)
    //{
    //    UpdateButtonStates();
    //}

    //private void UpdateButtonStates()
    //{
    //    //int currentXP = gameManager.GetTotalXP();

    //    //foreach (var button in upgradeButtons)
    //    //{
    //    //    if (button.HasUpgrade)
    //    //        button.SetInteractable(gameManager.GetTotalXP() >= button.GetUpgrade().upgradePrice);
    //    //}
    //}


    public void TryPurchaseUpgrade(UpgradeData upgrade, UpgradeItemUI button)
    {
      
        if (upgrade == null || gameManager.GetTotalCoins() < upgrade.upgradePrice) return;

        CheckIfPurchaseIsStaticUpgrades(upgrade);
        gameManager.SpendCoins(upgrade.upgradePrice);
        upgrade.upgradeLevel = upgrade.upgradeLevel + 1;

        Debug.Log("Try Purchase Upgrade " + upgrade + " and upgrade level is" + upgrade.upgradeLevel);
        //UpdateButtonStates();
    }


   


    private void AutoTap(int upgradeLevel)
    {
        int calculateAmountOfCoins = upgradeLevel * 5;
        gameManager.AddCoins(calculateAmountOfCoins);
    }

    private void IncreaseTapValue(int amount)
    {
        gameManager.ChangeTapValue(amount);
    }


    private void CallAllUpgradeEffects()
    {
        foreach(var upgrade in allUpgrades)
        {
            ApplyUpgradeEffect(upgrade);
        }
    }
    private void ApplyUpgradeEffect(UpgradeData upgrade)
    {
        if (upgrade.type == UpgradeType.AutoTap)
        {
            AutoTap(upgrade.upgradeLevel);
        }
       
    }

    private void CheckIfPurchaseIsStaticUpgrades(UpgradeData upgrade)//za provjeravanje staticnog upgradea(doubledip)
    {
        switch (upgrade.type)
        {
            case UpgradeType.DoubleDip:
                IncreaseTapValue(upgrade.upgradeLevel);
                break;
        }

    }

    //private void ApplyUpgradeEffect(UpgradeData upgrade)
    //{
    //    if(upgrade.type ==UpgradeType.AutoTap)
    //    {
    //        AutoTap(upgrade.upgradeLevel);
    //    }
    //    //switch (upgrade.type)
    //    //{
    //    //    case UpgradeType.DoubleDip:
    //    //        clickTarget.SetClickMultiplier(gameConfig.duobleTapMultiplier);
    //    //        break;
    //    //    case UpgradeType.QuintupleClick:
    //    //        clickTarget.SetClickMultiplier(gameConfig.megaTapMultiplier);
    //    //        break;
    //    //    case UpgradeType.MegaLaxLaunch:
    //    //        weightManager.SubtractWeight(gameConfig.laxativeWeightReduction);
    //    //        break;
    //    //    case UpgradeType.HealthSwap:
    //    //        foodPoolManager.ReplaceJunkWithHealthy(gameConfig.detoxBombReplacingSlotCount);
    //    //        break;
    //    //    case UpgradeType.SnackDecelerator:
    //    //        foodPoolManager.SlowScroll(true);
    //    //        break;
    //    //    //case UpgradeType.AutoTap:
    //    //    //    clickTarget.EnableAutoClick(true);
    //    //    //    break;
    //    //    case UpgradeType.FiberFirepower:
    //    //        foodPoolManager.BoostFiberInHealthy(true);
    //    //        break;
    //    //    case UpgradeType.LightweightJunk:
    //    //        foodPoolManager.ReduceJunkNutrition(true);
    //    //        break;
    //    //    case UpgradeType.RapidRelief:
    //    //        preassureSystem.PreassureDecreaseBoost(true);
    //    //        break;
    //    //    case UpgradeType.PreasureBrake:
    //    //        preassureSystem.PreassurePerClickBoost(true);
    //    //        break;
    //    //    case UpgradeType.ScaleSmasher:
    //    //        preassureSystem.WeightPerClickDrop(true);
    //    //        break;
    //    //}
    //}

    //private void RemoveUpgradeEffect(UpgradeData upgrade)
    //{
    //    switch (upgrade.type)
    //    {
    //        case UpgradeType.DoubleDip:
    //        case UpgradeType.QuintupleClick:
    //            clickTarget.SetClickMultiplier(1f);
    //            break;
    //        case UpgradeType.SnackDecelerator:
    //            foodPoolManager.SlowScroll(false);
    //            break;
    //        //case UpgradeType.AutoTap:
    //        //    clickTarget.EnableAutoClick(false);
    //        //    break;
    //        case UpgradeType.FiberFirepower:
    //            foodPoolManager.BoostFiberInHealthy(false);
    //            break;
    //        case UpgradeType.LightweightJunk:
    //            foodPoolManager.ReduceJunkNutrition(false);
    //            break;
    //        case UpgradeType.RapidRelief:
    //            preassureSystem.PreassureDecreaseBoost(false);
    //            break;
    //        case UpgradeType.PreasureBrake:
    //            preassureSystem.PreassurePerClickBoost(false);
    //            break;
    //        case UpgradeType.ScaleSmasher:
    //            preassureSystem.WeightPerClickDrop(false);
    //            break;
    //    }

    //    if (activeUpgrades.ContainsKey(upgrade.type))
    //        activeUpgrades.Remove(upgrade.type);
    //}


    //In case of game over status, reset all upgrades
    public void ResetAllUpgrades()
    {
        //// Zaustavi sve aktivne upgradeove i ukloni efekte
        //foreach (var kvp in activeUpgrades)
        //{
        //    StopCoroutine(kvp.Value);

        //    var upgrade = allUpgrades.FirstOrDefault(u => u.type == kvp.Key);
        //    if (upgrade != null)
        //    {
        //        RemoveUpgradeEffect(upgrade);
        //    }
        //}

        //activeUpgrades.Clear();

        // Resetiraj rucno efekte koji možda nisu u coroutine-u (npr. instantne) DA li je ovo potrebno imati tu??
        //clickTarget.SetClickMultiplier(1f);
        //clickTarget.EnableAutoClick(false);
        foodPoolManager.SlowScroll(false);
        foodPoolManager.BoostFiberInHealthy(false);
        foodPoolManager.ReduceJunkNutrition(false);
    }


    //Retrieving all upgrade buttons
    public List<UpgradeItemUI> GetAllUpgradeButons()
    {
        return upgradeButtons;
    }

}

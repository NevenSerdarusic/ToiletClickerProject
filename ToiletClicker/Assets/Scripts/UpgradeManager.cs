using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

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
    [SerializeField] private List<UpgradeData> allUpgrades;

    [Header("UI Setup")]
    [SerializeField] private UpgradeItemUI upgradeButtonPrefab;
    [SerializeField] private Transform upgradesContainer;
    [SerializeField] private float verticalSpacing = 100f; //Spacing between upgrade buttons in content 


    private readonly Dictionary<UpgradeType, Coroutine> activeUpgrades = new();
    private readonly List<UpgradeItemUI> instantiatedButtons = new();

    private void Start()
    {
        PopulateUpgrades();
        gameManager.OnXPChanged += HandleXPChanged;
        HandleXPChanged(gameManager.GetTotalXP());
    }

    private void PopulateUpgrades()
    {
        // O?isti stare buttone
        foreach (Transform child in upgradesContainer)
            Destroy(child.gameObject);
        instantiatedButtons.Clear();

        int currentXP = gameManager.GetTotalXP();

        // Sortiraj po cijeni (najjeftiniji prvi)
        var sortedUpgrades = allUpgrades.OrderBy(u => u.upgradePrice).ToList();

        for (int i = 0; i < sortedUpgrades.Count; i++)
        {
            var upgrade = sortedUpgrades[i];
            var button = Instantiate(upgradeButtonPrefab, upgradesContainer);

            // Pomakni poziciju gumba vertikalno
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -i * verticalSpacing);

            button.AssignUpgrade(upgrade, TryPurchaseUpgrade);

            bool isAvailable = currentXP >= upgrade.upgradePrice;
            button.SetInteractable(isAvailable);
            button.ShowLockedOverlay(!isAvailable);

            instantiatedButtons.Add(button);
        }

        // Opcionalno: Prilagodi visinu containera
        RectTransform containerRect = upgradesContainer.GetComponent<RectTransform>();
        float totalHeight = sortedUpgrades.Count * verticalSpacing;
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, totalHeight);
    }



    private void HandleXPChanged(int currentXP)
    {
        UpdateUpgradeButtonStates();
    }

    private void UpdateUpgradeButtonStates()
    {
        int currentXP = gameManager.GetTotalXP();

        foreach (var button in instantiatedButtons)
        {
            var upgrade = button.GetUpgrade();
            bool isAvailable = currentXP >= upgrade.upgradePrice;
            button.SetInteractable(isAvailable);
            button.ShowLockedOverlay(!isAvailable);
        }
    }


    public void TryPurchaseUpgrade(UpgradeData upgrade, UpgradeItemUI button)
    {
        if (upgrade == null || gameManager.GetTotalXP() < upgrade.upgradePrice) return;

        gameManager.SpendXP(upgrade.upgradePrice);

        if (upgrade.isInstant)
        {
            ApplyUpgradeEffect(upgrade);
        }
        else
        {
            if (activeUpgrades.ContainsKey(upgrade.type))
                StopCoroutine(activeUpgrades[upgrade.type]);

            //Start upgrade timer
            uiManager.StartUpgradeTimer(upgrade.upgradeDuration);

            Coroutine co = StartCoroutine(HandleTimedUpgrade(upgrade));
            activeUpgrades[upgrade.type] = co;
        }

        HandleXPChanged(gameManager.GetTotalXP()); // Refresh UI
    }


    //Active upgrades counter
    private IEnumerator HandleTimedUpgrade(UpgradeData upgrade)
    {
        ApplyUpgradeEffect(upgrade);
        yield return new WaitForSeconds(upgrade.upgradeDuration);
        RemoveUpgradeEffect(upgrade);
    }

    private void ApplyUpgradeEffect(UpgradeData upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.DoubleTap:
                clickTarget.SetClickMultiplier(gameConfig.duobleTapMultiplier);
                break;
            case UpgradeType.MegaTap:
                clickTarget.SetClickMultiplier(gameConfig.megaTapMultiplier);
                break;
            case UpgradeType.FastFlush:
                weightManager.SubtractWeight(gameConfig.laxativeWeightReduction);
                break;
            case UpgradeType.DetoxBomb:
                foodPoolManager.ReplaceJunkWithHealthy(gameConfig.detoxBombReplacingSlotCount);
                break;
            case UpgradeType.SnackLag:
                foodPoolManager.SlowScroll(true);
                break;
            case UpgradeType.AutoTap:
                clickTarget.EnableAutoClick(true);
                break;
            case UpgradeType.FullFlush:
                foodPoolManager.BoostFiberInHealthy(true);
                break;
            case UpgradeType.MegaBurn:
                foodPoolManager.ReduceJunkNutrition(true);
                break;
            case UpgradeType.PressDrop:
                preassureSystem.PreassureDecreaseBoost(true);
                break;
            case UpgradeType.DePress:
                preassureSystem.PreassurePerClickBoost(true);
                break;
            case UpgradeType.TapBurn:
                preassureSystem.WeightPerClickDrop(true);
                break;
        }
    }

    private void RemoveUpgradeEffect(UpgradeData upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.DoubleTap:
            case UpgradeType.MegaTap:
                clickTarget.SetClickMultiplier(1f);
                break;
            case UpgradeType.SnackLag:
                foodPoolManager.SlowScroll(false);
                break;
            case UpgradeType.AutoTap:
                clickTarget.EnableAutoClick(false);
                break;
            case UpgradeType.FullFlush:
                foodPoolManager.BoostFiberInHealthy(false);
                break;
            case UpgradeType.MegaBurn:
                foodPoolManager.ReduceJunkNutrition(false);
                break;
            case UpgradeType.PressDrop:
                preassureSystem.PreassureDecreaseBoost(false);
                break;
            case UpgradeType.DePress:
                preassureSystem.PreassurePerClickBoost(false);
                break;
            case UpgradeType.TapBurn:
                preassureSystem.WeightPerClickDrop(false);
                break;
        }

        if (activeUpgrades.ContainsKey(upgrade.type))
            activeUpgrades.Remove(upgrade.type);
    }


    //In case of game over status, reset all upgrades
    public void ResetAllUpgrades()
    {
        // Zaustavi sve aktivne upgradeove i ukloni efekte
        foreach (var kvp in activeUpgrades)
        {
            StopCoroutine(kvp.Value);

            var upgrade = allUpgrades.FirstOrDefault(u => u.type == kvp.Key);
            if (upgrade != null)
            {
                RemoveUpgradeEffect(upgrade);
            }
        }

        activeUpgrades.Clear();

        // Resetiraj ru?no efekte koji možda nisu u coroutine-u (npr. instantne) DA li je ovo potrebno imati tu??
        clickTarget.SetClickMultiplier(1f);
        clickTarget.EnableAutoClick(false);
        foodPoolManager.SlowScroll(false);
        foodPoolManager.BoostFiberInHealthy(false);
        foodPoolManager.ReduceJunkNutrition(false);
    }


    //Retrieving all upgrade buttons
    public List<UpgradeItemUI> GetAllUpgradeButons()
    {
        return instantiatedButtons;
    }

}

using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private EncryptedDataPoolManager encryptedDataPoolManager;
    [SerializeField] private FirewallManager firewallManager;
    [SerializeField] private ClickTarget clickTarget;
    [SerializeField] private TraceScannerManager traceScannerManager;

    [Header("Upgrade Settings")]
    [SerializeField] private List<UpgradeData> allUpgrades; //SO Library Upgrade

    [Header("UI Setup")]
    [SerializeField] private UpgradeItemUI upgradeButtonPrefab;
    [SerializeField] private UpgradeCardUI upgradeCardUIPrefab;
    [SerializeField] private Transform upgradesShopContainer;
    [SerializeField] private Transform upgradesInstructionContainer;
    [SerializeField] private float spacingBetweenInstantiatedPrefabs = 100f;


    private readonly Dictionary<UpgradeType, Coroutine> activeUpgrades = new();
    private readonly List<UpgradeItemUI> upgradeButtons = new();


    private void Start()
    {
        PopulateUpgrades();
        gameManager.OnXPChanged += HandleXPChanged; //Event subscription
        HandleXPChanged(gameManager.GetTotalXP()); //Initial check of interactable buttons in Shop
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



    private void HandleXPChanged(int currentXP)
    {
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        foreach (var button in upgradeButtons)
        {
            if (button.HasUpgrade)
            {
                bool canBuy = gameManager.GetTotalXP() >= button.GetUpgrade().upgradePrice;

                // Postavi interactable
                button.SetInteractable(canBuy);

                // Dohvati SVE TMP_Text komponente unutar buttona
                TMP_Text[] texts = button.GetComponentsInChildren<TMP_Text>(true);
                foreach (var txt in texts)
                {
                    txt.color = canBuy ? uiManager.DefaultTextColor : uiManager.GameMainTextColor;
                }
            }
        }
    }



    public void TryPurchaseUpgrade(UpgradeData upgrade, UpgradeItemUI button)
    {
        if (upgrade == null || gameManager.GetTotalXP() < upgrade.upgradePrice) return;

        gameManager.SpendXP(upgrade.upgradePrice);

        UpdateButtonStates();

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
            case UpgradeType.DualTap:
                clickTarget.SetClickMultiplier(gameConfig.Current.doubleTapMultiplier);
                break;
            case UpgradeType.MegaTap:
                clickTarget.SetClickMultiplier(gameConfig.Current.megaTapMultiplier);
                break;
            case UpgradeType.FirewallShock:
                firewallManager.SubtractFirewallProtection(gameConfig.Current.firewallProtectionDecreaseImpact);//INSTANT
                break;
            case UpgradeType.ProxySwap:
                encryptedDataPoolManager.ReplaceEncryptedWithDecoded(gameConfig.Current.codeReplacingSlotCount);//INSTANT
                break;
            case UpgradeType.CodeFreeze:
                encryptedDataPoolManager.SlowScroll(true);
                break;
            case UpgradeType.BotPress:
                clickTarget.EnableAutoClick(true);
                traceScannerManager.DisableTrace();
                break;
            case UpgradeType.ExploitMax:
                encryptedDataPoolManager.BoostCodeOptimization(true);
                break;
            case UpgradeType.ImpactDrop:
                encryptedDataPoolManager.ReduceEncryptedCodeSensibility(true);
                break;
            case UpgradeType.TraceDrop:
                traceScannerManager.DetectionDecreaseBoost(true);
                break;
            case UpgradeType.GhostTrace:
                traceScannerManager.DetectionPerClickBoost(true);
                break;
            case UpgradeType.SmackDown:
                traceScannerManager.FirewallPerClickDecrease(true);
                break;
        }
    }

    private void RemoveUpgradeEffect(UpgradeData upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.DualTap:
            case UpgradeType.MegaTap:
                clickTarget.SetClickMultiplier(1f);
                break;
            case UpgradeType.CodeFreeze:
                encryptedDataPoolManager.SlowScroll(false);
                break;
            case UpgradeType.BotPress:
                clickTarget.EnableAutoClick(false);
                traceScannerManager.EnableTrace();
                break;
            case UpgradeType.ExploitMax:
                encryptedDataPoolManager.BoostCodeOptimization(false);
                break;
            case UpgradeType.ImpactDrop:
                encryptedDataPoolManager.ReduceEncryptedCodeSensibility(false);
                break;
            case UpgradeType.TraceDrop:
                traceScannerManager.DetectionDecreaseBoost(false);
                break;
            case UpgradeType.GhostTrace:
                traceScannerManager.DetectionPerClickBoost(false);
                break;
            case UpgradeType.SmackDown:
                traceScannerManager.FirewallPerClickDecrease(false);
                break;
        }

        if (activeUpgrades.ContainsKey(upgrade.type))
            activeUpgrades.Remove(upgrade.type);
    }


    //In case of game over status, reset all upgrades
    public void ResetAllUpgrades()
    {
        //Stop all active upgrades and remove effects
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

        clickTarget.SetClickMultiplier(1f);
        clickTarget.EnableAutoClick(false);
        encryptedDataPoolManager.SlowScroll(false);
        encryptedDataPoolManager.BoostCodeOptimization(false);
        encryptedDataPoolManager.ReduceEncryptedCodeSensibility(false);
    }


    //Retrieving all upgrade buttons
    public List<UpgradeItemUI> GetAllUpgradeButons()
    {
        return upgradeButtons;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text upgradeNameText;
    [SerializeField] private Image upgradeIconImage;
    [SerializeField] private TMP_Text upgradeDescriptionText;
    [SerializeField] private TMP_Text upgradeDurationText;
    [SerializeField] private TMP_Text upgradePriceText;

    private UpgradeData currentUpgrade;
    private UpgradeData assignedUpgrade;

    public void AssignUpgrade(UpgradeData upgrade, System.Action<UpgradeData, UpgradeItemUI> purchaseCallback)
    {
        assignedUpgrade = upgrade;

        currentUpgrade = upgrade;

        upgradeNameText.text = upgrade.upgradeName;
        upgradeIconImage.sprite = upgrade.upgradeIcon;
        upgradeDescriptionText.text = upgrade.description;
        upgradeDurationText.text = FormatDuration(currentUpgrade.upgradeDuration);
        upgradePriceText.text = $"{upgrade.upgradePrice}";
    }

    private string FormatDuration(float duration)
    {
        return duration <= 0f ? "Instant" : $"{duration} s";
    }

}

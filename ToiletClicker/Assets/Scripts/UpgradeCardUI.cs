using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text upgradeNameText;
    [SerializeField] private Image upgradeIconImage;
    [SerializeField] private TMP_Text upgradeDescriptionText;
    //[SerializeField] private TMP_Text upgradePriceText;
    //[SerializeField] private TMP_Text upgradeDurationText;

    private UpgradeData currentUpgrade;
    private UpgradeData assignedUpgrade;

    public void Initialize(UpgradeData data)
    {
        upgradeNameText.text = data.upgradeName;
        upgradeIconImage.sprite = data.upgradeIcon;
        upgradeDescriptionText.text = data.description;
        //upgradePriceText.text = data.upgradePrice.ToString();
        
    }

    public void AssignUpgrade(UpgradeData upgrade, System.Action<UpgradeData, UpgradeItemUI> purchaseCallback)
    {
        assignedUpgrade = upgrade;

        currentUpgrade = upgrade;


        upgradeNameText.text = upgrade.upgradeName;

        upgradeIconImage.sprite = upgrade.upgradeIcon;
        upgradeDescriptionText.text = upgrade.description;
    }
}

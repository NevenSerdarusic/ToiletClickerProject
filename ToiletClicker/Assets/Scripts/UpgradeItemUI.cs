using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private Image lockedOverlay;

    public Button Button => button;

    private UpgradeData currentUpgrade;
    private System.Action<UpgradeData, UpgradeItemUI> onPurchase;

    public bool HasUpgrade => assignedUpgrade != null;
    private UpgradeData assignedUpgrade;

    public UpgradeData GetUpgrade() => assignedUpgrade;

    public void AssignUpgrade(UpgradeData upgrade, System.Action<UpgradeData, UpgradeItemUI> purchaseCallback)
    {
        assignedUpgrade = upgrade;

        currentUpgrade = upgrade;
        onPurchase = purchaseCallback;

        nameText.text = upgrade.upgradeName;
        costText.text = upgrade.upgradePrice.ToString();
        icon.sprite = upgrade.upgradeIcon;
        button.interactable = false;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onPurchase?.Invoke(currentUpgrade, this));
    }

    public void Clear()
    {
        currentUpgrade = null;
        nameText.text = "";
        costText.text = "";
        icon.sprite = null;
        button.interactable = false;
    }

    public void SetInteractable(bool state)
    {
        button.interactable = state;
    }


    public void ShowLockedOverlay(bool show)
    {
        // Npr. zaklju?aš button i staviš sivi overlay
        lockedOverlay.gameObject.SetActive(show); // moraš imati taj GameObject u UI-ju
    }

}

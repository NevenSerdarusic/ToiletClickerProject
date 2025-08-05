using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text upgradeLevel;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    //[SerializeField] private Image lockedOverlay;

    public Button Button => button;

    private UpgradeData currentUpgrade;
    private System.Action<UpgradeData, UpgradeItemUI> onPurchase;

    public bool HasUpgrade => currentUpgrade != null;

    public UpgradeData GetUpgrade() => currentUpgrade;


    public void AssignUpgrade(UpgradeData upgrade, System.Action<UpgradeData, UpgradeItemUI> purchaseCallback)
    {
        currentUpgrade = upgrade;
        onPurchase = purchaseCallback;

        nameText.text = upgrade.upgradeName;
        costText.text = upgrade.upgradePrice.ToString();
        icon.sprite = upgrade.upgradeIcon;
        upgradeLevel.text = upgrade.upgradeLevel.ToString();


        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onPurchase?.Invoke(currentUpgrade, this));
    }

    public void UpdateCardUIAfterPurchase(int newPrice, int newLevel)
    {
        costText.text = newPrice.ToString();
        upgradeLevel.text = newLevel.ToString();
    }
    public void Clear()
    {
        currentUpgrade = null;
        nameText.text = "";
        costText.text = "";
        icon.sprite = null;
        //button.interactable = false;
    }

    //public void SetInteractable(bool state)
    //{
    //    button.interactable = state;
    //    ShowLockedOverlay(!state);
    //}

    //public void ShowLockedOverlay(bool show)
    //{
    //    lockedOverlay.gameObject.SetActive(show);
    //}

}

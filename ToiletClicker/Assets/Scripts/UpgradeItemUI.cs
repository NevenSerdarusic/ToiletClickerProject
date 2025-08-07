using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    //[SerializeField] private Image icon;
    [SerializeField] private GameObject glow;
    [SerializeField] private Button button;
    [SerializeField] private Image lockedOverlay;

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
        //icon.sprite = upgrade.upgradeIcon;
        SetInteractable(false);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onPurchase?.Invoke(currentUpgrade, this));
    }

    public void Clear()
    {
        currentUpgrade = null;
        nameText.text = "";
        costText.text = "";
        //icon.sprite = null;
        SetInteractable(false);
    }

    public void SetInteractable(bool state)
    {
        button.interactable = state;

        if (glow != null)
            glow.SetActive(state);

        ShowLockedOverlay(!state);
    }

    public void ShowLockedOverlay(bool show)
    {
        lockedOverlay.gameObject.SetActive(show);
    }

}

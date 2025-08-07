using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text codeDisplayText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private GameObject glow;
    [SerializeField] private Button button;

    private CodeFragment currentItem;
    private System.Action<CodeFragment, ShopItemUI> onPurchase;

    public bool HasItem => currentItem != null;
    public CodeFragment GetItem() => currentItem;

    public void AssignItem(CodeFragment item, System.Action<CodeFragment, ShopItemUI> purchaseCallback)
    {
        currentItem = item;
        onPurchase = purchaseCallback;

        costText.text = item.cost.ToString();
        codeDisplayText.text = item.fragmentDisplayText;
        SetInteractable(false);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onPurchase?.Invoke(currentItem, this));
    }

    public void Clear()
    {
        currentItem = null;
        costText.text = "";
        SetInteractable(false);
    }

    public void SetInteractable(bool state)
    {
        button.interactable = state;

        if (glow != null)
            glow.SetActive(state);
    }
}

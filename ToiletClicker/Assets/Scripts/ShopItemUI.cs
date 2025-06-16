using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;

    private FoodItem currentItem;
    private System.Action<FoodItem, ShopItemUI> onPurchase;

    public bool HasItem => currentItem != null;
    public FoodItem GetItem() => currentItem;

    public void AssignItem(FoodItem item, System.Action<FoodItem, ShopItemUI> purchaseCallback)
    {
        currentItem = item;
        onPurchase = purchaseCallback;

        nameText.text = item.foodName;
        costText.text = item.cost.ToString();
        icon.sprite = item.foodImage;
        button.interactable = false;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onPurchase?.Invoke(currentItem, this));
    }

    public void Clear()
    {
        currentItem = null;
        nameText.text = "";
        costText.text = "";
        icon.sprite = null;
        button.interactable = false;
    }

    public void SetInteractable(bool state)
    {
        button.interactable = state;
    }
}

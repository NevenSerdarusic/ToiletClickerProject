using UnityEngine;
using UnityEngine.UI;

public class FoodSlot : MonoBehaviour
{
    [SerializeField] private Image foodImage;
    private FoodItem currentFood;

    public bool HasBeenEaten { get; private set; } = false;

    public void SetFood(FoodItem food)
    {
        currentFood = food;
        foodImage.sprite = food.foodImage;
        foodImage.enabled = true;
        HasBeenEaten = false;
    }

    public Sprite GetFoodSprite()
    {
        return currentFood.foodImage;
    }

    public void ClearSlot()
    {
        foodImage.enabled = false;
    }

    public void MarkAsEaten()
    {
        HasBeenEaten = true;
    }

    public FoodItem GetCurrentFood()
    {
        return currentFood;
    }

}

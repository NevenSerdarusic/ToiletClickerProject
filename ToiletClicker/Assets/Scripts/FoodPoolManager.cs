using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class FoodPoolManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private WeightManager weightManager;

    [Header("Slider Settings")]
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private RectTransform sliderLimiter;
    [SerializeField] private float scrollSpeed = 1f;

    [Header("Food Slot Setup")]
    [SerializeField] List<FoodItem> junkFoodItems;

    [Header("Eating Panel")]
    [SerializeField] Image foodImage;
    [SerializeField] FillMethod imageFillMethod;
    [SerializeField] int fillOrigin;
    [SerializeField] float animationDuration = 2f;
    

    private List<RectTransform> slotRects = new List<RectTransform>();
    private float slotWidth;
    private float spacing;

    private Vector3[] slotCorners = new Vector3[4];
    private Vector3[] limiterCorners = new Vector3[4];


    private void Start()
    {
        sliderLimiter.GetWorldCorners(limiterCorners);

        // Cache spacing if exists
        var layout = contentTransform.GetComponent<HorizontalLayoutGroup>();
        spacing = layout != null ? layout.spacing : 0f;

        foreach (Transform child in contentTransform)
        {
            if (child.TryGetComponent(out RectTransform rect))
            {
                slotRects.Add(rect);

                if (rect.TryGetComponent(out FoodSlot foodSlot))
                    foodSlot.SetFood(GetRandomFood());
            }
        }

        if (slotRects.Count > 0)
            slotWidth = slotRects[0].rect.width;
    }

    private void Update()
    {
        //If gamer is Paused or Game is over stop scrolling
        if (gameManager.IsGamePaused || gameManager.IsGameOver)
            return;

        ScrollContent();

        RectTransform closestSlot = null;
        float closestDistance = float.MaxValue;

        foreach (var rect in slotRects)
        {
            
            rect.GetWorldCorners(slotCorners);

            // Recycle ako je izvan desnog ruba
            if (slotCorners[0].x > limiterCorners[2].x)
            {
                if (!rect.GetComponent<FoodSlot>().HasBeenEaten)
                {
                    var food = rect.GetComponent<FoodSlot>().GetCurrentFood();
                    if (food != null)
                    {
                        float weightCalc = food.FoodWeightGain;
                        weightManager.AddWeight(food.FoodWeightGain);
                        rect.GetComponent<FoodSlot>().MarkAsEaten();

                        StartEatingAnimation(animationDuration);
                    }
                }

                RecycleSlot(rect);
            }
                
            
            // Centriranje slike
            float slotCenterX = (slotCorners[0].x + slotCorners[3].x) * 0.5f;
            float limiterCenterX = (limiterCorners[0].x + limiterCorners[3].x) * 0.5f;
            float distance = Mathf.Abs(slotCenterX - limiterCenterX);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSlot = rect;
            }
        }

        if (closestSlot != null)
            UpdateFoodImage(closestSlot);
    }

    private void ScrollContent()
    {
        contentTransform.anchoredPosition += Vector2.right * scrollSpeed * Time.deltaTime;
    }

    private void RecycleSlot(RectTransform slot)
    {
        contentTransform.anchoredPosition -= new Vector2(slotWidth + spacing, 0f);
        slot.SetAsFirstSibling();

        if (slot.TryGetComponent(out FoodSlot foodSlot))
            foodSlot.SetFood(GetRandomFood());
    }

    private void UpdateFoodImage(RectTransform slot)
    {
        if (slot.TryGetComponent(out FoodSlot foodSlot) && foodSlot.GetFoodSprite() != null)
        {
            foodImage.sprite = foodSlot.GetFoodSprite();

            foodImage.type = Image.Type.Filled;
            foodImage.fillMethod = imageFillMethod;
            foodImage.fillOrigin = fillOrigin;
            foodImage.fillClockwise = true;
            foodImage.fillAmount = 1f; // full na po?etku
        }
    }

    private FoodItem GetRandomFood()
    {
        if (junkFoodItems == null || junkFoodItems.Count == 0) return null;
        return junkFoodItems[Random.Range(0, junkFoodItems.Count)];
    }

    //Replacing Junk food with Healthy food in scroll rect
    public void ReplaceFirstUnhealthyFoodWithHealthy(FoodItem purchasedItem)
    {
        if (purchasedItem == null)
        {
            Debug.LogWarning("Kupljeni item je null!");
            return;
        }

        RectTransform closestJunkSlot = null;
        float closestDistance = float.MaxValue;

        Vector3[] limiterCorners = new Vector3[4];
        sliderLimiter.GetWorldCorners(limiterCorners);
        float limiterCenterX = (limiterCorners[0].x + limiterCorners[3].x) * 0.5f;


        foreach (var rect in slotRects)
        {
            rect.GetWorldCorners(slotCorners);
            float slotCenterX = (slotCorners[0].x + slotCorners[3].x) * 0.5f;
            float distance = Mathf.Abs(slotCenterX - limiterCenterX);

            var foodSlot = rect.GetComponent<FoodSlot>();
            if (foodSlot == null || foodSlot.HasBeenEaten) continue;

            FoodItem currentItem = foodSlot.GetCurrentFood();
            if (currentItem != null && junkFoodItems.Contains(currentItem))
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestJunkSlot = rect;
                }
            }
        }

        if (closestJunkSlot != null)
        {
            FoodSlot slot = closestJunkSlot.GetComponent<FoodSlot>();
            slot.SetFood(purchasedItem);
        }
        else
        {
            Debug.Log("Cant replace junk food in slot!");
        }
    }

    //Eating Image Animation
    //IVAN: dodati logiku za 3 spritea.....
    public void StartEatingAnimation(float duration = 1f)
    {
        LeanTween.value(gameObject, 1f, 0f, duration).setOnUpdate((float val) => {
            foodImage.fillAmount = val;
        });

        //3 spritea 
        //mrvice hrane
    }
}

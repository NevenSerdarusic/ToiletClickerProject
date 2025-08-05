using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
using System.Linq;

public class FoodPoolManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private WeightManager weightManager;
    [SerializeField] private ShopManager shopManager;

    [Header("Slider Settings")]
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private RectTransform sliderLimiter;
    [SerializeField] private float scrollSpeed = 1f;

    [Header("Food Slot Setup")]
    [SerializeField] List<FoodItem> junkFoodItems;
    private List<FoodItem> healthyFoodItems;

    [Header("Eating Panel")]
    [SerializeField] Image foodImage;
    [SerializeField] FillMethod imageFillMethod;
    [SerializeField] int fillOrigin;
    
    private List<RectTransform> slotRects = new List<RectTransform>();
    private float slotWidth;
    private float spacing;

    private Vector3[] slotCorners = new Vector3[4];
    private Vector3[] limiterCorners = new Vector3[4];

    private float defaultScrollSpeed;
    private bool isScrollSlowed = false;
    private bool fiberBoosted = false;
    private Dictionary<FoodItem, float> originalFibers = new Dictionary<FoodItem, float>();
    private Dictionary<FoodItem, (float originalFat, float originalSugar)> originalJunkNutritionValues = new();


    //private void Start()
    //{
    //    healthyFoodItems = shopManager.GetAllShopItems();

    //    sliderLimiter.GetWorldCorners(limiterCorners);

    //    // Cache spacing if exists
    //    var layout = contentTransform.GetComponent<HorizontalLayoutGroup>();
    //    spacing = layout != null ? layout.spacing : 0f;

    //    foreach (Transform child in contentTransform)
    //    {
    //        if (child.TryGetComponent(out RectTransform rect))
    //        {
    //            slotRects.Add(rect);

    //            if (rect.TryGetComponent(out FoodSlot foodSlot))
    //                foodSlot.SetFood(GetRandomFood());
    //        }
    //    }

    //    if (slotRects.Count > 0)
    //        slotWidth = slotRects[0].rect.width;
    //}

    //private void Update()
    //{
    //    //If gamer is Paused or Game is over stop scrolling
    //    if (gameManager.IsGamePaused || gameManager.IsGameOver)
    //        return;

    //    ScrollContent();

    //    RectTransform closestSlot = null;
    //    float closestDistance = float.MaxValue;

    //    foreach (var rect in slotRects)
    //    {
            
    //        rect.GetWorldCorners(slotCorners);

    //        // Recycle ako je izvan desnog ruba
    //        if (slotCorners[0].x > limiterCorners[2].x)
    //        {
    //            if (!rect.GetComponent<FoodSlot>().HasBeenEaten)
    //            {
    //                var food = rect.GetComponent<FoodSlot>().GetCurrentFood();
    //                if (food != null)
    //                {
    //                    float weightCalc = food.FoodWeightGain;
    //                    weightManager.AddWeight(food.FoodWeightGain);
    //                    weightManager.UpdateIndividualWeightUI(food.FoodWeightGain);
    //                    rect.GetComponent<FoodSlot>().MarkAsEaten();

    //                    float duration = GetEatingAnimationDuration(rect);
    //                    StartEatingAnimation(duration);
    //                }
    //            }

    //            RecycleSlot(rect);
    //            SoundManager.Instance.Play("Toilet Flush");
                
    //        }
                
            
    //        // Centriranje slike
    //        float slotCenterX = (slotCorners[0].x + slotCorners[3].x) * 0.5f;
    //        float limiterCenterX = (limiterCorners[0].x + limiterCorners[3].x) * 0.5f;
    //        float distance = Mathf.Abs(slotCenterX - limiterCenterX);

    //        if (distance < closestDistance)
    //        {
    //            closestDistance = distance;
    //            closestSlot = rect;
    //        }
    //    }

    //    if (closestSlot != null)
    //        UpdateFoodImage(closestSlot);
    //}

    //private void ScrollContent()
    //{
    //    contentTransform.anchoredPosition += Vector2.right * scrollSpeed * Time.deltaTime;
    //}

    //private void RecycleSlot(RectTransform slot)
    //{
    //    contentTransform.anchoredPosition -= new Vector2(slotWidth + spacing, 0f);
    //    slot.SetAsFirstSibling();

    //    if (slot.TryGetComponent(out FoodSlot foodSlot))
    //        foodSlot.SetFood(GetRandomFood());
    //}

    //private void UpdateFoodImage(RectTransform slot)
    //{
    //    if (slot.TryGetComponent(out FoodSlot foodSlot) && foodSlot.GetFoodSprite() != null)
    //    {
    //        foodImage.sprite = foodSlot.GetFoodSprite();

    //        foodImage.type = Image.Type.Filled;
    //        foodImage.fillMethod = imageFillMethod;
    //        foodImage.fillOrigin = fillOrigin;
    //        foodImage.fillClockwise = true;
    //        foodImage.fillAmount = 1f; // full na po?etku
    //    }
    //}

    //private FoodItem GetRandomFood()
    //{
    //    if (junkFoodItems == null || junkFoodItems.Count == 0) return null;
    //    return junkFoodItems[Random.Range(0, junkFoodItems.Count)];
    //}

    ////Replacing Junk food with Healthy food in scroll rect
    //public void ReplaceFirstUnhealthyFoodWithHealthy(FoodItem purchasedItem)
    //{
    //    if (purchasedItem == null)
    //    {
    //        Debug.LogWarning("Kupljeni item je null!");
    //        return;
    //    }

    //    RectTransform closestJunkSlot = null;
    //    float closestDistance = float.MaxValue;

    //    Vector3[] limiterCorners = new Vector3[4];
    //    sliderLimiter.GetWorldCorners(limiterCorners);
    //    float limiterCenterX = (limiterCorners[0].x + limiterCorners[3].x) * 0.5f;


    //    foreach (var rect in slotRects)
    //    {
    //        rect.GetWorldCorners(slotCorners);
    //        float slotCenterX = (slotCorners[0].x + slotCorners[3].x) * 0.5f;
    //        float distance = Mathf.Abs(slotCenterX - limiterCenterX);

    //        var foodSlot = rect.GetComponent<FoodSlot>();
    //        if (foodSlot == null || foodSlot.HasBeenEaten) continue;

    //        FoodItem currentItem = foodSlot.GetCurrentFood();
    //        if (currentItem != null && junkFoodItems.Contains(currentItem))
    //        {
    //            if (distance < closestDistance)
    //            {
    //                closestDistance = distance;
    //                closestJunkSlot = rect;
    //            }
    //        }
    //    }

    //    if (closestJunkSlot != null)
    //    {
    //        FoodSlot slot = closestJunkSlot.GetComponent<FoodSlot>();
    //        slot.SetFood(purchasedItem);
    //        gameManager.AddXP(gameConfig.xpPerHealthyFoodEaten);
    //    }
    //    else
    //    {
    //        Debug.Log("Cant replace junk food in slot!");
    //    }
    //}


    ////UPGRADES:
    ////UpgradeType: DetoxBomb
    //public void ReplaceJunkWithHealthy(int count)
    //{
    //    if (shopManager == null)
    //    {
    //        Debug.LogWarning("ShopManager reference not set in FoodPoolManager!!");
    //        return;
    //    }



    //    if (healthyFoodItems == null || healthyFoodItems.Count == 0)
    //    {
    //        Debug.LogWarning("The list of healthy foods is not set!");
    //        return;
    //    }

    //    var junkSlots = GetSortedJunkFoodSlotsByDistanceToLimiter();
    //    int replaceCount = Mathf.Min(count, junkSlots.Count);

    //    for (int i = 0; i < replaceCount; i++)
    //    {
    //        FoodItem randomHealthy = healthyFoodItems[Random.Range(0, healthyFoodItems.Count)];
    //        junkSlots[i].SetFood(randomHealthy);
    //    }
    //}

    //private List<FoodSlot> GetSortedJunkFoodSlotsByDistanceToLimiter()
    //{
    //    List<(FoodSlot slot, float distance)> junkSlotsWithDistance = new List<(FoodSlot, float)>();

    //    Vector3[] limiterCorners = new Vector3[4];
    //    sliderLimiter.GetWorldCorners(limiterCorners);
    //    float limiterCenterX = (limiterCorners[0].x + limiterCorners[3].x) * 0.5f;

    //    foreach (var rect in slotRects)
    //    {
    //        rect.GetWorldCorners(slotCorners);
    //        float slotCenterX = (slotCorners[0].x + slotCorners[3].x) * 0.5f;
    //        float distance = Mathf.Abs(slotCenterX - limiterCenterX);

    //        var foodSlot = rect.GetComponent<FoodSlot>();
    //        if (foodSlot == null || foodSlot.HasBeenEaten) continue;

    //        FoodItem currentItem = foodSlot.GetCurrentFood();
    //        if (currentItem != null && junkFoodItems.Contains(currentItem))
    //        {
    //            junkSlotsWithDistance.Add((foodSlot, distance));
    //        }
    //    }

    //    //Sort by distance from limiter
    //    junkSlotsWithDistance.Sort((a, b) => a.distance.CompareTo(b.distance));

    //    //Return only FoodSlots
    //    return junkSlotsWithDistance.Select(pair => pair.slot).ToList();
    //}


    ////UpgradeType: SlowScroll
    //public void SlowScroll(bool enable)
    //{
    //    if (!enable && !isScrollSlowed) return;

    //    if (enable)
    //    {
    //        if (!isScrollSlowed)
    //        {
    //            defaultScrollSpeed = scrollSpeed;
    //            scrollSpeed *= gameConfig.snackLagMultiplier;
    //            isScrollSlowed = true;
    //        }
    //    }
    //    else
    //    {
    //        scrollSpeed = defaultScrollSpeed;
    //        isScrollSlowed = false;
    //    }
    //}

    ////UpgradeType: BoostFiberInHealthy
    //public void BoostFiberInHealthy(bool enable)
    //{
    //    fiberBoosted = enable;

    //    if (shopManager == null)
    //    {
    //        Debug.LogWarning("ShopManager reference not set in FoodPoolManager!");
    //        return;
    //    }

    //    var allItems = shopManager.GetAllShopItems();
    //    if (allItems == null || allItems.Count == 0)
    //    {
    //        Debug.LogWarning("There are no items available in the shop!");
    //        return;
    //    }

    //    foreach (var item in allItems)
    //    {
    //        if (!healthyFoodItems.Contains(item)) continue;

    //        if (enable)
    //        {
    //            if (!originalFibers.ContainsKey(item))
    //                originalFibers[item] = item.fibersContent;

    //            item.fibersContent *= gameConfig.fiberOverloadBonusAmount;
    //        }
    //        else
    //        {
    //            if (originalFibers.ContainsKey(item))
    //                item.fibersContent = originalFibers[item];
    //        }
    //    }

    //    if (!enable)
    //        originalFibers.Clear(); // O?isti memoriju kad se deaktivira efekt

    //    Debug.Log(enable
    //        ? "All healthy foods are fortified with fiber!"
    //        : "The fiber in healthy foods has been restored to its original values.");
    //}

    ////UpgradeType: FastMetabolism
    //public void ReduceJunkNutrition(bool enable)
    //{
    //    foreach (var rect in slotRects)
    //    {
    //        var foodSlot = rect.GetComponent<FoodSlot>();
    //        if (foodSlot == null) continue;

    //        var currentFood = foodSlot.GetCurrentFood();
    //        if (currentFood != null && junkFoodItems.Contains(currentFood))
    //        {
    //            if (enable)
    //            {
    //                // Ve? spremljeno? Presko?i
    //                if (!originalJunkNutritionValues.ContainsKey(currentFood))
    //                {
    //                    // Spremi originalne vrijednosti
    //                    originalJunkNutritionValues[currentFood] = (currentFood.fatContent, currentFood.sugarContent);

    //                    // Postavi smanjene vrijednosti
    //                    currentFood.fatContent = 1f;
    //                    currentFood.sugarContent = 1f;
    //                }
    //            }
    //            else
    //            {
    //                // Ako imamo spremljene originalne vrijednosti, vrati ih
    //                if (originalJunkNutritionValues.TryGetValue(currentFood, out var originalValues))
    //                {
    //                    currentFood.fatContent = originalValues.originalFat;
    //                    currentFood.sugarContent = originalValues.originalSugar;
    //                }
    //            }
    //        }
    //    }

    //    // Ako se vra?amo na original, o?isti dictionary
    //    if (!enable)
    //    {
    //        originalJunkNutritionValues.Clear();
    //    }
    //}


    ////Eating Image Animation
    ////IVAN: dodati logiku za 3 spritea koje ce se gasiti kako bi animirali konzumaciju hrane
    //float GetEatingAnimationDuration(RectTransform slot)
    //{
    //    // Udaljenost koju slot mora pre?i do limitera (horizontalna)
    //    float startX = slot.position.x;
    //    float limiterX = sliderLimiter.position.x;

    //    float distance = Mathf.Abs(limiterX - startX);
    //    float duration = distance / scrollSpeed;

    //    return duration;
    //}
    //public void StartEatingAnimation(float duration = 1f)
    //{
    //    LeanTween.value(gameObject, 1f, 0f, duration).setOnUpdate((float val) => {
    //        foodImage.fillAmount = val;
    //    });
    //}
}

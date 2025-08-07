using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
using System.Linq;
using TMPro;

public class EncryptedDataPoolManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private FirewallManager firewallManager;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private UIActions uIActions;

    [Header("Slider Settings")]
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private RectTransform sliderLimiter;
    [SerializeField] private float scrollSpeed = 1f;

    [Header("Encrypted Code Fragments")]
    [SerializeField] List<CodeFragment> encryptedFragments;
    private List<CodeFragment> decodedFragments;
    
    private List<RectTransform> slotRects = new List<RectTransform>();
    private float slotWidth;
    private float spacing;

    private Vector3[] slotCorners = new Vector3[4];
    private Vector3[] limiterCorners = new Vector3[4];

    private float defaultScrollSpeed;
    private bool isScrollSlowed = false;
    private bool optimizationBoosted = false;
    
    //Code junk and detection Dictionary
    private Dictionary<CodeFragment, (float origJunk, float origDetection)> fragmentDefaults
    = new Dictionary<CodeFragment, (float, float)>();
    //Code optimization Dictionary
    private Dictionary<CodeFragment, float> fragmentOptimizationDefaults
    = new Dictionary<CodeFragment, float>();

    private void Start()
    {
        decodedFragments = shopManager.GetAllShopItems();

        sliderLimiter.GetWorldCorners(limiterCorners);

        // Cache spacing if exists
        var layout = contentTransform.GetComponent<HorizontalLayoutGroup>();
        spacing = layout != null ? layout.spacing : 0f;

        foreach (Transform child in contentTransform)
        {
            if (child.TryGetComponent(out RectTransform rect))
            {
                slotRects.Add(rect);

                if (rect.TryGetComponent(out CodeSlot foodSlot))
                    foodSlot.SetCode(GetRandomEncryptedCode());
            }
        }

        if (slotRects.Count > 0)
            slotWidth = slotRects[0].rect.width;

        InitializeCodeDictionaries();
    }

    private void InitializeCodeDictionaries()
    {
        foreach (var frag in encryptedFragments)
        {
            fragmentDefaults[frag] = (frag.junkDensity, frag.detectionRisk);
        }
        foreach (var frag in decodedFragments)
        {
            fragmentOptimizationDefaults[frag] = frag.optimizationValue;
        }

        //Debug.Log("=== JUNK AND DETECTION ===");
        //foreach (var kvp in fragmentDefaults)
        //{
        //    var frag = kvp.Key;
        //    var (origJunk, origDetection) = kvp.Value;
        //    Debug.Log($"Fragment '{frag.fragmentDisplayText}': junkDensity={origJunk}, detectionRisk={origDetection}");
        //}
        //Debug.Log("=== OPTIMIZATION ===");
        //foreach (var kvp in fragmentOptimizationDefaults)
        //{
        //    var frag = kvp.Key;
        //    var origOpt = kvp.Value;
        //    Debug.Log($"Fragment '{frag.fragmentDisplayText}': optimizationValue={origOpt}");
        //}
    }

    private void Update()
    {
        //If gamer is Paused or Game is over stop scrolling
        if (gameManager.IsGamePaused || gameManager.IsGameOver)
        {
            return;
        }
      
        ScrollContent();

        RectTransform closestSlot = null;
        float closestDistance = float.MaxValue;

        foreach (var rect in slotRects)
        {
            rect.GetWorldCorners(slotCorners);

            if (slotCorners[0].x > limiterCorners[2].x)
            {
                if (!rect.GetComponent<CodeSlot>().HasBeenDecoded)
                {
                    var code = rect.GetComponent<CodeSlot>().GetCurrentCode();
                    if (code != null)
                    {
                        firewallManager.AddFirewallProtection(code.FirewallProtectionImpact);
                        firewallManager.UpdateIndividualFirewallProtectionUI(code.FirewallProtectionImpact);
                        rect.GetComponent<CodeSlot>().MarkAsDecoded();

                        if (code.isEncrypted)
                        {
                            uIActions.AnimateProcessedCodeTyping(StringLibrary.encryptedCodeMessageDisplay, uiManager.DangerTextColor);
                        }
                        else
                        {
                            uIActions.AnimateProcessedCodeTyping(StringLibrary.decodedCodeMessageDispay, uiManager.WarningTextColor);
                        }
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
        

       
    }

    private void ScrollContent()
    {
        contentTransform.anchoredPosition += Vector2.right * scrollSpeed * Time.deltaTime;
    }

    private void RecycleSlot(RectTransform slot)
    {
        contentTransform.anchoredPosition -= new Vector2(slotWidth + spacing, 0f);
        slot.SetAsFirstSibling();

        if (slot.TryGetComponent(out CodeSlot codeSlot))
            codeSlot.SetCode(GetRandomEncryptedCode());
            codeSlot.HasBeenDecoded = false;
    }

    private CodeFragment GetRandomEncryptedCode()
    {
        if (encryptedFragments == null || encryptedFragments.Count == 0) return null;
        return encryptedFragments[Random.Range(0, encryptedFragments.Count)];
    }


    public void ReplaceFirstEncryptedCodeWithDecoded(CodeFragment purchasedItem)
    {
        if (purchasedItem == null)
        {
            Debug.LogWarning("Purchased item is null!");
            return;
        }

        RectTransform closestEncryptedSlot = null;
        float closestDistance = float.MaxValue;

        Vector3[] limiterCorners = new Vector3[4];
        sliderLimiter.GetWorldCorners(limiterCorners);
        float limiterCenterX = (limiterCorners[0].x + limiterCorners[3].x) * 0.5f;


        foreach (var rect in slotRects)
        {
            rect.GetWorldCorners(slotCorners);
            float slotCenterX = (slotCorners[0].x + slotCorners[3].x) * 0.5f;
            float distance = Mathf.Abs(slotCenterX - limiterCenterX);

            var codeSlot = rect.GetComponent<CodeSlot>();
            if (codeSlot == null || codeSlot.HasBeenDecoded) continue;

            CodeFragment currentItem = codeSlot.GetCurrentCode();
            if (currentItem != null && encryptedFragments.Contains(currentItem))
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEncryptedSlot = rect;
                }
            }
        }

        if (closestEncryptedSlot != null)
        {
            CodeSlot slot = closestEncryptedSlot.GetComponent<CodeSlot>();
            slot.SetCode(purchasedItem);
            gameManager.AddXP(purchasedItem.XPValue);
        }
        else
        {
            Debug.Log("Can`t replace encryted code in slot!");
        }
    }


    //UPGRADES:
    //UpgradeType: Proxy Flip
    public void ReplaceEncryptedWithDecoded(int count)
    {
        if (shopManager == null)
        {
            Debug.LogWarning("ShopManager reference not set in EncryptedDataPoolManager!!");
            return;
        }



        if (decodedFragments == null || decodedFragments.Count == 0)
        {
            Debug.LogWarning("The list of decoded fragments is not set!");
            return;
        }

        var encryptedSlots = GetSortedEncryptedCodeSlotsByDistanceToLimiter();
        int replaceCount = Mathf.Min(count, encryptedSlots.Count);

        for (int i = 0; i < replaceCount; i++)
        {
            CodeFragment randomDecoded = decodedFragments[Random.Range(0, decodedFragments.Count)];
            encryptedSlots[i].SetCode(randomDecoded);
        }
    }

    private List<CodeSlot> GetSortedEncryptedCodeSlotsByDistanceToLimiter()
    {
        List<(CodeSlot slot, float distance)> encryptedSlotsWithDistance = new List<(CodeSlot, float)>();

        Vector3[] limiterCorners = new Vector3[4];
        sliderLimiter.GetWorldCorners(limiterCorners);
        float limiterCenterX = (limiterCorners[0].x + limiterCorners[3].x) * 0.5f;

        foreach (var rect in slotRects)
        {
            rect.GetWorldCorners(slotCorners);
            float slotCenterX = (slotCorners[0].x + slotCorners[3].x) * 0.5f;
            float distance = Mathf.Abs(slotCenterX - limiterCenterX);

            var codeSlot = rect.GetComponent<CodeSlot>();
            if (codeSlot == null || codeSlot.HasBeenDecoded) continue;

            CodeFragment currentItem = codeSlot.GetCurrentCode();
            if (currentItem != null && encryptedFragments.Contains(currentItem))
            {
                encryptedSlotsWithDistance.Add((codeSlot, distance));
            }
        }

        //Sort by distance from limiter
        encryptedSlotsWithDistance.Sort((a, b) => a.distance.CompareTo(b.distance));

        //Return only FoodSlots
        return encryptedSlotsWithDistance.Select(pair => pair.slot).ToList();
    }

    //UpgradeType: Code Freeze
    public void SlowScroll(bool enable)
    {
        if (!enable && !isScrollSlowed) return;

        if (enable)
        {
            if (!isScrollSlowed)
            {
                defaultScrollSpeed = scrollSpeed;
                scrollSpeed *= gameConfig.scrollRectDecreaseSpeedMultiplier;
                isScrollSlowed = true;
            }
        }
        else
        {
            scrollSpeed = defaultScrollSpeed;
            isScrollSlowed = false;
        }
    }

    //UpgradeType: Exploit Max
    public void BoostCodeOptimization(bool enable)
    {
        if(decodedFragments == null || decodedFragments.Count == 0)
    {
            Debug.LogWarning("There are no decoded fragments!");
            return;
        }

        if (enable)
        {
            //Set new values from GameConfig
            foreach (var frag in decodedFragments)
            {
                frag.optimizationValue = fragmentOptimizationDefaults[frag]
                                         * gameConfig.encryptedCodeOptimizationMultiplier;
            }
        }
        else
        {
            //Return from Dictionary
            foreach (var kvp in fragmentOptimizationDefaults)
            {
                kvp.Key.optimizationValue = kvp.Value;
            }
        }
    }

    //UpgradeType: Impact Drop	
    public void ReduceEncryptedCodeSensibility(bool enable)
    {
        if (decodedFragments == null || decodedFragments.Count == 0)
        {
            Debug.LogWarning("There are no decoded fragments!");
            return;
        }

        if (enable)
        {
            //Set new values from GameConfig
            foreach (var frag in encryptedFragments)
            {
                frag.junkDensity = gameConfig.encryptedCodeJunkMultiplier;
                frag.detectionRisk = gameConfig.encryptedCodeDetectionMultiplier;
            }
        }
        else
        {
            //Return from Dictionary
            foreach (var kvp in fragmentDefaults)
            {
                var frag = kvp.Key;
                var (origJ, origD) = kvp.Value;
                frag.junkDensity = origJ;
                frag.detectionRisk = origD;
            }
        }
    }


}

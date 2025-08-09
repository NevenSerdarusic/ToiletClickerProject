using System;
using UnityEngine;
using UnityEngine.UI;

public class TraceScannerManager : MonoBehaviour
{
    [Header("Trace Conductiong Status")]
    [SerializeField] private bool isTraceActive = true;

    public void EnableTrace() => isTraceActive = true;
    public void DisableTrace() => isTraceActive = false;

    [Header("Game Status")]
    [SerializeField] private bool isOverloaded = false;
    [SerializeField] private float overloadTimer = 0f; //Real-time timer od overload

    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private FirewallManager firewallManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    
    [Header("Slider Settings")]
    [SerializeField] private Slider traceScannerSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private RectTransform preWarningThresholdMarker;
    [SerializeField] private RectTransform criticalTresholdMarker;

    //counter
    private float currentTraceDetection = 0f;
   
    //bool states that will be monitred on every Click
    private bool isDetectionDecreaseBoostActivated = false;
    private bool isDetectionPerClickBoostActivated = false;
    private bool isFirewallProtectionDecreasePerClickBoostActivated = false;

    public event Action<GameOverReason> OnGameOverRequested;


    private void Start()
    {
        PositionCriticalThresholdMarker();
        //Turn off warning text GO
        uiManager.ShowWarningMessage("", 0 , false, uiManager.DefaultTextColor);
        //Set slider to default value
        traceScannerSlider.value = 0;
    }

    private void Update()
    {
        if (!isTraceActive) return;

        HandleDetectionDrain();
        HandleOverloadLogic();
        HandlePreWarningUI();
    }

    public void OnClick()
    {
        currentTraceDetection += GetCurrentDetectionFillAmount();
        currentTraceDetection = Mathf.Clamp(currentTraceDetection, 0f, 100f);

        UpdateTraceDetectionSlider();
        ApplyFirewallProtectionDecreasePerClick();
    }

    private void HandleDetectionDrain()
    {
        if (currentTraceDetection > 0f)
        {
            currentTraceDetection -= GetCurrentDetectionDrainAmount() * Time.deltaTime;
            currentTraceDetection = Mathf.Clamp(currentTraceDetection, 0f, 100f);
            UpdateTraceDetectionSlider();
        }
    }

    private void HandleOverloadLogic()
    {
        if (currentTraceDetection >= gameConfig.Current.criticalThreshold)
        {
            if (!isOverloaded)
            {
                isOverloaded = true;
                OnCriticalDetectionReached();
            }
        }
        else if (currentTraceDetection < gameConfig.Current.preWarningThreshold)
        {
            if (isOverloaded)
            {
                isOverloaded = false;
                OnTraceDetectionBackToSafe();
            }

            overloadTimer = 0f;
        }

        if (isOverloaded)
        {
            overloadTimer += Time.deltaTime;
            if (overloadTimer >= gameConfig.Current.detectionOverloadDurationBeforeGameOver)
                OnGameOverRequested?.Invoke(GameOverReason.TraceDetected);
        }
    }

    private void HandlePreWarningUI()
    {
        if (currentTraceDetection >= gameConfig.Current.preWarningThreshold)
        {
            uiManager.ShowWarningMessage(
                uiManager.CriticalPreassure,
                uiManager.WarningTextDuration,
                true,
                uiManager.DangerTextColor
            );
        }
    }

    private void UpdateTraceDetectionSlider()
    {
        if (traceScannerSlider != null)
            traceScannerSlider.value = currentTraceDetection / 100f;
    }

    public void ResetTraceDetection()
    {
        currentTraceDetection = 0f;
        UpdateTraceDetectionSlider();
    }

    private void OnCriticalDetectionReached()
    {
        uiManager.StartTraceScannerTimer(gameConfig.Current.detectionOverloadDurationBeforeGameOver);
        SoundManager.Instance.PlayControlled("Countdown");
    }

    private void OnTraceDetectionBackToSafe()
    {
        uiManager.ShowWarningMessage(uiManager.SafePreassure, uiManager.WarningTextDuration, true, uiManager.WarningTextColor);
        uiManager.StopTraceDetectionTimer();
        SoundManager.Instance.Stop("Countdown");
    }

    private void ApplyFirewallProtectionDecreasePerClick()
    {
        if (firewallManager != null)
        {
            float amount = isFirewallProtectionDecreasePerClickBoostActivated
                ? gameConfig.Current.firewallProtectionDecreasePerClickMultiplier
                : gameConfig.Current.firewallDecreasePerClickStandard;

            firewallManager.SubtractFirewallProtection(amount);
        }
        else
        {
            Debug.LogWarning("FirewallManager is not set!");
        }
    }

    private float GetCurrentDetectionFillAmount()
    {
        return isDetectionPerClickBoostActivated
            ? gameConfig.Current.detectionFillMultiplier
            : gameConfig.Current.detectionPerClickStandard;
    }

    private float GetCurrentDetectionDrainAmount()
    {
        return isDetectionDecreaseBoostActivated
            ? gameConfig.Current.detectionDrainMultiplier
            : gameConfig.Current.detectionDecreasePerSecondStandard;
    }

    private void PositionCriticalThresholdMarker()
    {
        if (traceScannerSlider == null)
            return;

        RectTransform sliderRect = traceScannerSlider.GetComponent<RectTransform>();
        float sliderWidth = sliderRect.rect.width;

        if (criticalTresholdMarker != null)
        {
            float normalizedCritical = Mathf.Clamp01(gameConfig.Current.criticalThreshold / 100f);
            float xCritical = Mathf.Lerp(-sliderWidth / 2f, sliderWidth / 2f, normalizedCritical);
            Vector2 newPosCritical = criticalTresholdMarker.anchoredPosition;
            newPosCritical.x = xCritical;
            criticalTresholdMarker.anchoredPosition = newPosCritical;
        }

        if (preWarningThresholdMarker != null)
        {
            float normalizedPreWarning = Mathf.Clamp01(gameConfig.Current.preWarningThreshold / 100f);
            float xPreWarning = Mathf.Lerp(-sliderWidth / 2f, sliderWidth / 2f, normalizedPreWarning);
            Vector2 newPosPreWarning = preWarningThresholdMarker.anchoredPosition;
            newPosPreWarning.x = xPreWarning;
            preWarningThresholdMarker.anchoredPosition = newPosPreWarning;
        }
    }

    // UPGRADES CONTROL METHODS
    public void DetectionDecreaseBoost(bool isActive) => isDetectionDecreaseBoostActivated = isActive;
    public void DetectionPerClickBoost(bool isActive) => isDetectionPerClickBoostActivated = isActive;
    public void FirewallPerClickDecrease(bool isActive) => isFirewallProtectionDecreasePerClickBoostActivated = isActive;
}

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

        if (currentTraceDetection > 0f)
        {
            CalculateTrackDetectionDecrease();
            UpdateTraceDetectionSlider();
        }

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

    public void OnClick()
    {
        CalculateDetectionOnClick();

        ApplyFirewallProtectionDecreasePerClick();
    }

    private void UpdateTraceDetectionSlider()
    {
        if (traceScannerSlider != null)
            traceScannerSlider.value = currentTraceDetection / 100f;
    }

    public void ResetTraceDetection()
    {
        currentTraceDetection = 0f;
        
        if (traceScannerSlider != null)
            traceScannerSlider.value = currentTraceDetection / 100f;
    }

    private void OnCriticalDetectionReached()
    {
        //Start animating dangerous lights
        uiManager.StartTraceScannerTimer(gameConfig.Current.detectionOverloadDurationBeforeGameOver);
        SoundManager.Instance.PlayControlled("Countdown");
    }

    private void OnTraceDetectionBackToSafe()
    {
        uiManager.ShowWarningMessage(uiManager.SafePreassure, uiManager.WarningTextDuration, true, uiManager.WarningTextColor);
        uiManager.StopTraceDetectionTimer();
        SoundManager.Instance.Stop("Countdown");
    }

    private void CalculateDetectionOnClick()
    {
        if (isDetectionPerClickBoostActivated)
        {
            currentTraceDetection += gameConfig.Current.preassurePerClickUpgradeMultiplier; //1
            currentTraceDetection = Mathf.Clamp(currentTraceDetection, 0f, 100f);
        }
        else
        {
            currentTraceDetection += gameConfig.Current.detectionPerClickStandard; //3
            currentTraceDetection = Mathf.Clamp(currentTraceDetection, 0f, 100f);
        }
    }
    
    private void ApplyFirewallProtectionDecreasePerClick()
    {
        if (firewallManager != null)
        {
            if (isFirewallProtectionDecreasePerClickBoostActivated)
            {
                firewallManager.SubtractFirewallProtection(gameConfig.Current.firewallProtectionDecreasePerClickMultiplier);
            }
            else
            {
                firewallManager.SubtractFirewallProtection(gameConfig.Current.firewallDecreasePerClickStandard);
            }
        }
        else
        {
            Debug.LogWarning("Weight Manager is not set!");
        }
    }

    private void CalculateTrackDetectionDecrease()
    {
        //Check the bool isPreassureDecreaseBoostActivated and set the value of pressureDecreasePerSecond accordingly.
        float decrease = isDetectionDecreaseBoostActivated
         ? gameConfig.Current.detectionPerSecondMultiplier * Time.deltaTime
         : gameConfig.Current.detectionDecreasePerSecondStandard * Time.deltaTime;

        currentTraceDetection -= decrease;
        currentTraceDetection = Mathf.Clamp(currentTraceDetection, 0f, 100f);
    }


    //Method that sets the warningTeshold & criticalThreshold to a position on the slider that must not be exceeded
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



    //UPGRADES RELATED METHODS:
    //Method that controls the increase in the variable responsible for the rate of pressure drop during printing
    public void DetectionDecreaseBoost(bool isActive)
    {
        isDetectionDecreaseBoostActivated = isActive;
    }


    //Method that controls the reduction of the value of the variable responsible for the level of pressure increase when tapping
    public void DetectionPerClickBoost(bool isActive)
    {
        isDetectionPerClickBoostActivated = isActive;
    }

    public void FirewallPerClickDecrease(bool isActive)
    {
        isFirewallProtectionDecreasePerClickBoostActivated = isActive;
    }
}

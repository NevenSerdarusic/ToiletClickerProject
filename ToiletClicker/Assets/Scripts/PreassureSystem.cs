using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreassureSystem : MonoBehaviour
{
    [Header("Game Status")]
    [SerializeField] private bool isOverloaded = false;
    [SerializeField] private float overloadTimer = 0f; //Real-time timer od overload

    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private WeightManager weightManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    
    [Header("Slider Settings")]
    [SerializeField] private Slider pressureSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private RectTransform preWarningThresholdMarker;
    [SerializeField] private RectTransform criticalTresholdMarker;

    //counter
    private float currentPressure = 0f;
   
    //bool states that will be monitred on every Click
    private bool isPreassureDecreaseBoostActivated = false;
    private bool isPreassurePerClickBoostActivated = false;
    private bool isWeightLossPerClickBoostActivated = false;


    //public bool IsOverloaded() => isOverloaded;

    private void Start()
    {
        PositionCriticalThresholdMarker();
        //Turn off warning text GO
        uiManager.ShowWarningMessage("", 0 , false, uiManager.DefaultTextColor);
        //Set slider to default value
        pressureSlider.value = 0;
    }


    private void Update()
    {
        if (currentPressure > 0)
        {
            CalculatePressureDecrease();
            UpdateSlider();

        }

        //If we are above the critical value, we count the time
        if (currentPressure >= gameConfig.preWarningThreshold)
        {
            //If we are just now entering the overload zone, show a warning
            if (currentPressure >= gameConfig.criticalThreshold)
            {
                if (!isOverloaded)
                {
                    isOverloaded = true;
                    OnCriticalPressureReached();
                }
            }

            //Start measuring time in overload
            overloadTimer += Time.deltaTime;


            if (overloadTimer >= gameConfig.preassureOverloadDurationBeforeGameOver)
            {
                gameManager.TriggerGameOver(GameOverReason.PressureOverload);
            }
        }
        else
        {
            //If we fall below criticalThreshold, reset overload state
            if (isOverloaded)
            {
                isOverloaded = false;
                OnPressureBackToSafe();
            }

            overloadTimer = 0f;
        }
    }

    public void OnClick()
    {
        CalculatePreassureOnClick();

        //UpdateSlider();

        ApplyWeightLossPerClick();
    }

    private void UpdateSlider()
    {
        if (pressureSlider != null)
            pressureSlider.value = currentPressure / 100f;

        if (fillImage != null)
            fillImage.color = Color.Lerp(Color.green, Color.red, currentPressure / 100f);
    }

    private void OnCriticalPressureReached()
    {
        //Animacija??
        uiManager.ShowWarningMessage(uiManager.CriticalPreassure, uiManager.WarningTextDuration, true, uiManager.DangerTextColor);
        uiManager.StartPreassureTimer(gameConfig.preassureOverloadDurationBeforeGameOver);
    }

    private void OnPressureBackToSafe()
    {
        uiManager.ShowWarningMessage(uiManager.SafePreassure, uiManager.WarningTextDuration, true, uiManager.WarningTextColor);
        uiManager.StopPreassureTimer();
    }

    private void CalculatePreassureOnClick()
    {
        if (isPreassurePerClickBoostActivated)
        {
            currentPressure += gameConfig.preassurePerClickUpgradeMultiplier; //1
            currentPressure = Mathf.Clamp(currentPressure, 0f, 100f);
        }
        else
        {
            currentPressure += gameConfig.pressurePerClickStandard; //3
            currentPressure = Mathf.Clamp(currentPressure, 0f, 100f);
        }
    }
    
    private void ApplyWeightLossPerClick()
    {
        if (weightManager != null)
        {
            if (isWeightLossPerClickBoostActivated)
            {
                weightManager.SubtractWeight(gameConfig.weightLossPerClickUpgradeMultiplier); //-0.5
            }
            else
            {
                weightManager.SubtractWeight(gameConfig.weightLossPerClickStandard); //-0.01
            }
        }
        else
        {
            Debug.LogWarning("Weight Manager is not set!");
        }
    }

    private void CalculatePressureDecrease()
    {
        //Check the bool isPreassureDecreaseBoostActivated and set the value of pressureDecreasePerSecond accordingly.
        float decrease = isPreassureDecreaseBoostActivated
         ? gameConfig.pressureDecreasePerSecondUpgradeMultiplier * Time.deltaTime
         : gameConfig.pressureDecreasePerSecondStandard * Time.deltaTime;

        currentPressure -= decrease;
        currentPressure = Mathf.Clamp(currentPressure, 0f, 100f);
    }


    //Method that sets the warningTeshold & criticalThreshold to a position on the slider that must not be exceeded
    private void PositionCriticalThresholdMarker()
    {
        if (pressureSlider == null)
            return;

        RectTransform sliderRect = pressureSlider.GetComponent<RectTransform>();
        float sliderWidth = sliderRect.rect.width;

        if (criticalTresholdMarker != null)
        {
            float normalizedCritical = Mathf.Clamp01(gameConfig.criticalThreshold / 100f);
            float xCritical = Mathf.Lerp(-sliderWidth / 2f, sliderWidth / 2f, normalizedCritical);
            Vector2 newPosCritical = criticalTresholdMarker.anchoredPosition;
            newPosCritical.x = xCritical;
            criticalTresholdMarker.anchoredPosition = newPosCritical;
        }

        if (preWarningThresholdMarker != null)
        {
            float normalizedPreWarning = Mathf.Clamp01(gameConfig.preWarningThreshold / 100f);
            float xPreWarning = Mathf.Lerp(-sliderWidth / 2f, sliderWidth / 2f, normalizedPreWarning);
            Vector2 newPosPreWarning = preWarningThresholdMarker.anchoredPosition;
            newPosPreWarning.x = xPreWarning;
            preWarningThresholdMarker.anchoredPosition = newPosPreWarning;
        }
    }



    //UPGRADES RELATED METHODS:
    //Method that controls the increase in the variable responsible for the rate of pressure drop during printing
    public void PreassureDecreaseBoost(bool isActive)
    {
        isPreassureDecreaseBoostActivated = isActive;
    }


    //Method that controls the reduction of the value of the variable responsible for the level of pressure increase when tapping
    public void PreassurePerClickBoost(bool isActive)
    {
        isPreassurePerClickBoostActivated = isActive;
    }

    public void WeightPerClickDrop(bool isActive)
    {
        isWeightLossPerClickBoostActivated = isActive;
    }
}

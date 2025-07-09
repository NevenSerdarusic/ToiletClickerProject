using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIActions uIActions;
    
    [Header("Text References")]
    [SerializeField] private TMP_Text coinSaldo;
    [SerializeField] private TMP_Text xpSaldo;
    [SerializeField] private TMP_Text weightTotalText;
    [SerializeField] private TMP_Text weightIndividualText;
    [SerializeField] private TMP_Text gameOverText;

    [Header("GO References")]
    [SerializeField] private GameObject warningText;

    [Header("Timers")]
    [SerializeField] private Image upgradeTimer;
    [SerializeField] private Image preassureTimer;
    [SerializeField] private TMP_Text upgradeTimerText;
    [SerializeField] private TMP_Text preassureTimerText;

    [Header("Game Background Components")]
    [SerializeField] private List<GameObject> gameBackgroundPanels;

    [Header("General UI Settings")]
    [SerializeField] private Color warningColor;
    [SerializeField] private Color dangerColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private float warningTextDuration = 2f;
    [SerializeField] private Color defaultGameBackgroundColor;
    
    private Coroutine warningCoroutine;

    
    //PROPERTIES:
    public Color DefaultTextColor => defaultColor;
    public Color WarningTextColor => warningColor;
    public Color DangerTextColor => dangerColor;
    public string CriticalPreassure => StringLibrary.criticalPressure;
    public string SafePreassure => StringLibrary.safePreassure; 
    public float WarningTextDuration => warningTextDuration;

    private void Start()
    {
        SetMainUISettings();
    }

    private void SetMainUISettings()
    {
        //Apply the default color to all main game panels
        foreach (var panelObject in gameBackgroundPanels)
        {
            Image panel = panelObject.GetComponent<Image>();
            if (panel != null)
            {
                panel.color = defaultGameBackgroundColor;
            }
        }

        //Turn OFF listed GO
        upgradeTimer.gameObject.SetActive(false);
        preassureTimer.gameObject.SetActive(false);
        upgradeTimerText.gameObject.SetActive(false);
        preassureTimerText.gameObject.SetActive(false);
    }

    //Methods for updating text components on the scene
    public void UpdateCoins(int amount)
    {
        coinSaldo.text = amount.ToString();
    }

    public void UpdateXP(int amount) 
    {
        xpSaldo.text = amount.ToString();
    }

    public void UpdateTotalWeight(float amount)
    {
        weightTotalText.text = $"{amount:F2}";
    }

    public void UpdateIndividualWeight(float amount)
    {
        weightIndividualText.text = $"{(amount > 0 ? "+" : "")}{amount:F2} KG";
        if (uIActions != null)
        {
            uIActions.AnimateWeightChange(amount);
        }
    }



    //Method for displaying a panel indicating the end of the game
    public void ShowGameOverReason(GameOverReason reason)
    {
        switch (reason)
        {
            case GameOverReason.WeightLimit:
                gameOverText.text = StringLibrary.weightLimitReached;
                break;

            case GameOverReason.PressureOverload:
                gameOverText.text = StringLibrary.preassureOverloadReached;
                break;
        }
    }


    //Timers logic and animation
    public void StartUpgradeTimer(float duration)
    {
        if (upgradeTimer == null) return;

        if (uIActions != null)
        {
            
            uIActions.AnimateUpgradeTimer(upgradeTimer, upgradeTimerText, duration, StringLibrary.upgradeTimerText);
        }
    }

    public void StartPreassureTimer(float duration)
    {
        if (preassureTimer == null) return;

        if (uIActions != null)
        {
            uIActions.AnimatePreassureTimer(preassureTimer, preassureTimerText, duration, StringLibrary.preassureTimerText);
        }

        Debug.Log("Preassure Timer Activated!");
    }

    public void StopPreassureTimer()
    {
        if (preassureTimer == null) return;

        if (uIActions != null)
        {
            uIActions.StopPreassureTimer(preassureTimer);
        }

        Debug.Log("Preassure Timer Deactivated!");
    }


    //Methods that handle warning messages related to slider loading
    public void ShowWarningMessage(string message, float duration, bool isMessageActive, Color textColor)
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }

        if (isMessageActive)
        {
            warningCoroutine = StartCoroutine(HandleWarningMessage(message, duration, textColor));
        }
        else if (warningText != null)
        {
            warningText.SetActive(false);
        }
    }

    public IEnumerator HandleWarningMessage(string message, float duration, Color textColor)
    {
        if (warningText != null)
        {
            warningText.SetActive(false);
            yield return null; //wait one frame

            TMP_Text textComponent = warningText.GetComponent<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = message;
                textComponent.color = textColor;
            }

            warningText.SetActive(true);
            yield return new WaitForSeconds(duration);
            warningText.SetActive(false);
        }
    }
}

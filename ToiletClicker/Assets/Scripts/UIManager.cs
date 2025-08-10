using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIActions uIActions;
    [SerializeField] private GameConfig gameConfig;
    
    [Header("Text References")]
    [SerializeField] private TMP_Text cpSaldo;
    [SerializeField] private TMP_Text xpSaldo;
    [SerializeField] private TMP_Text firewallProtectionTotalText;
    [SerializeField] private TMP_Text gameOverText;

    [Header("GO References")]
    [SerializeField] private GameObject warningText;

    [Header("Game Background Components")]
    [SerializeField] private List<GameObject> gameBackgroundPanels;

    [Header("General UI Settings")]
    [SerializeField] private Color successTextColor;
    [SerializeField] private Color errorTextColor;
    [SerializeField] private Color gameMainTextColor;
    [SerializeField] private Color defaultTextColor;
    [SerializeField] private float warningTextDuration = 2f;
    [SerializeField] private Color defaultGameBackgroundColor;
    
    private Coroutine warningCoroutine;

    //PROPERTIES:
    public Color DefaultTextColor => defaultTextColor;
    public Color SuccessTextColor => successTextColor;
    public Color ErrorTextColor => errorTextColor;

    public Color GameMainTextColor => gameMainTextColor;
    public string CriticalPreassure => StringLibrary.criticalDetectionLevel;
    public string SafePreassure => StringLibrary.safeDetectionLevel; 
    public float WarningTextDuration => warningTextDuration;

    private void Start()
    {
        SetupInitialization();
    }

    private void SetupInitialization()
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

        uIActions.TimersSetupInitialization();
    }

    //Update Text Components in Stats Panel
    public void UpdateCP(int amount)
    {
        cpSaldo.text = amount.ToString();
    }
    public void UpdateXP(int amount) 
    {
        xpSaldo.text = amount.ToString();
    }

    //TOTAL FIREWALL PROTECTION PERCENTAGE
    public void UpdateTotalFirewallProtection(float amount)
    {
        firewallProtectionTotalText.text = $"{amount:F2}%";
    }
    //INDIVIDUAL FIREWALL PROTECTION PERCENTAGE
    public void UpdateIndividualFirewallProtection(float amount)
    {
        if (uIActions != null)
        {
            if (amount > 0f)
            {
                uIActions.AnimateIndividualFirewallProtectionImpact(amount, errorTextColor);
            }
            else if (amount < 0f)
            {
                uIActions.AnimateIndividualFirewallProtectionImpact(amount, successTextColor);
            }
        }
        else
        {
            Debug.LogWarning("UIAction component is missing!");
        }
    }


    //Method for displaying a panel indicating the end of the game
    public void ShowGameOverReason(GameOverReason reason)
    {
        switch (reason)
        {
            case GameOverReason.FirewallTimeout:
                gameOverText.text = StringLibrary.firewallBreachFailure;
                break;

            case GameOverReason.TraceDetected:
                gameOverText.text = StringLibrary.traceOverload;
                break;
        }
    }


    //Timers logic
    public void StartUpgradeTimer(float duration)
    {
        if (uIActions != null)
        {
            uIActions.AnimateUpgradeTimer(duration, successTextColor, errorTextColor);
        }
    }

    public void StartTraceScannerTimer(float duration)
    {
        if (uIActions != null)
        {
            uIActions.AnimateTrackDetectionTimer(duration);
        }
    }

    public void StopTraceDetectionTimer()
    {
        if (uIActions != null)
        {
            uIActions.StopPreassureTimer();
        }
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
            yield return null;

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

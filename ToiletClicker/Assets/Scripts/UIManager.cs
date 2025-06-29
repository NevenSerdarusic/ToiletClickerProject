using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TMP_Text coinSaldo;
    [SerializeField] private TMP_Text xpSaldo;
    [SerializeField] private TMP_Text weightText;
    [SerializeField] private TMP_Text gameOverText;

    [Header("GO References")]
    //[SerializeField] private GameObject gameOverScreen;
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

    //Tweening:
    private int preassureTweenId = -1;

    

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
        //gameOverScreen.SetActive(false);
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

    public void UpdateWeight(float amount)
    {
        weightText.text = $"{amount:F2}";
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

        upgradeTimer.fillAmount = 1f;
        upgradeTimer.gameObject.SetActive(true);

        //Text Animation
        AnimateTimerText(upgradeTimerText.gameObject, StringLibrary.upgradeTimerText);

        //Timer Animation
        LeanTween.value(upgradeTimer.gameObject, 1f, 0f, duration)
                 .setOnUpdate((float val) => upgradeTimer.fillAmount = val)
                 .setEase(LeanTweenType.linear)
                 .setOnComplete(() => upgradeTimer.gameObject.SetActive(false));
    }

    public void StartPreassureTimer(float duration)
    {
        if (preassureTimer == null) return;

        preassureTimer.fillAmount = 0f;
        preassureTimer.gameObject.SetActive(true);

        //Text Animation
        AnimateTimerText(preassureTimerText.gameObject, StringLibrary.preassureTimerText);

        //Timer Animation
        preassureTweenId = LeanTween.value(preassureTimer.gameObject, 0f, 1f, duration)
            .setOnUpdate((float val) => preassureTimer.fillAmount = val)
            .setEase(LeanTweenType.linear)
            .setOnComplete(() =>
            {
                preassureTimer.gameObject.SetActive(false);
                preassureTweenId = -1; //Reset ID when finnish
            }).id;
    }

    public void StopPreassureTimer()
    {
        if (preassureTimer == null) return;

        //Stop tween if is active
        if (LeanTween.isTweening(preassureTweenId))
        {
            LeanTween.cancel(preassureTweenId);
            preassureTweenId = -1;
        }

        preassureTimer.gameObject.SetActive(false);
    }

    //Method that will animate timer text
    public void AnimateTimerText(GameObject textGO, string text)
    {
        if (textGO == null) return;

        TMP_Text tmpText = textGO.GetComponent<TMP_Text>();
        RectTransform rect = textGO.GetComponent<RectTransform>();

        if (tmpText == null || rect == null)
        {
            Debug.LogWarning("Text GameObject must have TMP_Text and RectTransform components.");
            return;
        }

        textGO.SetActive(true);
        tmpText.text = text;
        tmpText.alpha = 0f;
        rect.localScale = Vector3.zero;

        // Pop-in scale + fade-in
        LeanTween.scale(textGO, Vector3.one * 1.3f, 0.3f).setEaseOutBack();
        LeanTween.value(textGO, 0f, 1f, 0.3f).setOnUpdate((float a) =>
        {
            tmpText.alpha = a;
        });

        // Fade-out + scale down + deactivate
        LeanTween.delayedCall(textGO, 0.8f, () =>
        {
            LeanTween.scale(textGO, Vector3.zero, 0.2f).setEaseInBack();
            LeanTween.value(textGO, 1f, 0f, 0.2f).setOnUpdate((float a) =>
            {
                tmpText.alpha = a;
            }).setOnComplete(() =>
            {
                textGO.SetActive(false);
            });
        });
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

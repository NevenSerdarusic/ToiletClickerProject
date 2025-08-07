using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class UIActions : MonoBehaviour
{
    [Header("Individual Firewall Protection Text Settings")]
    [SerializeField] private TMP_Text individualFirewallProtectionIncreaseText;
    [SerializeField] private int startFontSize = 20;
    [SerializeField] private int endFontSize = 72;
    [SerializeField] private int startAlphaValue = 0;
    [SerializeField] private int endAlphaValue = 1;
    [SerializeField] private Vector2 startRectSize = new Vector2(120, 40);
    [SerializeField] private Vector2 endRectSize = new Vector2(300, 100);
    [SerializeField] private float animationDuration = 1f;

    [Header("Upgrade Timer Settings")]
    [SerializeField] private List<GameObject> upgradeTimers;
    [SerializeField] private TMP_Text upgradeTimerActivationText;
    [SerializeField] private TMP_Text upgradeTimerDurationText;
    [SerializeField] private float warningRemainingDurationTreshold = 5f;

    [Header("Trace Detection Timer Settings")]
    [SerializeField] private Image traceScannerTimer;
    [SerializeField] private TMP_Text traceScannerTimerActivationText;

    [Header("Processed Code Settings")]
    [SerializeField] private TMP_Text processedCodeText;
    [SerializeField] private float processedCodeTypingDuration = 2f;

    [Header("Upgrade Pointer Settings")]
    [SerializeField] private GameObject upgradePointer;
    [SerializeField] private float upgradePointerAnimationDuration = 1f;
    [SerializeField] private float upgradePointerAnimationLimiter = 50f;
    private RectTransform rectTransform;
    private Vector2 initialAnchoredPos;
    private Vector3 initialPosition;
    private int tweenId = -1;

    //Tweening:
    private int preassureTweenId = -1;
    

    //INDIVIDUAL IMPACT ON FIREWALL TEXT ANIMATION
    public void AnimateIndividualFirewallProtectionImpact(float amount, Color color)
    {
        if (individualFirewallProtectionIncreaseText == null) return;

        individualFirewallProtectionIncreaseText.text = $"{(amount > 0 ? "+" : "")}{amount:F2} %";

        individualFirewallProtectionIncreaseText.color = color;

        CanvasGroup canvasGroup = individualFirewallProtectionIncreaseText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = individualFirewallProtectionIncreaseText.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = startAlphaValue;

        RectTransform rectTransform = individualFirewallProtectionIncreaseText.GetComponent<RectTransform>();
        rectTransform.sizeDelta = startRectSize;

        individualFirewallProtectionIncreaseText.fontSize = Mathf.RoundToInt(startFontSize);

        LeanTween.alphaCanvas(canvasGroup, endAlphaValue, animationDuration);
        LeanTween.size(rectTransform, endRectSize, animationDuration).setEase(LeanTweenType.easeOutBack);

        LeanTween.value(individualFirewallProtectionIncreaseText.gameObject, startFontSize, endFontSize, animationDuration)
            .setOnUpdate((float val) =>
            {
                individualFirewallProtectionIncreaseText.fontSize = Mathf.RoundToInt(val);
            })
            .setOnComplete(() =>
            {
                LeanTween.alphaCanvas(canvasGroup, 0f, animationDuration);
                LeanTween.size(rectTransform, startRectSize, animationDuration).setEase(LeanTweenType.easeInBack);

                LeanTween.value(individualFirewallProtectionIncreaseText.gameObject, endFontSize, startFontSize, animationDuration)
                    .setOnUpdate((float val) =>
                    {
                        individualFirewallProtectionIncreaseText.fontSize = Mathf.RoundToInt(val);
                    });
            });
    }


    //TIMERS ANIMATIONS
    public void TimersSetupInitialization()
    {
        foreach (var t in upgradeTimers) t.SetActive(false);
        traceScannerTimer.gameObject.SetActive(false);
        upgradeTimerActivationText.gameObject.SetActive(false);
        traceScannerTimerActivationText.gameObject.SetActive(false);
    }
    public void AnimateUpgradeTimer(float duration, Color regularColor, Color warningColor)
    {
        var slotGO = upgradeTimers.FirstOrDefault(go => !go.activeSelf);
        if (slotGO == null)
        {
            Debug.LogWarning("No free upgrade?timer slot available!");
            return;
        }


       
        //Animate Activation Text
        AnimateTimerActivationText(upgradeTimerActivationText.gameObject, StringLibrary.upgradeTimerActivationText);

        slotGO.SetActive(true);
        upgradeTimerDurationText.color = regularColor;
        upgradeTimerDurationText.text = FormatTime(duration);


        float timeRemaining = duration;

        LeanTween.value(upgradeTimerDurationText.gameObject, duration, 0f, duration)
        .setOnUpdate((float val) =>
        {
            timeRemaining = val;
            upgradeTimerDurationText.text = FormatTime(val);

            if (val <= warningRemainingDurationTreshold)
                upgradeTimerDurationText.color = warningColor;
            else
                upgradeTimerDurationText.color = regularColor;
        })
        .setEase(LeanTweenType.linear)
        .setOnComplete(() =>
        {
            //upgradeTimerDurationText.gameObject.SetActive(false);
            slotGO.SetActive(false);
        });
    }
    public int AnimateTrackDetectionTimer(float duration, Action onComplete = null)
    {
        if (traceScannerTimer == null || traceScannerTimerActivationText == null) return -1;

        // Osiguraj da se prethodni tween zaustavi
        if (LeanTween.isTweening(preassureTweenId))
        {
            LeanTween.cancel(preassureTweenId);
        }

        traceScannerTimer.fillAmount = 0f;
        traceScannerTimer.gameObject.SetActive(true);

        AnimateTimerActivationText(traceScannerTimerActivationText.gameObject, StringLibrary.traceScannerTimerActivationText);

        preassureTweenId = LeanTween.value(traceScannerTimer.gameObject, 0f, 1f, duration)
            .setOnUpdate(val => traceScannerTimer.fillAmount = val)
            .setEase(LeanTweenType.linear)
            .setOnComplete(() =>
            {
                traceScannerTimer.gameObject.SetActive(false);
                preassureTweenId = -1;
                onComplete?.Invoke();
            }).id;

        return preassureTweenId;
    }
    public void StopPreassureTimer()
    {
        if (LeanTween.isTweening(preassureTweenId))
        {
            LeanTween.cancel(preassureTweenId);
            preassureTweenId = -1;
        }

        traceScannerTimer.fillAmount = 0f;
        traceScannerTimer.gameObject.SetActive(false);
    }
    private void AnimateTimerActivationText(GameObject textGO, string text, Action onComplete = null)
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
        tmpText.alpha = startAlphaValue;
        rect.localScale = Vector3.zero;

        LeanTween.scale(textGO, Vector3.one * 1.3f, 0.3f).setEaseOutBack();
        LeanTween.value(textGO, 0f, 1f, 0.3f).setOnUpdate((float a) =>
        {
            tmpText.alpha = a;
        });

        LeanTween.delayedCall(textGO, 0.8f, () =>
        {
            LeanTween.scale(textGO, Vector3.zero, 0.2f).setEaseInBack();
            LeanTween.value(textGO, 1f, 0f, 0.2f).setOnUpdate((float a) =>
            {
                tmpText.alpha = a;
            }).setOnComplete(() =>
            {
                textGO.SetActive(false);
                onComplete?.Invoke();
            });
        });
    }
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"{minutes:00}:{seconds:00}";
    }


    //PROCESSED CODE ANIMATIONS
    public void AnimateProcessedCodeTyping(string textToDisplay, Color textColor)
    {
        if (processedCodeText == null || string.IsNullOrEmpty(textToDisplay)) return;

        processedCodeText.text = "";
        processedCodeText.color = textColor;
        processedCodeText.alpha = 1f;

        int totalChars = textToDisplay.Length;
        float delayPerChar = processedCodeTypingDuration / totalChars;

        // Animacija po znaku
        for (int i = 0; i < totalChars; i++)
        {
            int index = i;
            LeanTween.delayedCall(gameObject, delayPerChar * i, () =>
            {
                processedCodeText.text += textToDisplay[index];
            });
        }

        // Sakrij nakon završetka animacije (alpha = 0)
        LeanTween.delayedCall(gameObject, processedCodeTypingDuration + 0.05f, () =>
        {
            Color c = processedCodeText.color;
            c.a = 0f;
            processedCodeText.color = c;
        });
    }


    //UPGRADE POINTER SCROLL ANIMATION
    public void StartUpgradePointerAnimation()
    {
        if (upgradePointer == null)
        {
            Debug.LogWarning("Target object nije postavljen!");
            return;
        }

        upgradePointer.SetActive(true);

        rectTransform = upgradePointer.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning("Nema RectTransform komponente!");
            return;
        }

        initialAnchoredPos = rectTransform.anchoredPosition;

        tweenId = LeanTween.value(upgradePointer, initialAnchoredPos.y, initialAnchoredPos.y - upgradePointerAnimationLimiter, upgradePointerAnimationDuration)
                          .setLoopPingPong()
                          .setEase(LeanTweenType.easeInOutSine)
                          .setOnUpdate((float val) =>
                          {
                              rectTransform.anchoredPosition = new Vector2(initialAnchoredPos.x, val);
                          }).id;
    }

    public void StopUpgradePointerAnimation()
    {
        if (upgradePointer == null || rectTransform == null)
            return;

        if (tweenId != -1)
        {
            LeanTween.cancel(tweenId);
            tweenId = -1;
        }

        upgradePointer.SetActive(false);

        rectTransform.anchoredPosition = initialAnchoredPos;
    }
}


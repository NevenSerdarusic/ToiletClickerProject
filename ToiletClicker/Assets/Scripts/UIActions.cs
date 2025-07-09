using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIActions : MonoBehaviour
{
    [Header("Individual Weight Gain Animation Settings")]
    [SerializeField] private TMP_Text individualWeightGainText;
    [SerializeField] private int startFontSize = 20;
    [SerializeField] private int endFontSize = 72;
    [SerializeField] private int startAlphaValue = 0;
    [SerializeField] private int endAlphaValue = 1;
    [SerializeField] private Vector2 startRectSize = new Vector2(120, 40);
    [SerializeField] private Vector2 endRectSize = new Vector2(300, 100);
    [SerializeField] private float animationDuration = 1f;


    //Tweening:
    private int preassureTweenId = -1;



    // Fade in i scale animacija za TMP_Text
    public void AnimateTimerText(GameObject textGO, string text, Action onComplete = null)
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

    // Animacija update-a težine sa fade i scale te animacijom font size
    public void AnimateWeightChange(float amount)
    {
        if (individualWeightGainText == null) return;

        individualWeightGainText.text = $"{(amount > 0 ? "+" : "")}{amount:F2} KG";

        CanvasGroup canvasGroup = individualWeightGainText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = individualWeightGainText.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = startAlphaValue;

        RectTransform rectTransform = individualWeightGainText.GetComponent<RectTransform>();
        rectTransform.sizeDelta = startRectSize;

        individualWeightGainText.fontSize = Mathf.RoundToInt(startFontSize);

        LeanTween.alphaCanvas(canvasGroup, endAlphaValue, animationDuration);
        LeanTween.size(rectTransform, endRectSize, animationDuration).setEase(LeanTweenType.easeOutBack);

        LeanTween.value(individualWeightGainText.gameObject, startFontSize, endFontSize, animationDuration)
            .setOnUpdate((float val) =>
            {
                individualWeightGainText.fontSize = Mathf.RoundToInt(val);
            })
            .setOnComplete(() =>
            {
                LeanTween.alphaCanvas(canvasGroup, 0f, animationDuration);
                LeanTween.size(rectTransform, startRectSize, animationDuration).setEase(LeanTweenType.easeInBack);

                LeanTween.value(individualWeightGainText.gameObject, endFontSize, startFontSize, animationDuration)
                    .setOnUpdate((float val) =>
                    {
                        individualWeightGainText.fontSize = Mathf.RoundToInt(val);
                    });
            });
    }

    // Animacija timer fillAmount od 1 do 0 sa fadeom
    public void AnimateUpgradeTimer(Image upgradeTimer, TMP_Text upgradeTimerText, float duration, string timerText)
    {
        if (upgradeTimer == null || upgradeTimerText == null) return;

        upgradeTimer.fillAmount = 1f;
        upgradeTimer.gameObject.SetActive(true);

        AnimateTimerText(upgradeTimerText.gameObject, timerText);

        LeanTween.value(upgradeTimer.gameObject, 1f, 0f, duration)
            .setOnUpdate(val => upgradeTimer.fillAmount = val)
            .setEase(LeanTweenType.linear)
            .setOnComplete(() => upgradeTimer.gameObject.SetActive(false));
    }

    // Animacija timer fillAmount od 0 do 1 sa fadeom i mogu?noš?u za spremanje tween ID-ja
    public int AnimatePreassureTimer(Image preassureTimer, TMP_Text preassureTimerText, float duration, string timerText, Action onComplete = null)
    {
        if (preassureTimer == null || preassureTimerText == null) return -1;

        // Osiguraj da se prethodni tween zaustavi
        if (LeanTween.isTweening(preassureTweenId))
        {
            LeanTween.cancel(preassureTweenId);
        }

        preassureTimer.fillAmount = 0f;
        preassureTimer.gameObject.SetActive(true);

        AnimateTimerText(preassureTimerText.gameObject, timerText);

        preassureTweenId = LeanTween.value(preassureTimer.gameObject, 0f, 1f, duration)
            .setOnUpdate(val => preassureTimer.fillAmount = val)
            .setEase(LeanTweenType.linear)
            .setOnComplete(() =>
            {
                preassureTimer.gameObject.SetActive(false);
                preassureTweenId = -1;
                onComplete?.Invoke();
            }).id;

        return preassureTweenId;
    }


    public void StopPreassureTimer(Image preassureTimer)
    {
        if (LeanTween.isTweening(preassureTweenId))
        {
            LeanTween.cancel(preassureTweenId);
            preassureTweenId = -1;
        }

        preassureTimer.fillAmount = 0f;
        preassureTimer.gameObject.SetActive(false);
    }

}

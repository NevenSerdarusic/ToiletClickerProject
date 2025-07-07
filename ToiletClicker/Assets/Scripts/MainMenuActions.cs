using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using System;

public class MainMenuActions : MonoBehaviour
{
    [Header("Panel/Curtain Animation Settings")]
    [SerializeField] private float curtainSlideDuration = 2f;

    [Header("Panel Button Animation Settings")]
    [SerializeField] private float buttonSlideDistance = 300f;
    [SerializeField] private float buttonSlideDuration = 0.5f;

    [Header("Panel Title Animation Settings")]
    [SerializeField] private float delayBetweenCharsShow = 0.05f;
    [SerializeField] private float delayBetweenCharsHide = 0.05f;
    [SerializeField] private float moveYAmount = 10f;
    [SerializeField] private float moveDuration = 0.3f;
    [SerializeField] private LeanTweenType showEase = LeanTweenType.easeOutBack;
    [SerializeField] private LeanTweenType hideEase = LeanTweenType.easeInCubic;

    [Header("Background Panel Animation Settings")]
    [SerializeField] private RectTransform backgroundPanelRect;
    [SerializeField] private RectTransform clickableTargetRect; 
    [SerializeField] private float rectAnimDuration = 1f;
    [SerializeField] private LeanTweenType rectAnimEase = LeanTweenType.easeInOutCubic;

    [Header("Instruction Panel Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private LeanTweenType fadeEase = LeanTweenType.easeInOutQuad;

    private const string panelTitleName = "Panel Title";
    private const string letterParentName = "Panel Title Letters Parent";
    private readonly Dictionary<RectTransform, Vector2> originalButtonPositions = new();
    private readonly Dictionary<RectTransform, Vector2> originalCharPositions = new();

    private Coroutine titleCoroutine;

    //Curtains logic
    public void PullCurtain(GameObject panelGO, float targetRight, List<RectTransform> titleChars, Action onComplete = null)
    {
        SoundManager.Instance.Play("Curtain");

        var panel = panelGO.GetComponent<RectTransform>();
        var buttons = GetButtons(panelGO);
        HideMenuButtonsAnimated(buttons);

        float startRight = panel.offsetMax.x;

        if (titleChars != null)
            HidePanelTitle(panelGO, titleChars);

        LeanTween.value(gameObject, startRight, targetRight, curtainSlideDuration)
            .setEase(LeanTweenType.easeInOutCubic)
            .setOnUpdate((float val) =>
            {
                panel.offsetMax = new Vector2(-val, panel.offsetMax.y);
            })
            .setOnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }


    public void WithdrawCurtain(GameObject panelGO, float targetRight, List<RectTransform> titleChars, Action onComplete = null)
    {

        SoundManager.Instance.Play("Curtain");
        var panel = panelGO.GetComponent<RectTransform>();
        var buttons = GetButtons(panelGO);

        float startRight = panel.offsetMax.x;
        

        LeanTween.value(gameObject, -startRight, targetRight, curtainSlideDuration)
            .setEase(LeanTweenType.easeInOutCubic)
            .setOnUpdate((float val) =>
            {
                panel.offsetMax = new Vector2(-val, panel.offsetMax.y);
            })
            .setOnComplete(() =>
            {
                ShowMenuButtonsAnimated(buttons);
                if (titleChars != null)
                    ShowPanelTitle(panelGO, titleChars);

                onComplete?.Invoke();
            });

       
    }



    //Logic for animationg buttons on MainMenu (Curtain) panel
    public void InitializeMenuButtons(GameObject panelGO)
    {
        var buttons = GetButtons(panelGO);

        foreach (var rect in buttons)
        {
            // Spremi originalnu (ciljnu) poziciju
            originalButtonPositions[rect] = rect.anchoredPosition;

            // Izračunaj skriveni položaj
            Vector2 hiddenPos = rect.anchoredPosition - new Vector2(0f, buttonSlideDistance);

            // Postavi rect na skriveni položaj i deaktiviraj
            rect.anchoredPosition = hiddenPos;
            rect.gameObject.SetActive(false);
        }
    }
    private void ShowMenuButtonsAnimated(List<RectTransform> buttons)
    {
        foreach (var rect in buttons)
        {
            rect.gameObject.SetActive(true);

            if (!originalButtonPositions.ContainsKey(rect))
                originalButtonPositions[rect] = rect.anchoredPosition;

            Vector2 originalPos = originalButtonPositions[rect];
            Vector2 hiddenPos = originalPos - new Vector2(0f, buttonSlideDistance);

            rect.anchoredPosition = hiddenPos;

            LeanTween.move(rect, originalPos, buttonSlideDuration)
                .setEase(LeanTweenType.easeOutBounce);
        }
    }

    private void HideMenuButtonsAnimated(List<RectTransform> buttons)
    {
        foreach (var rect in buttons)
        {
            if (!originalButtonPositions.ContainsKey(rect))
                originalButtonPositions[rect] = rect.anchoredPosition;

            Vector2 targetPos = rect.anchoredPosition - new Vector2(0f, buttonSlideDistance);

            LeanTween.move(rect, targetPos, buttonSlideDuration)
                .setEase(LeanTweenType.easeInBack)
                .setOnComplete(() =>
                {
                    rect.gameObject.SetActive(false);
                });
        }
    }



    //GameTitle animation
    public void ShowPanelTitle(GameObject panelGO, List<RectTransform> charlist)
    {
        if (titleCoroutine != null)
            StopCoroutine(titleCoroutine);

        titleCoroutine = StartCoroutine(AnimateIn(charlist));
    }

    public void HidePanelTitle(GameObject panelGO, List<RectTransform> charlist)
    {
        if (titleCoroutine != null)
            StopCoroutine(titleCoroutine);

        titleCoroutine = StartCoroutine(AnimateOut(charlist));
    }

    //Catch buttons from panel GO
    private List<RectTransform> GetButtons(GameObject panelGO)
    {
        var buttons = panelGO.GetComponentsInChildren<Button>(true)
                        .Select(b => b.GetComponent<RectTransform>())
                        .ToList();

        foreach (var rect in buttons)
        {
            Debug.Log(rect.gameObject.name);
        }

        return buttons;
    }

    private RectTransform FindLettersParent(GameObject panelGO)
    {
        return panelGO.GetComponentsInChildren<RectTransform>(true)
            .FirstOrDefault(rt => rt.name == letterParentName);
    }

    private TMP_Text FindPanelTitle(GameObject panelGO)
    {
        return panelGO.GetComponentsInChildren<TMP_Text>(true)
            .FirstOrDefault(t => t.name == panelTitleName);
    }

    public void GenerateCharacters(GameObject panelGO, List<RectTransform> charRects)
    {
        var titleText = FindPanelTitle(panelGO);
        var letterParent = FindLettersParent(panelGO);

        if (titleText == null || letterParent == null)
        {
            Debug.LogError("Panel Title or Letter Parent not found!");
            return;
        }

        foreach (Transform child in letterParent)
        {
            Destroy(child.gameObject);
        }

        charRects.Clear();
        string text = titleText.text;
        titleText.enabled = false;

        float totalWidth = letterParent.rect.width;
        float totalHeight = letterParent.rect.height;
        float spacing = totalWidth / text.Length;
        float startX = -totalWidth / 2f + spacing / 2f;

        for (int i = 0; i < text.Length; i++)
        {
            GameObject letterGO = new GameObject("Char_" + text[i]);
            letterGO.transform.SetParent(letterParent, false);

            var letterText = letterGO.AddComponent<TextMeshProUGUI>();
            letterText.font = titleText.font;
            letterText.fontSize = titleText.fontSize;
            letterText.color = titleText.color;
            letterText.alignment = TextAlignmentOptions.Center;
            letterText.text = text[i].ToString();
            letterText.alpha = 0;

            var rect = letterGO.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(spacing, totalHeight);
            rect.anchoredPosition = new Vector2(startX + i * spacing, 0);

            charRects.Add(rect);
            originalCharPositions[rect] = rect.anchoredPosition;
        }
    }

    //Helper methods for animating chars in PanelTitle
    private IEnumerator AnimateIn(List<RectTransform> charRects)
    {
        for (int i = 0; i < charRects.Count; i++)
        {
            var rect = charRects[i];
            var text = rect.GetComponent<TMP_Text>();

            Vector2 orig = originalCharPositions[rect];
            Vector2 start = orig - new Vector2(0, moveYAmount);

            rect.anchoredPosition = start;
            text.alpha = 0;

            LeanTween.moveY(rect, orig.y, moveDuration).setEase(showEase);
            LeanTween.value(0f, 1f, moveDuration)
                 .setOnUpdate(val => text.alpha = val)
                 .setOnComplete(() => text.alpha = 1f);

            yield return new WaitForSeconds(delayBetweenCharsShow);
        }
    }

    private IEnumerator AnimateOut(List<RectTransform> charRects)
    {
        for (int i = charRects.Count - 1; i >= 0; i--)
        {
            var rect = charRects[i];
            var text = rect.GetComponent<TMP_Text>();

            Vector2 orig = originalCharPositions[rect];
            Vector2 end = orig - new Vector2(0, moveYAmount);

            rect.anchoredPosition = orig;

            LeanTween.moveY(rect, end.y, moveDuration).setEase(hideEase);
            LeanTween.value(1f, 0f, moveDuration)
               .setOnUpdate(val => text.alpha = val)
               .setOnComplete(() => text.alpha = 0f);

            yield return new WaitForSeconds(delayBetweenCharsHide);
        }
    }

    //Animate logic of backgroundPanel
    public void AnimateBackgroundPanel(bool open)
    {
        // Closed i open offseti
        Vector2 closedMin = Vector2.zero;
        Vector2 closedMax = Vector2.zero;
        Vector2 openMin = clickableTargetRect.offsetMin;
        Vector2 openMax = clickableTargetRect.offsetMax;

        // Definiraj odakle ide i kamo ide
        Vector2 startMin = open ? closedMin : openMin;
        Vector2 endMin = open ? openMin : closedMin;
        Vector2 startMax = open ? closedMax : openMax;
        Vector2 endMax = open ? openMax : closedMax;

        // Ako zatvaramo (open==false), aktiviraj odmah
        if (!open)
            backgroundPanelRect.gameObject.SetActive(true);

        // Postavi na start
        backgroundPanelRect.offsetMin = startMin;
        backgroundPanelRect.offsetMax = startMax;

        // Pokreni tween
        LeanTween.value(gameObject, 0f, 1f, rectAnimDuration)
            .setEase(rectAnimEase)
            .setOnUpdate((float t) =>
            {
                backgroundPanelRect.offsetMin = Vector2.Lerp(startMin, endMin, t);
                backgroundPanelRect.offsetMax = Vector2.Lerp(startMax, endMax, t);
            })
            .setOnComplete(() =>
            {
                // Ako otvaramo, na kraju isključi panel
                if (open)
                    backgroundPanelRect.gameObject.SetActive(false);
            });
    }

    //Animate logic of hidding and showing subPanel of InfoPanel
    public void AnimateInstructionalPanelTransparency(GameObject panel, bool transparent)
    {
        // Uzmi ili dodaj CanvasGroup
        var cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        // Ciljna vrijednost alfa
        float targetAlpha = transparent ? 0f : 1f;

        // Pokreni LeanTween fade animaciju na CanvasGroupu
        LeanTween.alphaCanvas(cg, targetAlpha, fadeDuration)
                 .setEase(fadeEase);
    }

    //On Start Set both panel to be invisible (Right = 1080)
    public void SetPanelsStartPosition(GameObject panelGO)
    {
        if (panelGO == null)
        {
            Debug.LogWarning("GameObject je null.");
            return;
        }

        RectTransform rectTransform = panelGO.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning("RectTransform nije pronađen na GameObjectu: " + panelGO.name);
            return;
        }

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1); // ili (1, 0) ako ne rastežeš po Y

        rectTransform.offsetMin = new Vector2(0, rectTransform.offsetMin.y);       // Left = 0
        rectTransform.offsetMax = new Vector2(-1080, rectTransform.offsetMax.y);   // Right = 1080
    }
}
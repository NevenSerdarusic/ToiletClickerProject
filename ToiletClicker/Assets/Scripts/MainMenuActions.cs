using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuActions : MonoBehaviour
{

    [Header("Curtain Animation")]
    [SerializeField] private RectTransform curtainRect;
    [SerializeField] private float curtainSlideDuration = 2f;

    [Header("Buttons Animation")]
    [SerializeField] private List<RectTransform> menuButtonRects; //Buttons on MainMenu Panel
    [SerializeField] private float buttonSlideDistance = 300f;
    [SerializeField] private float buttonSlideDuration = 0.5f;
    private readonly Dictionary<RectTransform, Vector2> originalButtonPositions = new();


    private void Awake()
    {
        foreach (var rect in menuButtonRects)
        {
            originalButtonPositions[rect] = rect.anchoredPosition;
        }
    }


    //Curtains logic
    public void DrawCurtains()
    {
        HideMenuButtonsAnimated();

        float startRight = curtainRect.offsetMax.x;
        float targetRight = 1080f;

        LeanTween.value(gameObject, startRight, targetRight, curtainSlideDuration)
        .setEase(LeanTweenType.easeInOutCubic)
        .setOnUpdate((float val) =>
        {
            curtainRect.offsetMax = new Vector2(-val, curtainRect.offsetMax.y);
        })
        .setOnComplete(() =>
        {
            //Debug.Log("Curtain opened - game can start");

        });
    }
    public void PullBackCurtains()
    {
        float startRight = curtainRect.offsetMax.x;
        float targetRight = 0f;

        LeanTween.value(gameObject, -startRight, targetRight, curtainSlideDuration)
       .setEase(LeanTweenType.easeInOutCubic)
       .setOnUpdate((float val) =>
       {
           curtainRect.offsetMax = new Vector2(-val, curtainRect.offsetMax.y);
       })
       .setOnComplete(() =>
       {
           ShowMenuButtonsAnimated();
       });
    }

    //Logic for animationg buttons on MainMenu (Curtain) panel
    private void ShowMenuButtonsAnimated()
    {
        foreach (var rect in menuButtonRects)
        {
            rect.gameObject.SetActive(true);

            Vector2 originalPos = originalButtonPositions[rect];
            Vector2 hiddenPos = originalPos - new Vector2(0f, buttonSlideDistance);

            rect.anchoredPosition = hiddenPos;

            LeanTween.move(rect, originalPos, buttonSlideDuration)
                .setEase(LeanTweenType.easeOutBounce);
        }
    }

    private void HideMenuButtonsAnimated()
    {
        foreach (var rect in menuButtonRects)
        {
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


}

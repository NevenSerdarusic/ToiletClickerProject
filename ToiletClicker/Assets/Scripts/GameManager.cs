using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Status")]
    [SerializeField] private bool isGamePaused;
    [SerializeField] private bool isGameOver;
    public bool IsGamePaused => isGamePaused;
    public bool IsGameOver => isGameOver;

    [Header("References")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PreassureSystem pressureSystem;
    [SerializeField] private GameConfig config;

    [Header("Curtain Animation")]
    [SerializeField] private RectTransform curtainRect;
    [SerializeField] private float curtainSlideDuration = 1.2f;

    [Header("Menu Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button[] menuButtons;

    [Header("Buttons Animation")]
    [SerializeField] private List<RectTransform> menuButtonRects;
    [SerializeField] private float buttonSlideDistance = 300f;
    [SerializeField] private float buttonSlideDuration = 0.5f;
    private readonly Dictionary<RectTransform, Vector2> originalButtonPositions = new();


    //Coins per click settings
    private int baseCoinsPerClick;
    private float currentClickMultiplier = 1f;// Privremeni boostovi

    private int totalCoins;

    //Event with which we monitor the interactable state of the buttons in Shop
    public event System.Action<int> OnCoinsChanged;


    private void Awake()
    {
        isGamePaused = true;

        foreach (var rect in menuButtonRects)
        {
            originalButtonPositions[rect] = rect.anchoredPosition;
        }
    }

    private void Start()
    {
        baseCoinsPerClick = config.coinsPerClick;
        totalCoins = PlayerPrefsHandler.GetCoins();
        uiManager.UpdateCoins(totalCoins);

        InitializeButtonListeners();
    }

    private void InitializeButtonListeners()
    {
        if (startButton != null)
            startButton.onClick.AddListener(PlayGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);
    }

    public void RegisterClick()
    {
        //if (pressureSystem.IsOverloaded())
        //    return;

        int actualCoins = Mathf.RoundToInt(baseCoinsPerClick * currentClickMultiplier);
        totalCoins += actualCoins;

        uiManager.UpdateCoins(totalCoins);
        PlayerPrefsHandler.SetCoins(totalCoins);

        //On each click, call an event to check the status of the button in the shop
        OnCoinsChanged?.Invoke(totalCoins);

        pressureSystem.OnClick();
    }

    public void SetClickMultiplier(float multiplier)
    {
        currentClickMultiplier = multiplier;
        Debug.Log($"Click multiplier set to {currentClickMultiplier}");
    }

    public void ResetClickMultiplier()
    {
        currentClickMultiplier = 1f;
        Debug.Log("Click multiplier reset to 1");
    }

    public int GetCoinsPerClick()
    {
        return Mathf.RoundToInt(baseCoinsPerClick * currentClickMultiplier);
    }
    
    public int GetTotalCoins() => totalCoins;
   

    public void ResetCoins()
    {
        totalCoins = 0;
        //Reset Player Prefs
        OnCoinsChanged?.Invoke(totalCoins);
    }

    public void SpendCoins(int amount)
    {
        totalCoins -= amount;
        totalCoins = Mathf.Max(0, totalCoins);
        PlayerPrefsHandler.SetCoins(totalCoins);
        uiManager.UpdateCoins(totalCoins);
        OnCoinsChanged?.Invoke(totalCoins);
    }


    public void TriggerGameOver(GameOverReason reason)
    {
        if (isGameOver) return;

        isGameOver = true;
        
        uiManager.ShowGameOverScreen(reason);
    }


    //Start Game
    public void PlayGame()
    {
        isGamePaused = false;

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

    //Pause Game
    public void PauseGame()
    {
        isGamePaused = true;

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

    //Quit Game
    public void QuitGame()
    {
        Application.Quit();
    }

   
    //Methods for animationg buttons
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
}

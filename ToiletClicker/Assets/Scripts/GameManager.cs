using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private enum GameState { MainMenu, Playing, Paused, Info, GameOver }

    [Header("Game State")]
    [SerializeField] private bool isGamePaused;
    [SerializeField] private bool isGameOver;
    [SerializeField] private float currentClickMultiplier = 1f;
    [SerializeField] private int totalCoins;
    [SerializeField] private int totalXP;

    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private PreassureSystem pressureSystem;
    [SerializeField] private WeightManager weightManager;
    [SerializeField] private MainMenuActions mainMenuActions;
    [SerializeField] private ClickTarget clickTarget;

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private GameObject gameOverPanel;
    private float closedCurtain = 0f;
    private float openCurtain = 1080f;

    [Header("Menu Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button infoBackButton;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button gameOverBackButton;

    [Header("Shop/Upgrade Panels")]
    [SerializeField] private RectTransform shopPanel;
    [SerializeField] private RectTransform upgradePanel;
    [SerializeField] private Image shopButtonImage;
    [SerializeField] private Sprite shopIcon;
    [SerializeField] private Sprite upgradeIcon;
    [SerializeField] private List<GameObject> panels;


    [Header("Upgrade Buttons")]
    [SerializeField] private List<Button> upgradeButtons;

    [Header("Menu Sub Panels")]
    [SerializeField] private GameObject instructionalSubPanel;
    [SerializeField] private GameObject gameOverReasonSubPanel;

    [Header("Shop & Upgrade Panel Toogle Animation Settings")]
    [SerializeField] private float panelSlideDuration = 0.5f;


    private int baseCoinsPerClick = 1;
    
    private bool showingShop = true;
    private readonly float shownPosition = -112.5f;
    private readonly float hiddenPosition = 115f;

    private List<RectTransform> mainTitleCharRects = new();
    private List<RectTransform> infoTitleCharRects = new();
    private List<RectTransform> gameOverTitleRects = new();

    public bool IsGamePaused => isGamePaused;
    public bool IsGameOver => isGameOver;

    //Event with which we monitor the interactable state of the buttons in Healthy Food panel
    public event System.Action<int> OnCoinsChanged;
    //Event with which we monitor the interactable state of the buttons in Upgrade panel
    public event System.Action<int> OnXPChanged;


    private void OnEnable()
    {
        if (clickTarget != null)
        {
            clickTarget.OnClicked += HandleClick;
        }
    }

    private void OnDisable()
    {
        if (clickTarget != null)
        {
            clickTarget.OnClicked -= HandleClick;
        }
    }


    private void Awake()
    {
        isGamePaused = true;
        InitilizePanelsProperties();
    }

    private void Start()
    {
        totalCoins = PlayerPrefsHandler.GetCoins();
        totalXP = PlayerPrefsHandler.GetXP();
        uiManager.UpdateCoins(totalCoins);
        uiManager.UpdateXP(totalXP);

        instructionalSubPanel.GetComponent<CanvasGroup>().alpha = 0;
        gameOverReasonSubPanel.GetComponent<CanvasGroup>().alpha = 0;

        InitializeButtonListeners();

        upgradeManager.GetAllUpgradeButons().Select(upg => upg.Button).ToList();

        InitializePurchasePanels();

        OpenMainPanel();
    }

    private void InitilizePanelsProperties()
    {
        mainMenuActions.GenerateCharacters(mainPanel, mainTitleCharRects);
        mainMenuActions.GenerateCharacters(infoPanel, infoTitleCharRects);
        mainMenuActions.GenerateCharacters(gameOverPanel, gameOverTitleRects);

        mainMenuActions.SetPanelsStartPosition(mainPanel);
        mainMenuActions.SetPanelsStartPosition(infoPanel);
        mainMenuActions.SetPanelsStartPosition(gameOverPanel);

        mainMenuActions.InitializeMenuButtons(mainPanel);
        mainMenuActions.InitializeMenuButtons(infoPanel);
        mainMenuActions.InitializeMenuButtons(gameOverPanel);
    }

    private void InitializeButtonListeners()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }

        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(ToggleShopUpgradePanel);
        }

        if (infoButton != null)
        {
            infoButton.onClick.AddListener(OpenInfoPanel);
        }

        if (infoBackButton != null)
        {
            infoBackButton.onClick.AddListener(CloseInfoPanel);
        }

        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(StartGame);
        }

        if (gameOverBackButton != null)
        {
            gameOverBackButton.onClick.AddListener(CloseGameOverPanel);
        }
    }

    private void InitializePurchasePanels()
    {
        //Initial settings of shop/upgrade panels
        shopPanel.anchoredPosition = new Vector2(shownPosition, shopPanel.anchoredPosition.y);
        upgradePanel.anchoredPosition = new Vector2(hiddenPosition, upgradePanel.anchoredPosition.y);
    }

    private void HandleClick()
    {
        RegisterClick();
    }

    public void RegisterClick()
    {
        int actualCoins = Mathf.RoundToInt(baseCoinsPerClick * currentClickMultiplier);
        totalCoins += actualCoins;

        uiManager.UpdateCoins(totalCoins);
        PlayerPrefsHandler.SetCoins(totalCoins);

        //On each click, call an event to check the status of the button in the shop
        OnCoinsChanged?.Invoke(totalCoins);

        OnXPChanged?.Invoke(totalXP);

        pressureSystem.OnClick();
    }

    public void AddXP(int amount)
    {
        totalXP += amount;
        uiManager.UpdateXP(totalXP);
        PlayerPrefsHandler.SetXP(totalXP);
        OnXPChanged?.Invoke(totalXP);
    }

    public void SetClickMultiplier(float multiplier)
    {
        currentClickMultiplier = multiplier;
    }

    public int GetTotalCoins() => totalCoins;

    public int GetTotalXP() => totalXP;


    //Method that manages the withdrawal of coins after a purchase in a shop
    public void SpendCoins(int amount)
    {
        totalCoins -= amount;
        totalCoins = Mathf.Max(0, totalCoins);
        PlayerPrefsHandler.SetCoins(totalCoins);
        uiManager.UpdateCoins(totalCoins);
        OnCoinsChanged?.Invoke(totalCoins);
    }

    //Method that manages the withdrawal of XP after a purchase in a shop
    public void SpendXP(int amount)
    {
        totalXP -= amount;
        totalXP = Mathf.Max(0, totalXP);
        PlayerPrefsHandler.SetXP(totalXP);
        uiManager.UpdateXP(totalXP);
        OnXPChanged?.Invoke(totalXP);
    }


    //Two reasons for GAME OVER: 1.Overweight 2.Constant High Preassure
    public void TriggerGameOver(GameOverReason reason)
    {
        isGameOver = true;

        uiManager.ShowGameOverReason(reason);

        // Reset all upgrades
        if (TryGetComponent<UpgradeManager>(out var upgradeManager))
        {
            upgradeManager.ResetAllUpgrades();
        }

        clickTarget.BlockClicks(true);

        GameOver();

        SoundManager.Instance.Play("Final Fart");
    }

    private void SetPanelsState(bool active)
    {
        foreach (var panel in panels)
        {
            panel.SetActive(active);
        }
    }


    //Logic related to the main game buttons
    //Start Game
    public void StartGame()
    {
        isGamePaused = false;

        if (isGameOver)
        {
            mainMenuActions.PullCurtain(gameOverPanel, openCurtain, gameOverTitleRects);
            mainMenuActions.AnimateBackgroundPanel(true);
            ResetGame();
            isGameOver = false;
        }
        else
        {
            //CloseMainPanel();
            mainMenuActions.WithdrawCurtain(mainPanel, closedCurtain, mainTitleCharRects);
            mainMenuActions.AnimateBackgroundPanel(true);
            SetPanelsState(false);
        }
    }

    //Pause Game
    public void PauseGame()
    {
        isGamePaused = true;
        OpenMainPanel();
        mainMenuActions.AnimateBackgroundPanel(false);
        SetPanelsState(true);
    }

    //Panel Management
    public void OpenMainPanel()
    {
        mainMenuActions.WithdrawCurtain(mainPanel, closedCurtain, mainTitleCharRects);
    }
    private void CloseMainPanel()
    {
        mainMenuActions.PullCurtain(mainPanel, openCurtain, mainTitleCharRects);
    }

    public void OpenInfoPanel()
    {
        mainMenuActions.PullCurtain(mainPanel, openCurtain, mainTitleCharRects);

        mainMenuActions.WithdrawCurtain(infoPanel, closedCurtain, infoTitleCharRects);

        mainMenuActions.AnimateInstructionalPanelTransparency(instructionalSubPanel, false);
    }

    public void CloseInfoPanel()
    {
        mainMenuActions.AnimateInstructionalPanelTransparency(instructionalSubPanel, true);

        mainMenuActions.PullCurtain(infoPanel, openCurtain, infoTitleCharRects);

        mainMenuActions.WithdrawCurtain(mainPanel, closedCurtain, mainTitleCharRects);
    }

    public void GameOver()
    {
        mainMenuActions.WithdrawCurtain(gameOverPanel, closedCurtain, gameOverTitleRects);
        mainMenuActions.AnimateInstructionalPanelTransparency(gameOverReasonSubPanel, false);
        SetPanelsState(true);
    }

    public void PlayAgain()
    {
        mainMenuActions.PullCurtain(gameOverPanel, closedCurtain, gameOverTitleRects);
        ResetGame();
        StartGame();
        SetPanelsState(false);
    }

    private void CloseGameOverPanel()
    {
        mainMenuActions.AnimateInstructionalPanelTransparency(gameOverReasonSubPanel, true);
        mainMenuActions.PullCurtain(gameOverPanel, openCurtain, gameOverTitleRects);
        mainMenuActions.WithdrawCurtain(mainPanel, closedCurtain, mainTitleCharRects);
    }

    //Quit Game
    public void QuitGame()
    {
        Application.Quit();
    }
    //Reset Game
    private void ResetGame()
    {
        isGameOver = false;

        totalCoins = 0;
        totalXP = 0;
        weightManager.ResetTotalWeight();

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        uiManager.UpdateCoins(totalCoins);
        uiManager.UpdateXP(totalXP);
    }


    //Logic for changing Shop and Upgrade Panel
    private void ToggleShopUpgradePanel()
    {
        if (showingShop)
        {
            //Move shop panel out, show upgrade panel
            SlideOutPanel(shopPanel);
            SlideInPanel(upgradePanel);
            shopButtonImage.sprite = shopIcon;
        }
        else
        {
            //Move the upgrade panel out, show the shop panel
            SlideOutPanel(upgradePanel);
            SlideInPanel(shopPanel);
            shopButtonImage.sprite = upgradeIcon;
        }

        showingShop = !showingShop;
    }



    //Logic for animating panels: HEALTHY FOOD - UPGRADE
    private void SlideInPanel(RectTransform panel)
    {
        panel.anchoredPosition = new Vector2(hiddenPosition, panel.anchoredPosition.y);

        LeanTween.moveX(panel, shownPosition, panelSlideDuration)
            .setEase(LeanTweenType.easeOutCubic);
    }

    private void SlideOutPanel(RectTransform panel)
    {
        LeanTween.moveX(panel, hiddenPosition, panelSlideDuration)
            .setEase(LeanTweenType.easeInCubic);
    }
}

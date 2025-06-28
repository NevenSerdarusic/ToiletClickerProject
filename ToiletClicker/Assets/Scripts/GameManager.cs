using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Status")]
    [SerializeField] private bool isGamePaused;
    [SerializeField] private bool isGameOver;
    [SerializeField] private float currentClickMultiplier = 1f;

    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private PreassureSystem pressureSystem;
    [SerializeField] private MainMenuActions mainMenuActions;
    [SerializeField] private ClickTarget clickTarget;

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private float closedCurtain = 0f;
    [SerializeField] private float openCurtain = 1080f;
    
    [Header("Menu Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button backButton;

    [Header("Shop/Upgrade Panels")]
    [SerializeField] private RectTransform shopPanel;
    [SerializeField] private RectTransform upgradePanel;
    [SerializeField] private Image shopButtonImage;
    [SerializeField] private Sprite shopIcon;
    [SerializeField] private Sprite upgradeIcon;
    [SerializeField] private float panelSlideDuration = 0.5f;
    private readonly float shownPosition = -112.5f;
    private readonly float hiddenPosition = 115f;
    private bool showingShop = true;

    [Header("Upgrade Buttons")]
    [SerializeField] private List<Button> upgradeButtons;

    //Coins per click settings
    private int baseCoinsPerClick = 1;

    private int totalCoins;
    private int totalXP;

    private List<RectTransform> mainTitleCharRects = new();
    private List<RectTransform> infoTitleCharRects = new();

    public bool IsGamePaused => isGamePaused;
    public bool IsGameOver => isGameOver;

    //Event with which we monitor the interactable state of the buttons in Healthy Food panel
    public event System.Action<int> OnCoinsChanged;
    //Event with which we monitor the interactable state of the buttons in Upgrade panel
    public event System.Action<int> OnXPChanged;
    //Game over event
    public event System.Action<GameOverReason> OnGameOver;

    private void OnEnable()
    {
        if (clickTarget != null)
        {
            clickTarget.OnClicked += HandleClick;
        }

        OnXPChanged += CheckUpgradeButtonsAndToggleShopButton;
    }

    private void OnDisable()
    {
        if (clickTarget != null)
        {
            clickTarget.OnClicked -= HandleClick;
        }

        OnXPChanged -= CheckUpgradeButtonsAndToggleShopButton;
    }


    private void Awake()
    {
        isGamePaused = true;

        mainMenuActions.GenerateCharacters(mainPanel, mainTitleCharRects);
        mainMenuActions.GenerateCharacters(infoPanel, infoTitleCharRects);

        mainMenuActions.SetPanelsStartPosition(mainPanel);
        mainMenuActions.SetPanelsStartPosition(infoPanel);

        mainMenuActions.InitializeMenuButtons(mainPanel);
        mainMenuActions.InitializeMenuButtons(infoPanel);
    }

    private void Start()
    {
        totalCoins = PlayerPrefsHandler.GetCoins();
        totalXP = PlayerPrefsHandler.GetXP();
        uiManager.UpdateCoins(totalCoins);
        uiManager.UpdateXP(totalXP);

        InitializeButtonListeners();

        upgradeManager.GetAllUpgradeButons().Select(upg => upg.Button).ToList();

        InitializePurchasePanels();

        OpenMainPanel();
    }


    private void InitializeButtonListeners()
    {
        if (startButton != null)
            startButton.onClick.AddListener(PlayGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);

        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(ToggleShopUpgradePanel);

        if (infoButton != null)
        {
            infoButton.onClick.AddListener(OpenInfoPanel);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(CloseInfoPanel);
        }
    }

    private void InitializePurchasePanels()
    {
        //Initial settings of shop/upgrade panels
        shopPanel.anchoredPosition = new Vector2(shownPosition, shopPanel.anchoredPosition.y);
        upgradePanel.anchoredPosition = new Vector2(hiddenPosition, upgradePanel.anchoredPosition.y);
        upgradePanel.gameObject.SetActive(false);
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

        pressureSystem.OnClick();
    }

    public void AddXP(int amount)
    {
        totalXP += amount;
        uiManager.UpdateXP(totalXP);
        PlayerPrefsHandler.SetXP(totalXP);
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
        totalXP= Mathf.Max(0, totalXP);
        PlayerPrefsHandler.SetCoins(totalXP);
        uiManager.UpdateCoins(totalXP);
    }

    //Two reasons for GAME OVER: 1.Overweight 2.Constant High Preassure
    public void TriggerGameOver(GameOverReason reason)
    {
        isGameOver = true;
        
        uiManager.ShowGameOverScreen(reason);

        // Reset all upgrades
        if (TryGetComponent<UpgradeManager>(out var upgradeManager))
        {
            upgradeManager.ResetAllUpgrades();
        }

        clickTarget.BlockClicks(true);
    }

    //Method that controls the interactivity of the upgrade button
    private void CheckUpgradeButtonsAndToggleShopButton(int _)
    {
        foreach (var upgradeButton in upgradeButtons)
        {
            if (upgradeButton.gameObject.activeInHierarchy && upgradeButton.interactable)
            {
                purchaseButton.interactable = true;
                return;
            }
        }

        purchaseButton.interactable = false;
    }


    //Logic related to the main game buttons
    //Start Game
    public void PlayGame()
    {
        isGamePaused = false;
        CloseMainPanel();
        mainMenuActions.AnimateBackgroundPanel(true);
    }

    //Pause Game
    public void PauseGame()
    {
        isGamePaused = true;
        OpenMainPanel();
        mainMenuActions.AnimateBackgroundPanel(false);
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

        mainMenuActions.AnimateInstructionalPanelTransparency(false);
    }
 
    public void CloseInfoPanel() 
    {
        mainMenuActions.AnimateInstructionalPanelTransparency(true);

        mainMenuActions.PullCurtain(infoPanel, openCurtain, infoTitleCharRects);

        mainMenuActions.WithdrawCurtain(mainPanel, closedCurtain, mainTitleCharRects);
    }

    //Quit Game
    public void QuitGame()
    {
        Application.Quit();
    }
    //Reset Game
    public void ResetGame()
    {
        isGameOver = false;
        isGamePaused = true;

        totalCoins = 0;
        totalXP = 0;
        
        PlayerPrefsHandler.SetCoins(totalCoins);
        PlayerPrefsHandler.SetXP(totalXP);
        
        uiManager.UpdateCoins(totalCoins);
        uiManager.UpdateXP(totalXP);

        OnCoinsChanged?.Invoke(totalCoins);
        OnXPChanged?.Invoke(totalXP);

        currentClickMultiplier = 1f;
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
        panel.gameObject.SetActive(true);
        panel.anchoredPosition = new Vector2(hiddenPosition, panel.anchoredPosition.y);

        LeanTween.moveX(panel, shownPosition, panelSlideDuration)
            .setEase(LeanTweenType.easeOutCubic);
    }

    private void SlideOutPanel(RectTransform panel)
    {
        LeanTween.moveX(panel, hiddenPosition, panelSlideDuration)
            .setEase(LeanTweenType.easeInCubic)
            .setOnComplete(() => panel.gameObject.SetActive(false));
    }
}

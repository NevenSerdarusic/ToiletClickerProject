using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private enum GameState { MainMenu, Playing, Paused, Info, GameOver }
    private GameState _previousState;

    [Header("Game State")]
    [SerializeField] private bool isGamePaused;
    [SerializeField] private bool isGameOver;
    [SerializeField] private float currentClickMultiplier = 1f;

    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private PreassureSystem pressureSystem;
    [SerializeField] private WeightManager weightManager;
    [SerializeField] private MainMenuActions mainMenuActions;
    [SerializeField] private ClickTarget clickTarget;

    [Header("Panels & SubPanels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject instructionalSubPanel;
    [SerializeField] private GameObject gameOverReasonSubPanel;
    [SerializeField] private List<GameObject> panels;
    [SerializeField] private float closedCurtain = 0f;
    [SerializeField] private float openCurtain = 1080f;

    [Header("Buttons")]
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
    [SerializeField] private float panelSlideDuration = 0.5f;

    [Header("Upgrade Buttons")]
    [SerializeField] private List<Button> upgradeButtons;

    private GameState _state;
    private float _clickMultiplier = 1f;
    private int _totalCoins;
    private int _totalXP;
    private bool _showingShop = true;
    private const int BaseCoinsPerClick = 1;
    private readonly float _shopShownX = -112.5f;
    private readonly float _shopHiddenX = 115f;
    private bool shouldAnimateBackgroundPanel = false;

    private List<RectTransform> mainTitleChars = new();
    private List<RectTransform> infoTitleChars = new();
    private List<RectTransform> gameOverTitleChars = new();

    public event Action<int> OnCoinsChanged;
    public event Action<int> OnXPChanged;

    public int GetTotalCoins() => _totalCoins;
    public int GetTotalXP() => _totalXP;
    public bool IsGamePaused => isGamePaused;
    public bool IsGameOver => isGameOver;

    private void Awake()
    {
        // Prepare panels
        foreach (var panel in panels)
            panel.SetActive(false);

        // Title chars
        mainMenuActions.GenerateCharacters(mainPanel, mainTitleChars);
        mainMenuActions.GenerateCharacters(infoPanel, infoTitleChars);
        mainMenuActions.GenerateCharacters(gameOverPanel, gameOverTitleChars);

        mainMenuActions.SetPanelsStartPosition(mainPanel);
        mainMenuActions.SetPanelsStartPosition(infoPanel);
        mainMenuActions.SetPanelsStartPosition(gameOverPanel);

        mainMenuActions.InitializeMenuButtons(mainPanel);
        mainMenuActions.InitializeMenuButtons(infoPanel);
        mainMenuActions.InitializeMenuButtons(gameOverPanel);
    }

    private void OnEnable()
    {
        clickTarget.OnClicked += () => HandleClick();
        OnXPChanged += _ => upgradeManager.UpdateUpgradeButtonStates();
        weightManager.OnGameOverRequested += reason => TriggerGameOver(reason);
        pressureSystem.OnGameOverRequested += reason => TriggerGameOver(reason);
    }

    private void OnDisable()
    {
        clickTarget.OnClicked -= () => HandleClick();
        OnXPChanged -= _ => upgradeManager.UpdateUpgradeButtonStates();
        weightManager.OnGameOverRequested -= reason => TriggerGameOver(reason);
        pressureSystem.OnGameOverRequested -= reason => TriggerGameOver(reason);
    }

    private void Start()
    {
        // Load data
        _totalCoins = PlayerPrefsHandler.GetCoins();
        _totalXP = PlayerPrefsHandler.GetXP();
        uiManager.UpdateCoins(_totalCoins);
        uiManager.UpdateXP(_totalXP);

        SetCanvasAlpha(instructionalSubPanel, 0);
        SetCanvasAlpha(gameOverReasonSubPanel, 0);

        BindButtons();
        InitShopPanels();
        ChangeState(GameState.MainMenu);
    }

    private void BindButtons()
    {
        startButton.onClick.AddListener(() => ChangeState(GameState.Playing));
        quitButton.onClick.AddListener(Application.Quit);
        pauseButton.onClick.AddListener(() => ChangeState(GameState.Paused));
        infoButton.onClick.AddListener(() => ChangeState(GameState.Info));
        infoBackButton.onClick.AddListener(() => ChangeState(GameState.MainMenu));
        playAgainButton.onClick.AddListener(() => ChangeState(GameState.Playing));
        gameOverBackButton.onClick.AddListener(() => ChangeState(GameState.MainMenu));
        purchaseButton.onClick.AddListener(ToggleShopUpgrade);
    }

    private void InitShopPanels()
    {
        shopPanel.anchoredPosition = new Vector2(_shopShownX, 0);
        upgradePanel.anchoredPosition = new Vector2(_shopHiddenX, 0);
        upgradePanel.gameObject.SetActive(false);
    }

    private void ChangeState(GameState newState)
    {
        _previousState = _state;
        _state = newState;

        isGamePaused = (_state != GameState.Playing);

        // Hide all panels
        foreach (var panel in panels)
            panel.SetActive(false);

        switch (_state)
        {
            case GameState.MainMenu:
                Show(mainPanel, mainTitleChars);
                break;
            case GameState.Playing:
                Hide(mainPanel, mainTitleChars);
                // Ako dolazimo iz pauze, nemoj resetirati
                if (_previousState == GameState.MainMenu || _previousState == GameState.Paused)
                    mainMenuActions.AnimateBackgroundPanel(true);
                else
                    mainMenuActions.AnimateBackgroundPanel(false);

                if (_previousState != GameState.Paused)
                {
                    clickTarget.BlockClicks(false);
                }
                else
                {
                    clickTarget.BlockClicks(false);
                }
                break;
            case GameState.Paused:
                Show(mainPanel, mainTitleChars);
                mainMenuActions.AnimateBackgroundPanel(false);
                break;
            case GameState.Info:
                SwitchPanels(infoPanel, mainPanel, infoTitleChars, mainTitleChars);
                SetCanvasAlpha(instructionalSubPanel, 1);
                break;
            case GameState.GameOver:
                Show(gameOverPanel, gameOverTitleChars);
                SetCanvasAlpha(gameOverReasonSubPanel, 1);
                clickTarget.BlockClicks(true);
                ResetProgress();
                break;
        }
    }

    private void HandleClick()
    {
        if (_state != GameState.Playing) return;
        int gain = Mathf.RoundToInt(BaseCoinsPerClick * _clickMultiplier);
        _totalCoins += gain;
        uiManager.UpdateCoins(_totalCoins);
        PlayerPrefsHandler.SetCoins(_totalCoins);
        OnCoinsChanged?.Invoke(_totalCoins);
        pressureSystem.OnClick();
    }

    public void AddXP(int amount)
    {
        _totalXP += amount;
        uiManager.UpdateXP(_totalXP);
        PlayerPrefsHandler.SetXP(_totalXP);
        OnXPChanged?.Invoke(_totalXP);
    }

    private void ResetProgress()
    {
        _totalCoins = 0;
        _totalXP = 0;
        weightManager.ResetCurrentWeight();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        uiManager.UpdateCoins(0);
        uiManager.UpdateXP(0);
    }

    // Panel operations
    private void Show(GameObject panel, List<RectTransform> titles)
    {
        panel.SetActive(true);
        mainMenuActions.WithdrawCurtain(panel, closedCurtain, titles);
    }

    private void Hide(GameObject panel, List<RectTransform> titles, Action onComplete = null)
    {
        mainMenuActions.PullCurtain(panel, openCurtain, titles, onComplete);
    }

    private void SwitchPanels(GameObject show, GameObject hide, List<RectTransform> showTitles, List<RectTransform> hideTitles)
    {
        Hide(hide, hideTitles, () =>
        {
            Show(show, showTitles);
        });
    }

    private void ToggleShopUpgrade()
    {
        bool showShop = !upgradePanel.gameObject.activeSelf;
        var inPanel = showShop ? shopPanel : upgradePanel;
        var outPanel = showShop ? upgradePanel : shopPanel;
        shopButtonImage.sprite = showShop ? shopIcon : upgradeIcon;
        inPanel.gameObject.SetActive(true);
        LeanTween.moveX(inPanel, _shopShownX, panelSlideDuration).setEaseOutCubic();
        LeanTween.moveX(outPanel, _shopHiddenX, panelSlideDuration)
                 .setEaseInCubic()
                 .setOnComplete(() => outPanel.gameObject.SetActive(false));
        _showingShop = showShop;
    }

    private void SetCanvasAlpha(GameObject obj, float alpha)
    {
        var cg = obj.GetComponent<CanvasGroup>() ?? obj.AddComponent<CanvasGroup>();
        cg.alpha = alpha;
    }

    //Method that manages the withdrawal of coins after a purchase in a shop
    public void SpendCoins(int amount)
    {
        _totalCoins -= amount;
        _totalCoins = Mathf.Max(0, _totalCoins);
        PlayerPrefsHandler.SetCoins(_totalCoins);
        uiManager.UpdateCoins(_totalCoins);
        OnCoinsChanged?.Invoke(_totalCoins);
    }

    //Method that manages the withdrawal of XP after a purchase in a shop
    public void SpendXP(int amount)
    {
        _totalXP -= amount;
        _totalXP = Mathf.Max(0, _totalXP);
        PlayerPrefsHandler.SetCoins(_totalXP);
        uiManager.UpdateCoins(_totalXP);
    }


    public void SetClickMultiplier(float multiplier)
    {
        currentClickMultiplier = multiplier;
    }

    private void TriggerGameOver(GameOverReason reason)
    {
        ChangeState(GameState.GameOver);
        uiManager.ShowGameOverReason(reason);
        clickTarget.BlockClicks(true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum PanelType { Main, Info, Settings,GameOver, GameCompleted }

[Serializable]
public class ButtonAction
{
    public Button button;
    public UnityEvent onClick;
}

[Serializable]
public class GamePanel
{
    public PanelType type;
    public GameObject panel;
    public List<RectTransform> titleCharRects;
    public List<GameObject> subPanels;
}

public static class SaveManager
{
    private const string CPKey = "CP";
    private const string XPKey = "XP";

    public static int CP
    {
        get => PlayerPrefs.GetInt(CPKey, 0);
        set { PlayerPrefs.SetInt(CPKey, value); PlayerPrefs.Save(); }
    }

    public static int XP
    {
        get => PlayerPrefs.GetInt(XPKey, 0);
        set { PlayerPrefs.SetInt(XPKey, value); PlayerPrefs.Save(); }
    }
}

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] private bool isGamePaused = true;
    [SerializeField] private bool isGameOver;
    [SerializeField] private float currentClickMultiplier = 1f;
    [SerializeField] private int totalCP;
    [SerializeField] private int totalXP;

    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private UIActions uiActions;
    [SerializeField] private TraceScannerManager traceScannerManager;
    [SerializeField] private FirewallManager firewallManager;
    [SerializeField] private MainMenuActions mainMenuActions;
    [SerializeField] private ClickTarget clickTarget;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private EncryptedDataPoolManager encryptedDataPoolManager;


    [Header("Menu Buttons & Actions")]
    [SerializeField] private List<ButtonAction> menuButtons;

    [Header("Menu Panels")]
    [SerializeField] private List<GamePanel> gamePanels;
    [SerializeField] private List<GameObject> panels;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private float closedPanelPosition = 0f;
    [SerializeField] private float openPanelPosition = 1080f;

    [Header("Shop/Upgrade Panels")]
    [SerializeField] private RectTransform shopPanel;
    [SerializeField] private RectTransform upgradePanel;
    [SerializeField] private Image purchaseButtonImage;
    [SerializeField] private Sprite shopIcon;
    [SerializeField] private Sprite upgradeIcon;
    [SerializeField] private float panelSlideDuration = 0.5f;

    [Header("Sub-Panel Settings")]
    [SerializeField] private float subPanelDelay = 0.5f;

    private PanelType? currentActivePanel = null;
    private int baseCoinsPerClick = 1;
    private bool showingShop = true;
    private const float shownPosition = -112.5f;
    private const float hiddenPosition = 115f;

    private ButtonAction playButtonAction;

    // Events
    public event Action<int> OnCoinsChanged;
    public event Action<int> OnXPChanged;

    public int GetTotalCoins() => totalCP;

    public int GetTotalXP() => totalXP;

    public bool IsGamePaused => isGamePaused;
    public bool IsGameOver => isGameOver;

    private void OnEnable()
    {
        clickTarget.OnClicked += HandleClick;
        traceScannerManager.OnGameOverRequested += TriggerGameOver;
        firewallManager.OnGameOverRequested += TriggerGameOver;
        firewallManager.OnNonProtectionAchived += TriggerGameCompleted;
    }

    private void OnDisable()
    {
        clickTarget.OnClicked -= HandleClick;
        traceScannerManager.OnGameOverRequested -= TriggerGameOver;
        firewallManager.OnGameOverRequested -= TriggerGameOver;
        firewallManager.OnNonProtectionAchived -= TriggerGameCompleted;
    }

    private void Awake()
    {
        isGamePaused = true;
        BindMenuButtons();
        playButtonAction = menuButtons
           .FirstOrDefault(ba => ba.button.name == "Play Button");
    }

    private void Start()
    {
        totalCP = SaveManager.CP;
        totalXP = SaveManager.XP;
        uiManager.UpdateCP(totalCP);
        uiManager.UpdateXP(totalXP);
        InitializeGamePanels();
        InitializePurchasePanels();

        foreach (var c in gamePanels)
        {
            // Hide sub-panels
            c.subPanels?.ForEach(sp => sp.SetActive(false));
        }

        ShowPanel(PanelType.Main);
    }

    private void InitializeGamePanels()
    {
        foreach (var c in gamePanels)
        {
            mainMenuActions.GenerateCharacters(c.panel, c.titleCharRects);
            mainMenuActions.SetPanelsStartPosition(c.panel);
            mainMenuActions.InitializeMenuButtons(c.panel);
        }
    }

    private void BindMenuButtons()
    {
        foreach (var ba in menuButtons)
        {
            if (ba.button != null && ba.onClick != null)
                ba.button.onClick.AddListener(() => ba.onClick.Invoke());
        }
    }

    private void InitializePurchasePanels()
    {
        shopPanel.anchoredPosition = new Vector2(shownPosition, shopPanel.anchoredPosition.y);
        upgradePanel.anchoredPosition = new Vector2(hiddenPosition, upgradePanel.anchoredPosition.y);
    }

    private void HandleClick()
    {
        RegisterClick();
    }

    public void RegisterClick()
    {
        int coinsGained = Mathf.RoundToInt(baseCoinsPerClick * currentClickMultiplier);
        totalCP += coinsGained;
        UpdateCoins();
        traceScannerManager.OnClick();
    }

    public void AddXP(int amount)
    {
        totalXP += amount;
        UpdateXP();
    }

    public void SetClickMultiplier(float multiplier) => currentClickMultiplier = multiplier;

    private void UpdateCoins()
    {
        SaveManager.CP = totalCP;
        uiManager.UpdateCP(totalCP);
        OnCoinsChanged?.Invoke(totalCP);
    }

    private void UpdateXP()
    {
        SaveManager.XP = totalXP;
        uiManager.UpdateXP(totalXP);
        OnXPChanged?.Invoke(totalXP);
    }

    public void SpendCoins(int amount)
    {
        totalCP = Mathf.Max(0, totalCP - amount);
        UpdateCoins();
    }

    public void SpendXP(int amount)
    {
        totalXP = Mathf.Max(0, totalXP - amount);
        UpdateXP();
    }

    public void TriggerGameOver(GameOverReason reason)
    {
        if (isGameOver) return;
        isGameOver = true;
        isGamePaused = true;

        SetPanelsActive(true);
        ShowPanel(PanelType.GameOver);
        uiManager.ShowGameOverReason(reason);
        clickTarget.BlockClicks(true);
        SoundManager.Instance.PlayLoopingSound("Alarm");
        firewallManager.CheckAndSaveBestScore();
        upgradeManager.ResetAllUpgrades();
        firewallManager.UpdateBestScore();
        mainMenuActions.StartLightAnimation();
    }

    public void StartGame()
    {
        isGamePaused = false;

        if (isGameOver)
        {
            HidePanel(currentActivePanel ?? PanelType.Main);
            ResetGame();
            isGameOver = false;
            clickTarget.BlockClicks(false);
        }
        else
        {
            HidePanel(PanelType.Main);
        }

        ShowBackground(true);
        SetPanelsActive(false, mainMenuActions.CurtainSlideDuration);
        //change text inside button on PLAY
        if (playButtonAction != null)
        {
            var txt = playButtonAction.button.GetComponentInChildren<TMP_Text>();
            if (txt != null)
                txt.text = "Play";
        }
    }


    public void PauseGame()
    {
        isGamePaused = true;
        SetPanelsActive(true);
        ShowPanel(PanelType.Main);
        ShowBackground(false);
        //change text inside button on CONTINUE
        if (playButtonAction != null)
        {
            var txt = playButtonAction.button
                        .GetComponentInChildren<TMP_Text>();
            if (txt != null) txt.text = "Continue";
        }
    }

    public void OpenMainPanel() => ShowPanel(PanelType.Main);
    public void CloseMainPanel() => HidePanel(PanelType.Main);
    public void OpenInfoPanel() => ShowPanel(PanelType.Info);
    public void CloseInfoPanel() => HidePanel(PanelType.Info);
    public void OpenGameOverPanel() => ShowPanel(PanelType.GameOver);
    public void CloseGameOverPanel() => HidePanel(PanelType.GameOver);
    public void OpenGameCompletedPanel() => ShowPanel(PanelType.GameCompleted);
    public void CloseGameCompletedPanel() => HidePanel(PanelType.GameCompleted);
    public void CloseSettingsPanel() => HidePanel(PanelType.Settings);
    public void OpenSettingsPanel() => ShowPanel(PanelType.Settings);



    private void ShowPanel(PanelType type)
    {
        currentActivePanel = type;

        var c = gamePanels.First(x => x.type == type);
        mainMenuActions.WithdrawPanel(c.panel, closedPanelPosition, c.titleCharRects);
        if (c.subPanels != null && c.subPanels.Count > 0)
            StartCoroutine(EnableSubPanelsWithDelay(c.subPanels)); //Enable SubPanels on panel
    }

    private IEnumerator EnableSubPanelsWithDelay(List<GameObject> subPanels)
    {
        yield return new WaitForSeconds(subPanelDelay);
        subPanels.ForEach(sp => sp.SetActive(true));
    }

    private void HidePanel(PanelType type)
    {
        var c = gamePanels.First(x => x.type == type);
        c.subPanels?.ForEach(sp => sp.SetActive(false)); //Disable SubPanels on panel
        mainMenuActions.PullPanel(c.panel, openPanelPosition, c.titleCharRects);
    }


    private void ShowBackground(bool show) => mainMenuActions.AnimateBackgroundPanel(show);

    private void SetPanelsActive(bool active, float delay = 0f)
    {
        StopAllCoroutines();
        StartCoroutine(DoSetPanels(active, delay));
    }

    private IEnumerator DoSetPanels(bool active, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        panels.ForEach(p => p.SetActive(active));
    }

    public void ToggleShopUpgradePanel()
    {
        var inPanel = showingShop ? shopPanel : upgradePanel;
        var outPanel = showingShop ? upgradePanel : shopPanel;

        SlideOutPanel(inPanel);
        SlideInPanel(outPanel);

        if (outPanel == upgradePanel)
        {
            uiActions.StartUpgradePointerAnimation();
        }
        if (inPanel == upgradePanel)
        {
            uiActions.StopUpgradePointerAnimation();
        }

        purchaseButtonImage.sprite = showingShop ? shopIcon : upgradeIcon;
        showingShop = !showingShop;
    }

    private void SlideInPanel(RectTransform panel)
    {
        panel.anchoredPosition = new Vector2(hiddenPosition, panel.anchoredPosition.y);
        LeanTween.moveX(panel, shownPosition, panelSlideDuration).setEase(LeanTweenType.easeOutCubic);
    }

    private void SlideOutPanel(RectTransform panel)
    {
        LeanTween.moveX(panel, hiddenPosition, panelSlideDuration).setEase(LeanTweenType.easeInCubic);
    }

    public void QuitGame() => Application.Quit();

    private void ResetGame()
    {
        totalCP = 0;
        totalXP = 0;
        PlayerPrefsHandler.SetXP(0);
        PlayerPrefsHandler.SetCP(0);
        firewallManager.ResetTotalFirewallProtection();
        traceScannerManager.ResetTraceDetection();
        UpdateCoins();
        UpdateXP();
    }

    public void StopAlarm()
    {
        SoundManager.Instance.StopLoopingSound("Alarm");
    }

    public void SetMusicMuted() => musicManager?.ToggleBackgroundMusicMute();

    //Game Passed - Ideal weight achived
    private void TriggerGameCompleted()
    {
        if (isGameOver) return;
        isGameOver = true;
        isGamePaused = true;

        SetPanelsActive(true);

        ShowPanel(PanelType.GameCompleted);

        //Play Sound

        firewallManager.CheckAndSaveBestScore();

        upgradeManager.ResetAllUpgrades();

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        PlayerPrefsHandler.SetGameCompleted(true);

        firewallManager.UpdateBestScore();

        //Start Animation
    }
}
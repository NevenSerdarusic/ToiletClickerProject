using System;
using UnityEngine;

public class FirewallManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameManager gameManager;

    private float currentFirewallProtection;

    public event Action<GameOverReason> OnGameOverRequested;

    public event Action<float> OnFirewallProtectionChanged;

    public event Action OnNonProtectionAchived;

    private void Start()
    {
        //If no default firewall protection is set, set the initial one from GameConfig
        if (!PlayerPrefs.HasKey("Firewall"))
        {
            currentFirewallProtection = gameConfig.Current.startingFirewallProtection;
            PlayerPrefsHandler.SetFirewallProtection(currentFirewallProtection);
        }
        else
        {
            currentFirewallProtection = PlayerPrefsHandler.GetFirewallProtection(gameConfig.Current.startingFirewallProtection);
        }

        UpdateTotalFirewallProtectionUI();
    }

    public void AddFirewallProtection(float amount)
    {
        currentFirewallProtection += amount;
        PlayerPrefsHandler.SetFirewallProtection(currentFirewallProtection);
        
        UpdateTotalFirewallProtectionUI();
        OnFirewallProtectionChanged?.Invoke(currentFirewallProtection);

        if (currentFirewallProtection >= gameConfig.Current.maxFirewallProtection)
        {
            OnGameOverRequested?.Invoke(GameOverReason.FirewallTimeout);
        }
    }

    public void SubtractFirewallProtection(float amount)
    {
        currentFirewallProtection -= amount;
        currentFirewallProtection = Mathf.Max(0f, currentFirewallProtection); //Protect negative number
        PlayerPrefsHandler.SetFirewallProtection(currentFirewallProtection);
        UpdateTotalFirewallProtectionUI();
        OnFirewallProtectionChanged?.Invoke(currentFirewallProtection);

        if (currentFirewallProtection <= gameConfig.Current.nonFirewallProtection)
        {
            OnNonProtectionAchived?.Invoke();
        }
    }


    private void UpdateTotalFirewallProtectionUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateTotalFirewallProtection(currentFirewallProtection);
        }
    }

    public void UpdateIndividualFirewallProtectionUI(float amount)
    {
        if (uiManager != null) 
        {
            uiManager.UpdateIndividualFirewallProtection(amount);
        }
    }

    public void ResetTotalFirewallProtection()
    {
        currentFirewallProtection = gameConfig.Current.startingFirewallProtection;
        //PlayerPrefsHandler.SetFirewallProtection(currentFirewallProtection);
        UpdateTotalFirewallProtectionUI();
    }

}

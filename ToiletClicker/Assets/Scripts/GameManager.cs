using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PreassureSystem pressureSystem;
    [SerializeField] private GameConfig config;

    //Coins per click settings
    private int baseCoinsPerClick;
    private float currentClickMultiplier = 1f;// Privremeni boostovi

    private int totalCoins;

    //Event with which we monitor the interactable state of the buttons in Shop
    public event System.Action<int> OnCoinsChanged;


    private void Start()
    {
        baseCoinsPerClick = config.coinsPerClick;
        totalCoins = PlayerPrefsHandler.GetCoins();
        uiManager.UpdateCoins(totalCoins);
    }

    public void RegisterClick()
    {
        if (pressureSystem.IsOverloaded())
            return;

        int actualCoins = Mathf.RoundToInt(baseCoinsPerClick * currentClickMultiplier);
        totalCoins += actualCoins;

        uiManager.UpdateCoins(totalCoins);
        PlayerPrefsHandler.SetCoins(totalCoins);

        //On each click, call an event to check the status of the button in the shop
        OnCoinsChanged?.Invoke(totalCoins);

        pressureSystem.OnClick();
    }


    //public void AddPermanentClickValue(int amount)
    //{
    //    baseCoinsPerClick += amount;
    //    Debug.Log($"Permanent baseCoinsPerClick increased to {baseCoinsPerClick}");
    //}

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
}

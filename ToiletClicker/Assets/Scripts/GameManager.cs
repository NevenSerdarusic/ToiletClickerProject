using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PreassureSystem pressureSystem;
    [SerializeField] private GameConfig config;

    [Header("Coin Settings")]
    //[SerializeField] private int coinsPerClick = 1;
    private int baseCoinsPerClick;         // Trajna vrijednost (npr. putem upgradea)
    private float currentClickMultiplier = 1f; // Privremeni boostovi

    private int totalCoins;

    private void Start()
    {
        baseCoinsPerClick = config.coinsPerClick;
        totalCoins = PlayerPrefsHandler.GetCoins();
        uiManager.UpdateCoins(totalCoins);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }


    private void HandleClick()
    {
        if (pressureSystem.IsOverloaded())
            return;

        int actualCoins = Mathf.RoundToInt(baseCoinsPerClick * currentClickMultiplier);
        totalCoins += actualCoins;

        uiManager.UpdateCoins(totalCoins);

        //Save to PP
        PlayerPrefsHandler.SetCoins(totalCoins);

        pressureSystem.OnClick(); // puni slider
    }

    public void AddPermanentClickValue(int amount)
    {
        baseCoinsPerClick += amount;
        Debug.Log($"Permanent baseCoinsPerClick increased to {baseCoinsPerClick}");
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
   

    //Reset
    public void ResetCoins()
    {
        totalCoins = 0;
        //Reset Player Prefs
        Debug.Log("Coins reset.");
    }

    public void SpendCoins(int amount)
    {
        totalCoins -= amount;
        totalCoins = Mathf.Max(0, totalCoins);
        PlayerPrefsHandler.SetCoins(totalCoins);
        uiManager.UpdateCoins(totalCoins);
    }
}

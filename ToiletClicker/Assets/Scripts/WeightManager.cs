using System;
using UnityEngine;

public class WeightManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameManager gameManager;

    private float currentWeight;

    public event Action<GameOverReason> OnGameOverRequested;

    public event Action<float> OnWeightChanged;

    public event Action OnIdealWeightAchived;

    public float GetCurrentWeight() => currentWeight;

    private void Start()
    {
        //If no default weight is set, set the initial one from GameConfig
        if (!PlayerPrefs.HasKey("PlayerWeight"))
        {
            currentWeight = gameConfig.startingWeight;
            PlayerPrefsHandler.SetWeight(currentWeight);
        }
        else
        {
            currentWeight = PlayerPrefsHandler.GetWeight();
        }

        UpdateTotalWeightUI();

        UpdateBestScore();
    }

    public void AddWeight(float amount)
    {
        currentWeight += amount;
        PlayerPrefsHandler.SetWeight(currentWeight);
        
        UpdateTotalWeightUI();
        OnWeightChanged?.Invoke(currentWeight);

        if (currentWeight >= gameConfig.maxWeight)
        {
            OnGameOverRequested?.Invoke(GameOverReason.WeightLimit);
        }
    }

    public void SubtractWeight(float amount)
    {
        currentWeight -= amount;
        currentWeight = Mathf.Max(0f, currentWeight); //Protect negative number
        PlayerPrefsHandler.SetWeight(currentWeight);
        UpdateTotalWeightUI();
        OnWeightChanged?.Invoke(currentWeight);

        if (currentWeight <= gameConfig.idealWeight)
        {
            OnIdealWeightAchived?.Invoke();
        }
    }


    private void UpdateTotalWeightUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateTotalWeight(currentWeight);
        }
    }

    public void UpdateIndividualWeightUI(float amount)
    {
        if (uiManager != null) 
        {
            uiManager.UpdateIndividualWeight(amount);
        }
    }

    public void UpdateBestScore()
    {
        if (uiManager != null)
        {
            uiManager.UpdateBestScoreWeight(gameConfig.startingWeight);
        }
    }

    public void ResetTotalWeight()
    {
        currentWeight = gameConfig.startingWeight;
        UpdateTotalWeightUI();
    }

    //Method for checking and saving the lowest point of weight in any played game, as a BestScore
    public void CheckAndSaveBestScore()
    {
        float best = PlayerPrefsHandler.GetBestScoreWeight(gameConfig.startingWeight);

        if (currentWeight < best)
        {
            PlayerPrefsHandler.SetBestScoreWeight(currentWeight);
        }
    }

}

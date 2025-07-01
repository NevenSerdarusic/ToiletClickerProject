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
    }

    public void AddWeight(float amount)
    {
        currentWeight += amount;
        PlayerPrefsHandler.SetWeight(currentWeight);
        
        UpdateTotalWeightUI();

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

    public void ResetTotalWeight()
    {
        currentWeight = gameConfig.startingWeight;
        UpdateTotalWeightUI();
    }
}

using System;
using TMPro;
using UnityEngine;

public class WeightManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private UIManager uiManager;


    private float currentWeight;

    private void Start()
    {
        //If no difficulty is set, set the initial one from GameConfig
        if (!PlayerPrefs.HasKey("PlayerWeight"))
        {
            currentWeight = gameConfig.startingWeight;
            PlayerPrefsHandler.SetWeight(currentWeight);
        }
        else
        {
            currentWeight = PlayerPrefsHandler.GetWeight();
        }

        UpdateWeightUI();
    }

    public void AddWeight(float amount)
    {
        currentWeight += amount;
        PlayerPrefsHandler.SetWeight(currentWeight);
        
        UpdateWeightUI();

        if (currentWeight >= gameConfig.maxWeight)
        {
            OnGameOver();
        }
    }

    private void OnGameOver()
    {
        Debug.Log("Game Over! Maximum difficulty reached.");
        uiManager.ShowGameOverScreen();
    }

    private void UpdateWeightUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateWeight(currentWeight);
        }
    }

    public float GetCurrentWeight()
    {
        return currentWeight;
    }
}

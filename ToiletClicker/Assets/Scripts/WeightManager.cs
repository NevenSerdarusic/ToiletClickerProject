using System;
using TMPro;
using UnityEngine;

public class WeightManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameManager gameManager;

    private float currentWeight;

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

        UpdateWeightUI();
    }

    public void AddWeight(float amount)
    {
        currentWeight += amount;
        PlayerPrefsHandler.SetWeight(currentWeight);
        
        UpdateWeightUI();

        if (currentWeight >= gameConfig.maxWeight)
        {
            gameManager.TriggerGameOver(GameOverReason.WeightLimit);
        }
    }

    public void SubtractWeight(float amount)
    {
        currentWeight -= amount;
        currentWeight = Mathf.Max(0f, currentWeight); //Protect negative number
        PlayerPrefsHandler.SetWeight(currentWeight);
        UpdateWeightUI();
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

    public void ResetCurrentWeight()
    {
        currentWeight = 0f;
    }
}

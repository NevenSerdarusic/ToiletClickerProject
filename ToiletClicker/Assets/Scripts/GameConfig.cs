using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Player Settings")]
    public float startingWeight = 450f;
    public float maxWeight = 500f;
    public float idealWeight = 80f;

    [Header("Coin Settings")]
    public int coinsPerClick = 1;

    [Header("Preassure Settings")]
    public float pressurePerClick = 5f;
    public float pressureDecreasePerSecond = 10f;
    public float criticalThreshold = 95f;

    [Header("Click Boost Settings")]
    public float boostDuration = 10f;
    public int boostMultiplier = 2;
    public float boostAppearMinTime = 10f;
    public float boostAppearMaxTime = 30f;
}

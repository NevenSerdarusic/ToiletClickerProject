using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Player Weight Settings")]
    public float startingWeight = 450f;
    public float maxWeight = 500f;
    public float idealWeight = 80f;

    [Header("Weight Ranges")]
    public float range1Min = 80f;
    public float range1Max = 120f;
    public float range2Min = 121f;
    public float range2Max = 160f;
    public float range3Min = 161f;
    public float range3Max = 200f;
    public float range4Min = 201f;

    [Header("XP Settings")]
    public int xpPerHealthyFoodEaten = 5;

    [Header("Preassure Settings")]
    public float pressurePerClickStandard = 5f;
    public float pressureDecreasePerSecondStandard = 2f;
    public float preWarningThreshold = 80f;
    public float criticalThreshold = 95f;
    public float preassureOverloadDurationBeforeGameOver = 3f;

    [Header("Weight Loss Settings")]
    public float weightLossPerClickStandard = 0.01f;

    [Header("Upgrades Settings")]
    public float autoClickInterval = 0.5f;
    public float duobleTapMultiplier = 2;
    public float megaTapMultiplier = 5;
    public int laxativeWeightReduction = 5;
    public int detoxBombReplacingSlotCount = 5;
    public float snackLagMultiplier = 0.5f;
    public float fiberOverloadBonusAmount = 3.5f;
    public float junkFoodFatReduction = 1f;//
    public float junkFoodSugarReduction = 1f;//
    public float pressureDecreasePerSecondUpgradeMultiplier = 10f;
    public float preassurePerClickUpgradeMultiplier = 1f;
    public float weightLossPerClickUpgradeMultiplier = 0.5f;
}

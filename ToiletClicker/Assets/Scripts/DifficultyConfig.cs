using UnityEngine;

[CreateAssetMenu(fileName = "NewDifficultyConfig", menuName = "Configs/DifficultyConfig")]
public class DifficultyConfig : ScriptableObject
{
    [Header("Firewall Settings")]
    public float startingFirewallProtection = 70f;
    public float maxFirewallProtection = 100f;
    public float nonFirewallProtection = 0f;

    [Header("Trace Detection Settings")]
    public float detectionPerClickStandard = 5f;
    public float detectionDecreasePerSecondStandard = 2f;
    public float preWarningThreshold = 80f;
    public float criticalThreshold = 95f;
    public float detectionOverloadDurationBeforeGameOver = 3f;

    [Header("Firewall Decrease Settings")]
    public float firewallDecreasePerClickStandard = 0.01f;

    [Header("Clickable Trap Button Settings")]
    public int minPositiveFirewallValue = 1;
    public int maxPositiveFirewallValue = 5;
    public int minNegativeFirewallValue = 1;
    public int maxNegativeFirewallValue = 10;

    [Header("Upgrades Settings")]
    public float autoClickInterval = 0.5f;
    public float doubleTapMultiplier = 2;
    public float megaTapMultiplier = 5;
    public int firewallProtectionDecreaseImpact = 5;
    public int codeReplacingSlotCount = 5;
    public float scrollRectDecreaseSpeedMultiplier = 0.5f;
    public float encryptedCodeOptimizationMultiplier = 3.5f;
    public float encryptedCodeJunkMultiplier = 1f;
    public float encryptedCodeDetectionMultiplier = 1f;
    public float detectionFillMultiplier = 10f;
    public float detectionDrainMultiplier = 1f;
    public float firewallProtectionDecreasePerClickMultiplier = 0.5f;
}

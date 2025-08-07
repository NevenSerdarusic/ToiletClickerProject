using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Firewall Settings")]
    public float startingFirewallProtection = 70f;
    public float maxFirewallProtection = 100f;
    public float nonFirewallProtection = 0f;

    [Header("Preassure Settings")]
    public float pressurePerClickStandard = 5f;
    public float pressureDecreasePerSecondStandard = 2f;
    public float preWarningThreshold = 80f;
    public float criticalThreshold = 95f;
    public float preassureOverloadDurationBeforeGameOver = 3f;

    [Header("Firewall Decrease Settings")]
    public float firewallDecreasePerClickStandard = 0.01f;

    [Header("Clickable Trap Button Settings")]
    public int minFirewallIncreaseValue = 1;
    public int maxFirewallIncreaseValue = 20;

    [Header("Upgrades Settings")]
    public float autoClickInterval = 0.5f;
    public float duobleTapMultiplier = 2;
    public float megaTapMultiplier = 5;
    public int firewallProtectionDecreaseImpact = 5;
    public int codeReplacingSlotCount = 5;
    public float scrollRectDecreaseSpeedMultiplier = 0.5f;
    public float encryptedCodeOptimizationMultiplier = 3.5f;
    public float encryptedCodeJunkMultiplier = 1f;
    public float encryptedCodeDetectionMultiplier = 1f;
    public float detectionPerSecondMultiplier = 10f;
    public float preassurePerClickUpgradeMultiplier = 1f;
    public float firewallProtectionDecreasePerClickMultiplier = 0.5f;
}

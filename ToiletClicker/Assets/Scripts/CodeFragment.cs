using UnityEngine;


[CreateAssetMenu(fileName = "NewCode", menuName = "Code/CodeFragment")]
public class CodeFragment : ScriptableObject
{
    public string fragmentDisplayText;
    public int cost;
    public float junkDensity;
    public float detectionRisk;
    public float optimizationValue;
    public bool isEncrypted;

    [Tooltip("XP awarded when this code fragment is used (only for decoded fragments)")]
    public int XPValue;

    private const float junkCoefficient = 0.025f;
    private const float detectionCoefficient = 0.025f;
    private const float optimizationCoefficient = 0.25f;


    public float firewallProtectionImpact => (junkDensity * junkCoefficient) + (detectionRisk * detectionCoefficient) - (optimizationValue * optimizationCoefficient);

    public float FirewallProtectionImpact => firewallProtectionImpact;

}

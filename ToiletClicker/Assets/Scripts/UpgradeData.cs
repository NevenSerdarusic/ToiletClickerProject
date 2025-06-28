using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public UpgradeType type;
    public string upgradeName;
    public int upgradePrice;
    public float upgradeDuration;
    public Sprite upgradeIcon;
    public string description;
    public bool isInstant; //if upgrade has intant impact, without timer
}

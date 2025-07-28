using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public UpgradeType type;
    public int upgradeLevel;
    public string upgradeName;
    public int upgradePrice;
    public float upgradeDuration;
    public Sprite upgradeIcon;
    public string description;
    public bool isInstant; //if upgrade has intant impact, without timer


    //method for each upgrade
    public virtual void ApplyUpgrade()
    {

    }
}

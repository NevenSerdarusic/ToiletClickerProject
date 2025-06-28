using UnityEngine;

public static class PlayerPrefsHandler
{
    //KEYS:
    private const string CoinsKey = "TotalCoins";
    private const string WeightKey = "PlayerWeight";
    private const string XPKey = "TotalXP";



    // ----------- COINS -----------
    public static int GetCoins()
    {
        return PlayerPrefs.GetInt(CoinsKey, 0);
    }

    public static void SetCoins(int amount)
    {
        PlayerPrefs.SetInt(CoinsKey, amount);
    }

    // ----------- WEIGHT -----------
    public static float GetWeight()
    {
        return PlayerPrefs.GetFloat(WeightKey, 450f); // default je 450kg
    }

    public static void SetWeight(float weight)
    {
        PlayerPrefs.SetFloat(WeightKey, weight);
    }

    // ----------- XP -----------
    public static int GetXP()
    {
        return PlayerPrefs.GetInt(XPKey, 0);
    }

    public static void SetXP(int amount)
    {
        PlayerPrefs.SetInt(XPKey, amount);
    }

}

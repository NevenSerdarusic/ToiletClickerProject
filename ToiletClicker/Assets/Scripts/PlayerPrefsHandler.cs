using UnityEngine;

public static class PlayerPrefsHandler
{
    //KEYS:
    private const string CoinsKey = "TotalCoins";
    private const string WeightKey = "PlayerWeight";
    private const string XPKey = "TotalXP";
    private const string FartVolumeKey = "FartVolume";
    private const string MusicMutedKey = "MusicMuted";  // 0 = not muted, 1 = muted
    private const string BestScoreWeightKey = "BestScoreWeight";
    private const string GameCompletedKey = "GameCompleted";


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

    // ----------- FART VOLUME -----------
    public static float GetFartVolume()
    {
        return PlayerPrefs.GetFloat(FartVolumeKey, 0.5f); // Default: 0.5f
    }

    public static void SetFartVolume(float value)
    {
        PlayerPrefs.SetFloat(FartVolumeKey, value);
    }


    // ----------- MAIN MUSIC MUTE STATUS -----------
    public static bool IsMusicMuted()
    {
        return PlayerPrefs.GetInt(MusicMutedKey, 0) == 1;
    }

    public static void SetMusicMuted(bool isMuted)
    {
        PlayerPrefs.SetInt(MusicMutedKey, isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }



    // ----------- BEST SCORE (lowest weight) -----------
    public static float GetBestScoreWeight(float defaultWeight)
    {
        return PlayerPrefs.GetFloat(BestScoreWeightKey, defaultWeight); 
    }

    public static void SetBestScoreWeight(float weight)
    {
        PlayerPrefs.SetFloat(BestScoreWeightKey, weight);
        PlayerPrefs.Save();
    }


    // ----------- GAME COMPLETION STATUS -----------
    public static bool IsGameCompleted()
    {
        return PlayerPrefs.GetInt(GameCompletedKey, 0) == 1;
    }

    public static void SetGameCompleted(bool completed)
    {
        PlayerPrefs.SetInt(GameCompletedKey, completed ? 1 : 0);
        PlayerPrefs.Save();
    }

}

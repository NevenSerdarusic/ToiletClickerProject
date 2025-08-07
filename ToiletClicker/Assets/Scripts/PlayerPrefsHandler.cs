using UnityEngine;

public static class PlayerPrefsHandler
{
    //KEYS:
    private const string CPKey = "TotalCP";
    private const string FirewallProtectionKey = "Firewall";
    private const string XPKey = "TotalXP";
    private const string MusicMutedKey = "MusicMuted";
    private const string BestScoreFirewallDecreaseKey = "BestScoreFirewall";
    private const string GameCompletedKey = "GameCompleted";
    private const string TypingVolumeKey = "TypingVolume";


    // ----------- CP -----------
    public static int GetCP()
    {
        return PlayerPrefs.GetInt(CPKey, 0);
    }

    public static void SetCP(int amount)
    {
        PlayerPrefs.SetInt(CPKey, amount);
    }

    // ----------- FIREWALL -----------
    public static float GetFirewallProtection(float startingFirewallProtection)
    {
        return PlayerPrefs.GetFloat(FirewallProtectionKey, startingFirewallProtection);
    }

    public static void SetFirewallProtection(float amount)
    {
        PlayerPrefs.SetFloat(FirewallProtectionKey, amount);
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
    public static float GetBestScoreFirewallDecrease(float defaultFirewall)
    {
        return PlayerPrefs.GetFloat(BestScoreFirewallDecreaseKey, defaultFirewall); 
    }

    public static void SetBestScoreFirewallDecrease(float amount)
    {
        PlayerPrefs.SetFloat(BestScoreFirewallDecreaseKey, amount);
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

    // ----------- SOUND VOLUME (EFFECTS / TYPING / SLIDER) -----------
    public static float GetTypingVolume()
    {
        return PlayerPrefs.GetFloat(TypingVolumeKey, 0.5f); // Default: full volume
    }

    public static void SetTypingVolume(float value)
    {
        PlayerPrefs.SetFloat(TypingVolumeKey, value);
        PlayerPrefs.Save();
    }
}

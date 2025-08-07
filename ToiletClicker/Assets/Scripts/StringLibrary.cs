using UnityEngine;

public static class StringLibrary
{
    //Trace Detection Reached
    public const string criticalDetectionLevel = "TRACE ALERT! You've triggered critical system pressure.\nLay off the clicks or risk getting traced!";
    public const string safeDetectionLevel = "System stabilized. You're safe to continue cracking.";

    //Timers Activated
    public const string upgradeTimerActivationText = "Upgrade Timer Activated!";
    public const string traceScannerTimerActivationText = "Trace Scanner Timer Activated!";

    //Game Over Reasons
    public const string firewallBreachFailure = "SYSTEM LOCKDOWN! \n" +
    "You failed to bypass the firewall in time—security protocols have shut you out.\n" +
    "Tip: Act quickly and invest Crack Points in decryption speed upgrades to avoid future blocks.";

    public const string traceOverload = "TRACE COMPLETE! \n" +
    "You've been tracked. System pressure exceeded safe limits and you were traced back.\n" +
    "Use trace-jamming tools and moderate your click bursts to stay hidden longer.\n" +
    "Hack smart—too much noise and you're an easy catch.";

    //Processed Code Display
    public const string encryptedCodeMessageDisplay = "DENIED";
    public const string decodedCodeMessageDispay = "SUCCESS";
}

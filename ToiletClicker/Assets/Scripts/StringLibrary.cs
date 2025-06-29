using UnityEngine;

public static class StringLibrary
{
    //Preassure Reached
    public const string criticalPressure = "CRITICAL PRESSURE! You need to stop pushing";
    public const string safePreassure = "Pressure stabilized. You can continue pushing.";

    //Timers Activated
    public const string upgradeTimerText = "Upgrade activated";
    public const string preassureTimerText = "Preassure activated";

    //Game Over Reasons
    public const string weightLimitReached = " Game Over! \n" +
    "You’ve tipped the scales beyond their limit—your weight has reached its peak.\n" +
    "Remember, every great journey is built on balance. Try pacing your taps and swapping junk for healthy bites to keep your numbers in check!\n" +
    "Next round, focus on lightening the load early—your future self will thank you.";


    public const string preassureOverloadReached = " Game Over! \n" +
    "You lingered too long on the porcelain throne—your pressure gauge has officially blown past safe levels.\n" +
    "Tip: Fast relief comes from smart upgrades. Use pressure?slowdown and auto?click boosts to keep your meter calm.\n" +
    "Moderation is key—dial back the pressure and you’ll last longer in the next session!";
}

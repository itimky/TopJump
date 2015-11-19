using UnityEngine;
using System.Collections;


public static class PlayerPrefManager
{
    public const int DefaultDuration = 10;

    public static int GetDuration(string tag)
    {
//        if (tag == TagManager.Bubble)
//            return 20;
        if (tag == TagManager.CoinX2 || tag == TagManager.ScoreX2)
            return 5;

        var duration = PlayerPrefs.GetInt(TagToDurationKey(tag));
        if (duration == 0)
        {
            return DefaultDuration;
        }

        return duration;
    }


    static string TagToDurationKey(string tag)
    {        
        return tag + PlayerPrefKeys.Duration;
    }

    public static int GetUpgradeLevel(string tag)
    {
        var level = PlayerPrefs.GetInt(tag + PlayerPrefKeys.UpgradeLevel);
        return level;
    }

    public static void SaveUpgradeLevel(string tag, int level)
    {
        PlayerPrefs.SetInt(tag + PlayerPrefKeys.UpgradeLevel, level);
        PlayerPrefs.Save();
    }

    public static int GetMaxScore()
    {
        return PlayerPrefs.GetInt(PlayerPrefKeys.MaxScore);
    }

    public static void SaveMaxScore(int maxScore)
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.MaxScore, maxScore);
        PlayerPrefs.Save();
    }
}

public static class PlayerPrefKeys
{
    // Collected
    public const string MaxScore = "MaxScore";
    public const string Gold = "Gold";

    // Durations
    public const string Duration = "Duration";
    public const string UpgradeLevel = "UpgradeLevel";
}

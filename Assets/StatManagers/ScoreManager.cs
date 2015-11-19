using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class ScoreManager : MonoBehaviour, IGameOverHandler
{
    public static int Score { get; private set; }

    private static Text ScoreText;
    private static int multiplier;
    private float lastPlatformLine;

    private const int defaultMultiplier = 1;

    public GameObject ScoreMultiplier;
    static MultiplierPanel MultiplierPanel;

    void Start()
    {
        Score = 0;
        lastPlatformLine = 0;
        ScoreText = GetComponent<Text>();
        MultiplierPanel = new MultiplierPanel(ScoreMultiplier);
        ResetMultiplier();
        Game.RegisterGameOverHandler(this);
    }

    void Update()
    {
        if (PlayerInfo.maxTraveled - lastPlatformLine > PlatformFactory.VerticalDistance)
            UpdateScore();
    }

    public void OnGameOver()
    {
        if (Score > PlayerPrefManager.GetMaxScore())
            PlayerPrefManager.SaveMaxScore(Score);
    }

    public static void SetMultiplier(int multiplier)
    {
        ScoreManager.multiplier *= multiplier;
        MultiplierPanel.Enable();
        MultiplierPanel.SetMultiplier(ScoreManager.multiplier);
        DrawScore();
    }

    public static void ResetMultiplier()
    {
        multiplier = defaultMultiplier;
        MultiplierPanel.Disable();
        DrawScore();
    }

    private void UpdateScore()
    {
        var diff = PlayerInfo.maxTraveled - lastPlatformLine;
        var count = (int)(diff / PlatformFactory.VerticalDistance);
        lastPlatformLine += count * PlatformFactory.VerticalDistance;
        Score += count * multiplier;

        DrawScore();
    }

    private static void DrawScore()
    {
//        if (multiplier == defaultMultiplier)
//        {
        var scoreStr = Score.ToString();
        while (scoreStr.Length < 6)
            scoreStr = "0" + scoreStr;
        ScoreText.text = scoreStr;
//        }
//        else
//        {
//            ScoreText.text = string.Format("Score x{0}: {1}", multiplier, Score);
//        }
    }
}

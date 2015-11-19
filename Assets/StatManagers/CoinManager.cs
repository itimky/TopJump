using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinManager : MonoBehaviour, IGameOverHandler
{
    private static Text GoldText;

    public static int Gold { get; private set; }

    private static int multiplier;
    private const int defaultMultiplier = 1;

    public GameObject CoinMultiplier;
    static MultiplierPanel MultiplierPanel;

    void Start()
    {
        Gold = 0;
        GoldText = GetComponent<Text>();
        MultiplierPanel = new MultiplierPanel(CoinMultiplier);
        ResetMultiplier();
        Game.RegisterGameOverHandler(this);
    }


    public void OnGameOver()
    {
        var totalGold = PlayerPrefs.GetInt(PlayerPrefKeys.Gold) + Gold;
        PlayerPrefs.SetInt(PlayerPrefKeys.Gold, totalGold);
    }


    public static void SetMultiplier(int multiplier)
    {
        CoinManager.multiplier *= multiplier;
        MultiplierPanel.Enable();
        MultiplierPanel.SetMultiplier(CoinManager.multiplier);
        DrawScore();
    }

    public static void ResetMultiplier()
    {
        MultiplierPanel.Disable();
        multiplier = defaultMultiplier;
        DrawScore();
    }

    public static void AddGold(GameObject gold)
    {
        CoinFactory.GoldBeenUsed(gold);
        Gold += multiplier;
        DrawScore();
    }

    private static void DrawScore()
    {
//        if (multiplier == defaultMultiplier)
        GoldText.text = Gold.ToString();
//        else
//        {
//            GoldText.text = string.Format("Gold x{0}: {1}", multiplier, Gold);
//        }
    }
}

public class MultiplierPanel
{
    readonly GameObject panel;
    Text text;

    private Text Text { get { return text ?? (text = panel.GetComponentInChildren<Text>()); } }

    bool isEnabled;

    public MultiplierPanel(GameObject panel)
    {
        this.panel = panel;
//        text = panel.GetComponentInChildren<Text>();
    }

    public void Enable()
    {
        if (isEnabled)
            return;
        
        isEnabled = true;
        panel.SetActive(true);

    }

    public void Disable()
    {
        if (!isEnabled)
            return;
        
        isEnabled = false;
        panel.SetActive(false);
    }

    public void SetMultiplier(int multiplier)
    {
        Text.text = "x" + multiplier;

        if (multiplier < 8)
            Text.color = Color.white;
        else if (multiplier < 32)
            Text.color = Color.yellow;
        else
            Text.color = Color.red;
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour, IGameOverHandler
{
    private static Text GoldText;

    public static int Gold { get; private set; }

    private static int multiplier;
    private const int defaultMultiplier = 1;

    public GameObject CoinMultiplier;
    static MultiplierPanel MultiplierPanel;

    public AudioClip CoinPickup1;
    public AudioClip CoinPickup2;
    public AudioClip CoinPickup3;
    private static List<AudioClip> CoinPickups;
    private static AudioSource audioSource;

    void Start()
    {
        Gold = 0;
        GoldText = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
//        AudioSource.Sp
        CoinPickups = new List<AudioClip>(){ CoinPickup1, CoinPickup2, CoinPickup3 };
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
        var pickupAudio = CoinPickups[MoveController.currentTileNum];
        audioSource.PlayOneShot(pickupAudio, 0.1f);
//        audioSource.Play();

//        AudioSource.PlayClipAtPoint(CoinPickAudio, gold.transform.position);
//        AudioSource.PlayClipAtPoint()
//        CoinPick.Play();
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
        panel.transform.localScale = new Vector3(1, 1, 1);
//        panel.SetActive(true);

    }

    public void Disable()
    {
        if (!isEnabled)
            return;
        
        isEnabled = false;
        panel.transform.localScale = Vector3.zero;
//        panel.SetActive(false);
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
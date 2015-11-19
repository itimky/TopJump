using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UpgradeShop : MonoBehaviour
{
    public Text JetPackUpgradePrice;
    public Text JetPackLevelText;
    public Button JetPackUpgrateButton;

    static Dictionary<int, int> LevelToPrices;

    static int maxLevel;

    void Start()
    {
        PlayerPrefManager.SaveUpgradeLevel(TagManager.JetPack, 0);
        LevelToPrices = new Dictionary<int, int>();
        LevelToPrices.Add(1, 500);
        LevelToPrices.Add(2, 1000);
        LevelToPrices.Add(3, 5000);
        LevelToPrices.Add(4, 10000);
        LevelToPrices.Add(5, 25000);
        LevelToPrices.Add(6, 50000);

        maxLevel = LevelToPrices.Count;

        InitJetPack();
    }

    private void InitJetPack()
    {
        var jetPackLevel = PlayerPrefManager.GetUpgradeLevel(TagManager.JetPack);
        JetPackLevelText.text = jetPackLevel.ToString();
        if (jetPackLevel >= maxLevel)
        {
            JetPackUpgrateButton.gameObject.SetActive(false);
            JetPackUpgradePrice.gameObject.SetActive(false);
            return;
        }
        
        JetPackUpgradePrice.text = LevelToPrices[jetPackLevel + 1].ToString();
    }


    public void UpgradeJetPack()
    {
        var jetPackLevel = PlayerPrefManager.GetUpgradeLevel(TagManager.JetPack);
        if (jetPackLevel >= maxLevel)
            return;

        PlayerPrefManager.SaveUpgradeLevel(TagManager.JetPack, jetPackLevel + 1);
        InitJetPack();
    }
}

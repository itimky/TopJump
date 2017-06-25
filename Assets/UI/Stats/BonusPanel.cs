using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BonusPanel : MonoBehaviour
{
    public RectTransform CoinBonus;
    public RectTransform ScoreBonus;
    public RectTransform MagnetBonus;
    public RectTransform BubbleBonus;
    public RectTransform JetPackBonus;

    static RectTransform Panel;
    static List<BonusItem> UsingBonuses;
    static List<BonusItem> AllBonuses;

    void Start()
    {
        UsingBonuses = new List<BonusItem>();
        AllBonuses = new List<BonusItem>();
        Panel = GetComponent<RectTransform>();

        AllBonuses.Add(MakeBonusItem(Game.MakePrefabInstance(CoinBonus)));
        AllBonuses.Add(MakeBonusItem(Game.MakePrefabInstance(ScoreBonus)));
        AllBonuses.Add(MakeBonusItem(Game.MakePrefabInstance(MagnetBonus)));
        AllBonuses.Add(MakeBonusItem(Game.MakePrefabInstance(BubbleBonus)));
        AllBonuses.Add(MakeBonusItem(Game.MakePrefabInstance(JetPackBonus)));
    }

    private BonusItem MakeBonusItem(RectTransform bonus)
    {
        return new BonusItem(bonus, bonus.Find("ScaleSlider").GetComponent<Slider>());
    }

    public static void AddBonus(string tag)
    {
        var bonus = GetBonusByTag(tag);
        if (!UsingBonuses.Contains(bonus))
        {
            UsingBonuses.Add(bonus);
            bonus.RectTransform.SetParent(Panel, false);
        }
        bonus.SetRemain(1);
    }

    public static void RemoveBonus(string tag)
    {
        var bonus = GetBonusByTag(tag);
        bonus.RectTransform.SetParent(null, false);
        UsingBonuses.Remove(bonus);
    }

    public static void SetRemain(string tag, float norm)
    {
        var bonus = GetBonusByTag(tag);
        bonus.SetRemain(norm);
    }

    static BonusItem GetBonusByTag(string tag)
    {
        return AllBonuses.First(b => b.CompareTag(tag));
    }
}

class BonusItem
{
    public readonly RectTransform RectTransform;
    public readonly Slider Slider;

    public BonusItem(RectTransform rectTransform, Slider slider)
    {
        RectTransform = rectTransform;
        Slider = slider;
    }

    public bool CompareTag(string tag)
    {
        return RectTransform.CompareTag(tag);
    }

    public void SetRemain(float norm)
    {
        if (norm > 1)
            throw new UnityException("InvalidSLiderValue (> 1)");

        Slider.value = norm;

        if (norm > 0.5f)
            Slider.GetComponentInChildren<UnityEngine.UI.Image>().color = Color.Lerp(Color.yellow, Color.green, norm * 2 - 1);
        else
            Slider.GetComponentInChildren<UnityEngine.UI.Image>().color = Color.Lerp(Color.red, Color.yellow, norm * 2);
    }
}
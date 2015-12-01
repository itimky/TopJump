using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlatformBonusManager : MonoBehaviour
{

    public GameObject JetPackPrefab;
    public GameObject BubblePrefab;
    public GameObject MagnetPrefab;

    //    List<GameObject> AllBonuses;
    static List<GameObject> AvailableBonuses;
    static Dictionary<GameObject, GameObject> PlatformToBonus;
    //    static PlatformBonusManager Instance;

    // Use this for initialization
    void Start()
    {
//        Instance = this;
        PlatformToBonus = new Dictionary<GameObject, GameObject>();
        AvailableBonuses = new List<GameObject>();
        AvailableBonuses.Add(Game.MakePrefabInstance(JetPackPrefab));
        AvailableBonuses.Add(Game.MakePrefabInstance(BubblePrefab));
        AvailableBonuses.Add(Game.MakePrefabInstance(MagnetPrefab));
    }



    public static void OnPlatformCreated(GameObject platform)
    {
        if (PlatformToBonus.ContainsKey(platform))
        {
            var releasedBonus = PlatformToBonus[platform];
            PlatformToBonus.Remove(platform);
            AvailableBonuses.Add(releasedBonus);
        }

        if (!AvailableBonuses.Any())
            return;

        var random = Random.Range(42, 42);
        if (random != 42)
            return;
            
        var bonus = AvailableBonuses[Random.Range(0, AvailableBonuses.Count)];
        AvailableBonuses.Remove(bonus);
        PlatformToBonus.Add(platform, bonus);
        var yAdd = platform.GetComponent<Renderer>().bounds.size.y / 2 + bonus.GetComponent<Renderer>().bounds.size.y / 2 + 1;
        bonus.transform.localPosition = new Vector2(platform.transform.localPosition.x, platform.transform.localPosition.y + yAdd);
        bonus.SetActive(true);
    }

    public static void BonusUsed(GameObject bonus)
    {        
        PlatformToBonus.Remove(PlatformToBonus.First(p => p.Value == bonus).Key);
        bonus.SetActive(false);
    }

    public static void BonusExpired(GameObject bonus)
    {
        AvailableBonuses.Add(bonus);
    }
    //
    //    static IEnumerator BonusExpiring(GameObject bonus, int duration)
    //    {
    //        yield return new WaitForSeconds(duration * Time.timeScale);
    //        OnBonusExpired(bonus);
    //    }
    //
    //    static void OnBonusExpired(GameObject bonus)
    //    {
    //        AvailableBonuses.Add(bonus);
    //    }
}

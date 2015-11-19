using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class FallingBonusManager : Pausable
{
    public GameObject ScoreX2Prefab;
    public GameObject GoldX2Prefab;
    //    public GameObject JetPackPrefab;
    //    public GameObject DoubleJumpPrefab;

    public float BonusActiveSeconds;
    public float RecycleOffset;

    private static List<GameObject> Bonuses;
    private static List<GameObject> ActiveBonuses;
    private static List<GameObject> VisibleBonuses;


    private List<float> BonusPositions;

    void Start()
    {
        Game.RegisterPausableObject(this);
        Bonuses = new List<GameObject>();
        ActiveBonuses = new List<GameObject>();
        VisibleBonuses = new List<GameObject>();

        BonusPositions = new List<float>();
        var platformPositions = Game.positions;
        for (int i = 1; i < platformPositions.Count; i++)
            BonusPositions.Add((platformPositions[i - 1] + platformPositions[i]) / 2);

        Bonuses.Add(Instantiate(ScoreX2Prefab));
        Bonuses.Add(Instantiate(GoldX2Prefab));
//        Bonuses.Add(Instantiate(JetPackPrefab));
        RegisterRepeating();
    }


    void Update()
    {
        foreach (var visibleBonus in VisibleBonuses.ToList())
            if (visibleBonus.transform.localPosition.y + RecycleOffset < PlayerInfo.maxTraveled)
            {
                visibleBonus.SetActive(false);
                VisibleBonuses.Remove(visibleBonus);
            }
    }

    void CreateBonus()
    {
        var random = Random.Range(0, 10);

        GameObject bonus = null;
        if (random == 9)
            bonus = Bonuses.First(b => b.CompareTag(TagManager.ScoreX2));
        else if (random == 0)
            bonus = Bonuses.First(b => b.CompareTag(TagManager.CoinX2));
        
        if (VisibleBonuses.Contains(bonus))
            return;

        if (bonus == null)
            return;

        ShowFallingBonus(bonus);
    }

    private void ShowFallingBonus(GameObject bonus)
    {
        SetPosition(bonus.transform);
        bonus.SetActive(true);
        var rigidBody = bonus.transform.GetComponent<Rigidbody2D>();
        rigidBody.AddForce(new Vector2(0, -7), ForceMode2D.Impulse);
        VisibleBonuses.Add(bonus);
    }

    private void SetPosition(Transform transform)
    {

        var xPos = BonusPositions[Random.Range(0, BonusPositions.Count)];
        var yPos = PlayerInfo.maxTraveled + 50f;
        transform.localPosition = new Vector2(xPos, yPos);
    }

    public void AddBonus(GameObject bonus)
    {
        bonus.SetActive(false);
        VisibleBonuses.Remove(bonus);

        if (ActiveBonuses.Contains(bonus))
            readdedBonuses.Add(bonus);
        else
        {
            var duration = PlayerPrefManager.GetDuration(bonus.tag) * Time.timeScale;
            StartCoroutine(ProcessMultiplier(bonus, duration));
        }
    }

    private List<GameObject> readdedBonuses = new List<GameObject>();

    private IEnumerator ProcessMultiplier(GameObject bonus, float duration)
    {
        ApplyBonus(bonus);            

        var timeLeft = duration;
        var updateRate = 0.01f * Time.timeScale;
        while (true)
        {
            if (!Game.IsPaused && !Game.IsGameOver)
            {
                if (readdedBonuses.Contains(bonus))
                {
                    readdedBonuses.Remove(bonus);
                    ApplyBonus(bonus);
                    timeLeft = duration;
                }

                if (timeLeft <= 0)
                    break;
                else
                {
                    timeLeft -= updateRate;
//                    BonusPanel.SetRemain(bonus.tag, timeLeft / duration);
                }
            }
            yield return new WaitForSeconds(updateRate);
        }


        RemoveBonus(bonus);
    }

    private void ApplyBonus(GameObject bonus)
    {
        if (!ActiveBonuses.Contains(bonus))
            ActiveBonuses.Add(bonus);

        if (bonus.CompareTag(TagManager.ScoreX2))
            ScoreManager.SetMultiplier(2);
        else if (bonus.CompareTag(TagManager.CoinX2))
            CoinManager.SetMultiplier(2);        
    }

    private void RemoveBonus(GameObject bonus)
    {
        if (bonus.CompareTag(TagManager.ScoreX2))
            ScoreManager.ResetMultiplier();
        else if (bonus.CompareTag(TagManager.CoinX2))
            CoinManager.ResetMultiplier();
        
        ActiveBonuses.Remove(bonus);
    }

    Dictionary<Rigidbody2D, Vector2> prepauseVelocities;

    public override void Pause()
    {
        base.Pause();
        CancelRepeating();
        prepauseVelocities = new Dictionary<Rigidbody2D, Vector2>();
        foreach (var rb in VisibleBonuses.Select(vb => vb.GetComponent<Rigidbody2D>()))
        {
            prepauseVelocities.Add(rb, rb.velocity);
            rb.Sleep();
        }
    }

    public override void Resume()
    {
        base.Resume();
        foreach (var rb in VisibleBonuses.Select(vb => vb.GetComponent<Rigidbody2D>()))
        {
            rb.WakeUp();
            rb.velocity = prepauseVelocities[rb];
        }
        RegisterRepeating();
    }



    private void RegisterRepeating()
    {
        InvokeRepeating("CreateBonus", 1f, 5f); 
    }

    private void CancelRepeating()
    {
        CancelInvoke("CreateBonus");
    }
}


public struct ActiveBonus
{
    public readonly GameObject GameObj;
    public readonly System.DateTime ActivationTime;

    public ActiveBonus(GameObject gameObj)
    {
        GameObj = gameObj;
        ActivationTime = System.DateTime.Now;
    }
}
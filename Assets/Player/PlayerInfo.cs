using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerInfo : MonoBehaviour
{
    public static float maxTraveled;

    private static List<GameObject> activeBonuses;
    private Transform tr;

    public static bool IsInvulnerable { get; private set; }

    private bool HaveLost;

    void Awake()
    {
//        enabled = false;
        //46859
        //паспорт
        //регистрация
        //паспорт т с
        //кредитный договор 1 и посл стр
        //копия птс
        //акт передачи птс в банк
        //скан осаго и каско
        //
        //
        //
        //фио, дата рожд, скан прав.
        //
        //до 19:00
        //
        tr = transform;
        maxTraveled = 0;
        activeBonuses = new List<GameObject>();
//        Game.RegisterPausableObject(this);
    }

    void Update()
    {

//        var currentY = transform.localPosition.y;
//        if (maxTraveled < currentY)
        maxTraveled = tr.localPosition.y;

        if (HaveLost)
            GameOver();
//
//        if (CheckGameOver())
//            GameOver();
    }

    void OnLevelWasLoaded(int level)
    {
        if (level != 1)
            enabled = false;
    }


    void OnBecameInvisible()
    {
        HaveLost = true;
    }

    private bool CheckGameOver()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (!GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds))
            return true; // Player not in camera      

        return false;
    }

    public void GameOver()
    {
        HaveLost = false;
        Game.GameOver();
    }


    public void AddBonus(GameObject bonus)
    {
        if (bonus.CompareTag(TagManager.JetPack))
            UseJetPack(bonus);
        else if (bonus.CompareTag(TagManager.Bubble))
            UseBubble(bonus);
        else if (bonus.CompareTag(TagManager.Magnet))
            UseMagnet(bonus);
    }


    public void MakeDamage()
    {
        if (HasBubble())
        {
            var bubbleBonus = activeBonuses.FirstOrDefault(b => b.CompareTag(TagManager.Bubble));
            var bubble = transform.Find("Bubble").gameObject;
            StopBonusUsage(bubbleBonus, bubble);
            StartCoroutine(Invulnerable());
        }
        else
            GameOver();
    }


    public static bool HasJetPack()
    {
        return activeBonuses.Any(b => b.CompareTag(TagManager.JetPack));
    }


    public static bool HasBubble()
    {
        return activeBonuses.Any(b => b.CompareTag(TagManager.Bubble));
    }


    IEnumerator Invulnerable()
    {
        IsInvulnerable = true;
        bool isTransparent = false;
        for (int i = 0; i < 5 * 3; i++)
        {
            yield return new WaitForSeconds(0.2f * Time.timeScale);
            if (isTransparent)
                SetTransparency(1f);
            else
                SetTransparency(0.3f);

            isTransparent = !isTransparent;
        }
        SetTransparency(1f);
        IsInvulnerable = false;
    }

    void SetTransparency(float alpha)
    {
        var renderer = GetComponent<Renderer>();
        var mat = renderer.material;
        renderer.material.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
    }


    void UseJetPack(GameObject bonus)
    {
        var rigidBody2d = GetComponent<Rigidbody2D>();
        rigidBody2d.gravityScale = 0;
        rigidBody2d.velocity = new Vector2(rigidBody2d.velocity.x, 20);
        var jetPack = transform.Find("JetPackFlight").gameObject;
        StartBonusUsageCoroutine(bonus, jetPack, () => rigidBody2d.gravityScale = 1);
    }


    void UseBubble(GameObject bonus)
    {
        if (IsInvulnerable)
        {
            StopCoroutine("Invulnerable");
            IsInvulnerable = false;
        }
        var bubble = transform.Find("Bubble").gameObject;
        StartBonusUsageCoroutine(bonus, bubble);
    }

    void UseMagnet(GameObject bonus)
    {
        var magnet = transform.Find("MagnetField").gameObject;
        StartBonusUsageCoroutine(bonus, magnet);
    }


    void StartBonusUsageCoroutine(GameObject bonus, GameObject visual, System.Action onExpired = null)
    {        
        StartCoroutine(StartBonusUsage(bonus, visual, onExpired));
    }


    IEnumerator StartBonusUsage(GameObject bonus, GameObject visual, System.Action onExpired)
    {   
        PlatformBonusManager.BonusUsed(bonus);
        activeBonuses.Add(bonus);
        BonusPanel.AddBonus(bonus.tag);

        var duration = (float)PlayerPrefManager.GetDuration(bonus.tag) * Time.timeScale;
        var timeLeft = duration;
        var updateRate = 0.01f * Time.timeScale;
        visual.SetActive(true);
        while (true)
        {
            if (!Game.IsPaused && !Game.IsGameOver)
            {
                if (timeLeft <= 0)
                    break;
                else
                {
                    timeLeft -= updateRate;
                    BonusPanel.SetRemain(bonus.tag, timeLeft / duration);
                }
            }
            yield return new WaitForSeconds(updateRate);
//            yield return new WaitForSeconds(duration * Time.timeScale);
        }
        if (onExpired != null)
            onExpired();

        StopBonusUsage(bonus, visual);
    }

    void StopBonusUsage(GameObject bonus, GameObject visual)
    {
        BonusPanel.RemoveBonus(bonus.tag);
        visual.SetActive(false);
        activeBonuses.Remove(bonus);
        PlatformBonusManager.BonusExpired(bonus);
    }
}
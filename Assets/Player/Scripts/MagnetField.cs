﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MagnetField : MonoBehaviour
{
    List<AttractedCoin> coins;
    Transform tr;
    bool isEnabled;

    //    // Use this for initialization
    void Start()
    {
        coins = new List<AttractedCoin>();

//        StopCoroutine("AttractCoins");

        tr = transform;
//        StartCoroutine(AttractCoins());
        InvokeRepeating("AttractCoins", 0, Time.fixedDeltaTime * 2);
    }
    //
    //    // Update is called once per frame
    //    void FixedUpdate()
    //    {
    //        AttractCoins(coins);
    //    }

    void AttractCoins()
    {
//        var coinList = coins.ToList();
//        if (coinList.Count == 0 && isEnabled == false)
//        {
//            tr.SetParent(null);
//            return;
//        }
//
        foreach (var coin in coins.ToList())
        {
            if (!coin.IsActive)
                coins.Remove(coin);
            else if (tr.parent != null)
                coin.Attract(tr.parent.position);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag(TagManager.Coin))
            return;

        var coin = other.gameObject.transform;
//        if (coin != null && !coins.Contains(coin))//&& !coins.Any(c => c.IsSameBody(coin)))
        if (coin != null)

//            coins.Add(coin);
            coins.Add(new AttractedCoin(coin));
    }


    //    public void Enable()
    //    {
    //        isEnabled = true;
    //    }
    //
    //
    //    public void Disable()
    //    {
    //        isEnabled = false;
    //    }

    //    IEnumerator AttractCoins()
    //    {
    //
    //    }
}

public struct AttractedCoin
{
    readonly Transform transform;
    readonly System.DateTime attractionStart;

    public AttractedCoin(Transform transform)
    {
        this.transform = transform;
        attractionStart = System.DateTime.Now;
    }

    //    public const float speed = 5f;

    public void Attract(Vector3 position)
    {
        float multiplier = PlayerInfo.HasJetPack() ? 0.5f : 0.1f;

//        float speed = 5 + Mathf.Exp((System.DateTime.Now - attractionStart).Milliseconds * multiplier);
        float speed = (System.DateTime.Now - attractionStart).Milliseconds * multiplier;
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, position, step);


//        Vector2 direction = (position - Body.transform.position).normalized * 10;
//        Body.Sleep();
////        var force = direction * (System.DateTime.Now - attractionStart).Milliseconds / 100;
//        Body.velocity = direction;
//        Body.AddForce(force, ForceMode2D.Impulse);
//        Body.AddForceAtPosition(force, position, ForceMode2D.Impulse);
    }

    public bool IsActive
    {
        get { return transform.gameObject.activeSelf; }
    }

    public bool IsSame(Transform transform)
    {
        return ReferenceEquals(this.transform, transform);
    }
}
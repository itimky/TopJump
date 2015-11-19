using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlayerPhisicsManager : Pausable
{
    public Vector3 jumpVelocity;

    private FallingBonusManager _fallingBonusManager;
    private PlayerInfo _playerInfo;

    static Rigidbody2D rigidbody2d;

    void Start()
    {
        Game.RegisterPausableObject(this);
        rigidbody2d = GetComponent<Rigidbody2D>();
        _fallingBonusManager = GetComponent<FallingBonusManager>();
        _playerInfo = GetComponent<PlayerInfo>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (rigidbody2d.velocity.y == 0)
            InteractManager.Interact(this.gameObject, collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Game.IsGameOver)
            return;
        
        if (other.gameObject.layer == LayerManager.FallingBonuses)
            _fallingBonusManager.AddBonus(other.gameObject);
        else if (other.gameObject.layer == LayerManager.Treasures)
        {
            if (other.gameObject.CompareTag(TagManager.Coin))
                CoinManager.AddGold(other.gameObject);
        }
        else if (other.gameObject.layer == LayerManager.PlatformBonuses)
        {
            _playerInfo.AddBonus(other.gameObject);
        }
    }


    private Vector2 prepauseVelocity;

    public override void Pause()
    {
        base.Pause();
        prepauseVelocity = rigidbody2d.velocity;
        rigidbody2d.Sleep();
    }

    public override void Resume()
    {
        base.Resume();
        rigidbody2d.WakeUp();
        rigidbody2d.velocity = prepauseVelocity;
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlayerPhisicsManager : Pausable
{
	public Vector2 jumpVelocity;
	public Vector2 superJumpVelocity;

	private FallingBonusManager _fallingBonusManager;
	private PlayerInfo _playerInfo;
	private PlayerAudio _audio;

	static Rigidbody2D rigidbody2d;

	void Start()
	{
		Game.RegisterPausableObject(this);
		_audio = GetComponent<PlayerAudio>();
		rigidbody2d = GetComponent<Rigidbody2D>();
		_fallingBonusManager = GameObject.Find("Game").GetComponent<FallingBonusManager>();
		_playerInfo = GetComponent<PlayerInfo>();
	}

	void OnCollisionStay2D(Collision2D collision)
	{
		if (rigidbody2d.velocity.y == 0)
			InteractWithPlatform(collision.gameObject);
		//InteractManager.Interact(this.gameObject, collision.gameObject);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (Game.IsGameOver)
			return;

		if (other.gameObject.layer == LayerManager.FallingBonuses)
			_fallingBonusManager.AddBonus(other.gameObject);
		else
		if (other.gameObject.layer == LayerManager.Treasures)
		{
			if (other.gameObject.CompareTag(TagManager.Coin))
				CoinManager.AddGold(other.gameObject);
		}
		else
		if (other.gameObject.layer == LayerManager.PlatformBonuses)
		{
			_playerInfo.AddBonus(other.gameObject);
		}
	}

	void InteractWithPlatform(GameObject platform)
	{
		if (Game.IsPaused)
			return;

		if (platform.CompareTag(TagManager.SimplePlatform) || platform.CompareTag(TagManager.AstralPlatform))
			Jump();

		if (platform.CompareTag(TagManager.SuperJumpPlatform))
			SuperJump();

		if (platform.CompareTag(TagManager.SpikePlatform))
		{
			if (platform.GetComponent<SpikePlatformBehavior>().IsHarmful && !PlayerInfo.IsInvulnerable)
				_playerInfo.MakeDamage();
			else
				Jump();
		}
	}

	void Jump()
	{
		rigidbody2d.velocity = jumpVelocity;
		_audio.Jump();
	}

	void SuperJump()
	{
		rigidbody2d.velocity = superJumpVelocity;
		_audio.SuperJump();
	}


	private Vector2 prepauseVelocity;

	public override void Pause()
	{
		if (Game.IsGameOver)
			return;

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
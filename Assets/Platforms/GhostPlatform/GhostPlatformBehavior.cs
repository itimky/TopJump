using UnityEngine;
using System.Collections;

public class GhostPlatformBehavior : Pausable
{
    public bool IsAstral { get; private set; }

    private BoxCollider2D boxCollider2d;
    private Animator animator;

    void Start()
    {        
        animator = GetComponent<Animator>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        Game.RegisterPausableObject(this);
    }

    void Toggle()
    {
        IsAstral = !IsAstral;
        boxCollider2d.isTrigger = IsAstral;
    }

    float prepauseSpeed;

    public override void Pause()
    {
        prepauseSpeed = animator.speed;
        animator.speed = 0;
    }

    public override void Resume()
    {
        animator.speed = prepauseSpeed;
    }
}

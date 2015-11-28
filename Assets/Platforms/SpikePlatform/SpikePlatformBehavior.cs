using UnityEngine;
using System.Collections;

public class SpikePlatformBehavior : Pausable
{
    public bool IsHarmful { get; private set; }

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        Game.RegisterPausableObject(this);
    }

    void Toggle()
    {        
        IsHarmful = !IsHarmful;
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

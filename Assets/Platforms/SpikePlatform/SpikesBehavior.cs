using UnityEngine;
using System.Collections;

public class SpikesBehavior : Pausable
{
    public bool AreSpikesUp { get; private set; }

    private Animator animator;

    void Toggle()
    {
        animator = GetComponent<Animator>();
        AreSpikesUp = !AreSpikesUp;
        Game.RegisterPausableObject(this);
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

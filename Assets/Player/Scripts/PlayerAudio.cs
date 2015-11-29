using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour
{

    public AudioClip JumpAudio;
    public AudioClip SuperJumpAudio;
    public AudioClip LeftMoveAudio;
    public AudioClip RightMoveAudio;

    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Jump()
    {
        source.PlayOneShot(JumpAudio, 0.4f);
    }

    public void SuperJump()
    {
        source.PlayOneShot(SuperJumpAudio, 0.6f);
    }

    public void Move(MoveDirection direction)
    {
        if (direction == MoveDirection.Left)
            source.PlayOneShot(LeftMoveAudio, 0.7f);
        else if (direction == MoveDirection.Right)
            source.PlayOneShot(RightMoveAudio, 07f);
    }
}

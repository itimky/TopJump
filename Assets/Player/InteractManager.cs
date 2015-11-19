using UnityEngine;
using System.Collections;

public class InteractManager
{
    private static readonly Vector2 jumpVelocity = new Vector2(0, 10);
    private static readonly Vector2 superJumpVelocity = new Vector2(0, 26.9f);

    public static void Interact(GameObject player, GameObject platform)
    {
        if (Game.IsPaused)
            return;

        if (platform.CompareTag(TagManager.SimplePlatform) || platform.CompareTag(TagManager.AstralPlatform))
            player.GetComponent<Rigidbody2D>().velocity = jumpVelocity;

        if (platform.CompareTag(TagManager.SuperJumpPlatform))
            player.GetComponent<Rigidbody2D>().velocity = superJumpVelocity;

        if (platform.CompareTag(TagManager.SpikePlatform))
        {
            if (platform.GetComponent<SpikePlatformBehavior>().IsHarmful && !PlayerInfo.IsInvulnerable)
                player.GetComponent<PlayerInfo>().MakeDamage();
            else
                player.GetComponent<Rigidbody2D>().velocity = jumpVelocity;
        }
            
    }
}

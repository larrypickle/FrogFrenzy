using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashHat : Hat
{
    public FlashHat(GameObject visual)
        : base(HatBehavior.HatType.Flash, visual )
    {
    }


    public override void Activate(PlayerMovement player)
    {
        base.Activate(player);
        
        // increase speed
        player.MoveDelayTime /= player.hatSpeedMultiplier;
        player.runSpeed += 10f;
        player.PlayFire();
    }
    public override void Attach(PlayerMovement player)
    {

    }

    public override void Detach(PlayerMovement player)
    {
        player.MoveDelayTime *= player.hatSpeedMultiplier;
        player.DestroyFire();
        player.runSpeed -= 10f;
    }
}

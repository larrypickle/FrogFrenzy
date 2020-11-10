using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WitchHat : Hat
{
    public WitchHat(GameObject visual)
        : base(HatBehavior.HatType.Witch, visual)
    {

    }


    public override void Activate(PlayerMovement player)
    {
        base.Activate(player);

        LeanTween.cancel(player.killBar);
        player.killBar.LeanScaleX(1.0f, 1.0f).setOnComplete(player.ContinuousMove);
        //LeanTween.color(playerMovement.killBar, Color.blue, 0.1f);
        player.killBar.GetComponent<Image>().color = Color.blue;
        player.killTime += 3.0f;
    }


    public override void Attach(PlayerMovement player)
    {

    }

    public override void Detach(PlayerMovement player)
    {
        player.killTime -= 3f;
    }
}

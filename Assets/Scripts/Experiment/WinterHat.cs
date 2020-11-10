using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinterHat : Hat
{
    public WinterHat(GameObject visual)
        : base(HatBehavior.HatType.Winter, visual)
    {

    }


    public override void Activate(PlayerMovement player)
    {
        base.Activate(player);

        // longer attack time
        //faster push time
        if (player.pushTime >= 0.5f)
        {
            player.bar.GetComponent<Image>().color = Color.blue;
            player.pushTime /= 2f;
            Debug.Log($"Winter Hat Push Time: {player.pushTime}");
        }
    }
    public override void Attach(PlayerMovement player)
    {

    }

    public override void Detach(PlayerMovement player)
    {
        player.bar.GetComponent<Image>().color = Color.red;
        player.pushTime *= 2f;
    }
}

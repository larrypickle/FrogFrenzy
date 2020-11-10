using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowboyHat : Hat
{

    public CowboyHat(GameObject visual)
        : base(HatBehavior.HatType.Cowboy, visual)
    {
    }


    public override void Activate(PlayerMovement player)
    {
        base.Activate(player);
        player.attack.LeanColor(Color.red, 0.1f);
        player.attack.LeanAlpha(0.8f, 0.1f);
        player.pushSize += 1f;
    }
    public override void Attach(PlayerMovement player)
    {
        base.Attach(player);
        player.croak.Play();
        player.PlayAttack();
    }

    public override void Detach(PlayerMovement player)
    {
        base.Detach(player);
        player.pushSize -= 1f;
        player.attack.LeanColor(Color.black, 0.1f);
        player.attack.LeanAlpha(0.8f, 0.1f);
    }
}

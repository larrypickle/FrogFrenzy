﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseHat : Hat
{

    public NurseHat(GameObject visual)
        : base(HatBehavior.HatType.Nurse, visual)
    {

    }
    public override void Activate(PlayerMovement player)
    {
        base.Activate(player);        

        if (player.frogSize == FrogSize.smallest) return;

        // replace with a messasging system?
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in allEnemies)
        {
            enemy.GetComponent<EnemyMovement>().moveSpeed /= 4.0f;
        }


        //shrink player (BUGGY... maybe fixed?)
        // update frog size
        if (player.frogSize == FrogSize.regular)
        {
            player.frogSize = FrogSize.small;
            player.currentObjectScale = player.OGObjectScale - 0.1f;
        }
        else if (player.frogSize == FrogSize.small)
        {
            player.frogSize = FrogSize.smallest;
            player.currentObjectScale = player.OGObjectScale - 0.2f;
        }
        player.transform.localScale = new Vector3(player.currentObjectScale, player.currentObjectScale, player.currentObjectScale);
        player.col.radius /= 1.7f;

    }
    public override void Attach(PlayerMovement player)
    {
        base.Attach(player);
    }

    public override void Detach(PlayerMovement player)
    {
        base.Detach(player);

        int numNurseHats = player.Hats.GetNumHat(_type);
        if (numNurseHats < 2)
        {
            Debug.Log($"Making {player.frogSize}");
            switch (player.frogSize)
            {
                case FrogSize.smallest:
                    player.frogSize = FrogSize.small;
                    player.currentObjectScale = player.OGObjectScale - 0.1f;
                    break;
                case FrogSize.small:
                    player.frogSize = FrogSize.regular;
                    player.currentObjectScale = player.OGObjectScale;
                    break;
            }
        }
        player.gameObject.transform.localScale = new Vector3(player.currentObjectScale, player.currentObjectScale, player.currentObjectScale);
        // reset enemy move speed
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in allEnemies)
        {
            enemy.GetComponent<EnemyMovement>().moveSpeed = player.ogEnemyMoveSpeed;
        }

    }
}

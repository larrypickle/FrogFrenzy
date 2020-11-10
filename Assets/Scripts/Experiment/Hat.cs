using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hat
{
    protected HatBehavior.HatType _type;
    protected GameObject _visual;

    public Hat(HatBehavior.HatType inType, GameObject visual)
    {
        this._type = inType;
        this._visual = visual;
    }

    public HatBehavior.HatType Type => _type;

    public virtual void Activate(PlayerMovement player)
    {
        if (!ValidatePlayer(player)) return;
        //Debug.Log($"Activating Hat: {_type}");
    }
    public virtual void Attach(PlayerMovement player)
    {
        if (!ValidatePlayer(player)) return;
      //  Debug.Log($"Attaching Hat: {_type}");
        player.lives++;
    }
    public virtual void Detach(PlayerMovement player)
    {
        if (!ValidatePlayer(player)) return;
       // Debug.Log($"Detaching Hat: {_type}");
    }

    protected bool ValidatePlayer(PlayerMovement playerMovement)
    {
        if (playerMovement == null)
        {
            Debug.Log($"GameObject does not have a valid player controller");
            return false;
        }
        return true;
    }
}

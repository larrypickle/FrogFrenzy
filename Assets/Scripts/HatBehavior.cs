using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatBehavior : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public enum HatType { //tracks what level it is
        Nurse,
        Cowboy,
        Flash,
        Witch,
        Winter
    }
    // Start is called before the first frame update


    [SerializeField] public HatType hatType;
    private Hat hat;


    public HatType Type => hatType;
    public Hat GetHat => hat;


    public Hat CreateHat(GameObject visual)
    {
        switch (hatType)
        {
            case HatType.Nurse:
                hat = new NurseHat(visual);
                break;
            case HatType.Cowboy:
                hat = new CowboyHat(visual);
                break;
            case HatType.Flash:
                hat = new FlashHat(visual);
                break;
            case HatType.Witch:
                hat = new WitchHat(visual);
                break;
            case HatType.Winter:
                hat = new WinterHat(visual);
                break;
        }
        return hat;
    }
}

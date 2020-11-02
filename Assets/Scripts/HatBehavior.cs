using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    void Start()
    {
    }
    public void activateHat()
    {
        if(hatType == HatType.Nurse) {
            // increase health
        } else if (hatType == HatType.Cowboy) {
            // halves existing enemy speed
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log(allEnemies.Length);
            foreach(GameObject enemy in allEnemies) {
                enemy.GetComponent<EnemyMovement>().moveSpeed /= 2.0f;
            }
        } else if (hatType == HatType.Flash) {
            // increase speed TODO doesn't work rn
            playerMovement.MoveDelayTime /= 4.0f;
            
        } else if (hatType == HatType.Witch) {
            // speed up kill wait
            LeanTween.cancelAll();
            playerMovement.killBar.LeanScaleX(1.0f, 1.0f).setOnComplete(playerMovement.ContinuousMove);            
        } else if (hatType == HatType.Winter) {
            // longer attack time
            playerMovement.killTime += 3.0f;
        }
    }
    public HatType hatType;

    // Update is called once per frame
    void Update()
    {
        
        
    }


    
}

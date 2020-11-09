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

            //shrink player
            // playerMovement.Shrink();
        } else if (hatType == HatType.Cowboy) {
            // halves existing enemy speed
            playerMovement.pushSize += 0.5f;
            /*
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log(allEnemies.Length);
            foreach(GameObject enemy in allEnemies) {
                enemy.GetComponent<EnemyMovement>().moveSpeed /= 4.0f;
            }*/
        } else if (hatType == HatType.Flash) {
            // increase speed TODO doesn't work rn
            playerMovement.MoveDelayTime /= PlayerMovement.hatSpeedMultiplier;
            //halves existing enemy speed
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log(allEnemies.Length);
            foreach (GameObject enemy in allEnemies)
            {
                enemy.GetComponent<EnemyMovement>().moveSpeed /= 4.0f;
            }

        } else if (hatType == HatType.Witch) {
            // speed up kill wait
            LeanTween.cancelAll();
            playerMovement.killBar.LeanScaleX(1.0f, 1.0f).setOnComplete(playerMovement.ContinuousMove);            
        } else if (hatType == HatType.Winter) {
            // longer attack time
            playerMovement.killTime += 3.0f;
            //faster push time
            if(playerMovement.pushTime >= 0.5f)
            {
                playerMovement.pushTime /= 2f;
                Debug.Log("pushtime " + playerMovement.pushTime);
            }
        }
    }
    public HatType hatType;

    // Update is called once per frame
    void Update()
    {
        
        
    }


    
}

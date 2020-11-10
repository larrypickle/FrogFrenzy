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
    void Start()
    {
    }
    public void activateHat()
    {
        if(hatType == HatType.Nurse) {
            if(playerMovement.frogSize == PlayerMovement.FrogSize.smallest) { // already 2x small
                return;
            }
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject enemy in allEnemies) {
                enemy.GetComponent<EnemyMovement>().moveSpeed /= 4.0f;
            }
            //shrink player (BUGGY... maybe fixed?)
            // update frog size
            if(playerMovement.frogSize == PlayerMovement.FrogSize.regular) {
                playerMovement.frogSize = PlayerMovement.FrogSize.small;
                playerMovement.currentObjectScale = playerMovement.OGObjectScale - 0.1f;
            } else if (playerMovement.frogSize == PlayerMovement.FrogSize.small) {
                playerMovement.frogSize = PlayerMovement.FrogSize.smallest;
                playerMovement.currentObjectScale = playerMovement.OGObjectScale - 0.2f;
            } 
            playerMovement.transform.localScale = new Vector3(playerMovement.currentObjectScale, playerMovement.currentObjectScale, playerMovement.currentObjectScale);
            playerMovement.col.radius /= 1.7f;
            //playerMovement.Shrink();
        } else if (hatType == HatType.Cowboy) {
            // increases push size
            playerMovement.attack.LeanColor(Color.red, 0.1f);
            playerMovement.attack.LeanAlpha(0.8f, 0.1f);
            playerMovement.pushSize += 1f;
            
        } else if (hatType == HatType.Flash) {
            // increase speed
            playerMovement.MoveDelayTime /= playerMovement.hatSpeedMultiplier;
            playerMovement.runSpeed += 10f;
            //instantiate fire effect
            
            playerMovement.fx = Instantiate(playerMovement.fire, playerMovement.transform.position, playerMovement.transform.rotation);
            playerMovement.fx.transform.parent = playerMovement.transform;
            

        } else if (hatType == HatType.Witch) {
            // speed up kill wait
            LeanTween.cancel(playerMovement.killBar);
            playerMovement.killBar.LeanScaleX(1.0f, 1.0f).setOnComplete(playerMovement.ContinuousMove);
            //LeanTween.color(playerMovement.killBar, Color.blue, 0.1f);
            playerMovement.killBar.GetComponent<Image>().color = Color.blue;
            playerMovement.killTime += 3.0f;

        }
        else if (hatType == HatType.Winter) {
            // longer attack time
            //faster push time
            if(playerMovement.pushTime >= 0.5f)
            {
                playerMovement.bar.GetComponent<Image>().color = Color.blue;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool canMove = true;
    private Vector3 forward;
    float timer;
    public GameObject vfx;
    public GameObject vfx2;
    //score update
    public GameManager gameManager;
    public PlayerMovement player;
    public float lengthIncrease = 0.25f;
    public AudioSource hit;
    // Start is called before the first frame update
    void Start()
    {
        timer = 3f;
        float randAngle = Random.Range(0, Mathf.Deg2Rad * 360);
        forward = new Vector3(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        transform.position += moveSpeed * forward * Time.deltaTime;
        //accelerate enemies and make them go a different direction
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collision works");
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("wall detected");
            forward *= -1;
        }

        //when it gets hit by the push
        if (collision.gameObject.CompareTag("Attack"))
        {
            
            // delete self
            Debug.Log("hit");
            GameObject spawnedfx = Instantiate(vfx2, transform.position, Quaternion.identity) as GameObject;
            forward *= -1;
            hit.Play();
            //gameManager.UpdateScore();
            //player.attackLength += lengthIncrease;
            //gameObject.SetActive(false);

        }
    }
    public void Die(Vector3 pos)
    {
        GameObject spawnedfx = Instantiate(vfx, pos, Quaternion.identity) as GameObject;
        //forward *= -1;
        gameManager.UpdateScore();
    }

    /*
        
    */


}

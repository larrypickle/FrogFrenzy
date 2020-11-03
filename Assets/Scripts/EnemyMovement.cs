using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool canMove = true;
    protected Vector3 forward;
    float timer;
    public GameObject vfx;
    public GameObject vfx2;
    //score update
    public GameManager gameManager;
    public PlayerMovement player;
    public float lengthIncrease = 0.25f;
    public AudioSource hit;
    // Start is called before the first frame update
    protected void Start()
    {
        float randAngle = Random.Range(0, Mathf.Deg2Rad * 360);
        forward = new Vector3(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
    }

    // Update is called once per frame
    protected void Update()
    {
        transform.position += moveSpeed * forward * Time.deltaTime;
        //accelerate enemies and make them go a different direction
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("collision works");
        if (collision.gameObject.CompareTag("Wall"))
        {
            forward *= -1;
        }

        //when it gets hit by the push
        if (collision.gameObject.CompareTag("Attack"))
        {
            
            // delete self
           // Debug.Log("hit");
            GameObject spawnedfx = Instantiate(vfx2, transform.position, Quaternion.identity) as GameObject;
            // forward *= -1;
            Vector3 pos = gameObject.transform.position;
            if ((pos.x < player.transform.position.x) && (forward.x > 0))
                forward.x *= -1;

            if ((pos.x > player.transform.position.x) && (forward.x < 0))
                forward.x *= -1;

            if ((pos.y < player.transform.position.y) && (forward.y > 0))
                forward.y *= -1;

            if ((pos.y > player.transform.position.y) && (forward.y < 0))
                forward.y *= -1;
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //Debug.Log("wall detected");
            forward *= -1;
        }
    }


}

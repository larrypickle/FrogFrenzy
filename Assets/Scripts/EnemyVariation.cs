using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVariation : EnemyMovement
{
    public Rigidbody2D rb;

    void Start()
    {
        ChangeDirection();
        //rb = this.GetComponent<Rigidbody2D>();
        InvokeRepeating("ChangeDirection", 3f, 3f);

    }
   
    private void ChangeDirection()
    {
        //Debug.Log("Works");
        rb.AddForce(new Vector2(Random.Range(-3f, 3f), 0));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpin : EnemyMovement
{
    new void Update()
    {

        transform.position += moveSpeed * forward * Time.deltaTime;

        transform.Rotate(Vector3.forward);

    }
}


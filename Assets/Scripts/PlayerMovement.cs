using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DentedPixel;

public class PlayerMovement : MonoBehaviour
{
    //Based off of https://www.youtube.com/watch?v=G4aAUodsU3o
    [SerializeField]
    public float moveSpeed = 0.25f;

    Vector3 targetPosition;
    Vector3 startPosition;
    bool moving;
    public bool CanMove = true;
    public float MoveDelayTime = 0.5f;
    //attack
    public GameObject attack;
    GameObject attackObj;
    public float attackLength = 0.25f;

    public float objectScale = 0.3f;
    public GameObject vfx;

    //for collision

    //timer
    public GameObject bar;
    public float time = 2f;
    public bool canFire = false;

    //killbar
    public GameObject killBar;
    public float killTime = 5f;
    //public bool canKill = false;

    //continous move
    public bool discreteMove = true;
    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    public float runSpeed = 20.0f;
    Rigidbody2D body;
    CircleCollider2D col;

    //slowing enemies down in continuous move
    public EnemyMovement enemy;
    public AudioSource audio;
    public AudioSource powerup;


    // Start is called before the first frame update
    void Start()
    {
        CanMove = true;
        AnimateBar();
        AnimateKillBar();
        body = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            if (Vector3.Distance(startPosition, transform.position) > 1f) //snap to the targetPosition at a certain frame
            {
                transform.position = targetPosition;
                moving = false;

                return;
            }

            transform.position += (targetPosition - startPosition) * moveSpeed * Time.deltaTime;
            return;

        }


        if (discreteMove)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (IsTileEmpty(Vector3.up, 1f) && CanMove == true)
                {
                    targetPosition = transform.position + (Vector3.up);
                    startPosition = transform.position;
                    moving = true;
                    //StartCoroutine(MoveDelay());

                }

            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (IsTileEmpty(Vector3.down, 1f) && CanMove == true)
                {

                    targetPosition = transform.position + (Vector3.down);
                    startPosition = transform.position;
                    moving = true;
                    //StartCoroutine(MoveDelay());
                }


            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (IsTileEmpty(Vector3.left, 1f) && CanMove == true)
                {
                    if (transform.localScale.x < 0)
                    {
                        Vector3 temp = transform.localScale;
                        temp.x *= -1;
                        transform.localScale = temp;
                    }
                    targetPosition = transform.position + (Vector3.left);
                    startPosition = transform.position;
                    moving = true;
                    //StartCoroutine(MoveDelay());
                    //gameObject.transform.localScale = new Vector3(1, 1);



                }


            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (IsTileEmpty(Vector3.right, 1f) && CanMove == true)
                {
                    if (transform.localScale.x > 0)
                    {
                        Vector3 temp = transform.localScale;
                        temp.x *= -1;
                        transform.localScale = temp;
                    }
                    targetPosition = transform.position + (Vector3.right);
                    startPosition = transform.position;
                    moving = true;
                    //StartCoroutine(MoveDelay());
                    //gameObject.transform.localScale = new Vector3(-1, 1);

                }



            }

            if (Input.GetKeyDown(KeyCode.Space) && canFire == true)
            {
                // attack
                attackObj = Instantiate(attack, gameObject.transform.position, Quaternion.identity);
                //timer = 3.0f;
                Destroy(attackObj, attackLength);
                canFire = false;
                bar.transform.localScale = new Vector3(0, 1, 1); //resets the timer bar
                AnimateBar();
            }
        }

        else
        {
            //continous move
            horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
            vertical = Input.GetAxisRaw("Vertical"); // -1 is down
        }

        /*if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            cooldown.text = "Cooldown Timer: " + timer.ToString("#.00");
        }*/
    }

    private void FixedUpdate()
    {
        //continuous move
        if (!discreteMove)
        {
            if (horizontal != 0 && vertical != 0) // Check for diagonal movement
            {
                // limit movement speed diagonally, so you move at 70% speed
                horizontal *= moveLimiter;
                vertical *= moveLimiter;
            }

            body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
        }
    }
    private bool IsTileEmpty(Vector3 direction, float distance)
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter = new ContactFilter2D();

        // visualise the direction we are testing for
        //Debug.DrawRay(transform.position, r.direction, Color.blue, rayLength);
        int numHits = col.Cast(direction, filter, hits, distance);
        for(int i = 0; i < numHits; i++)
        {
            if (!hits[i].collider.isTrigger)
            {
                Debug.Log("hit wall");
                return false;
            }
        }
        /*if (hit.collider != null)
        {
            if (hit.collider.tag == "Wall" || hit.collider.tag == "Enemy")
            {
                Debug.Log("Hit wall");
                return false;
            }
        }*/

        return true; //return true if it hits nothing
    }

    IEnumerator MoveDelay()
    {
        CanMove = false;
        yield return new WaitForSeconds(MoveDelayTime);
        CanMove = true;


    }

    void ContinuousMove()
    {
        powerup.Play();
        //activates continuous movement
        discreteMove = false;
        //increase size of collider
        col.radius += 1;
        //increase size of player
        gameObject.transform.localScale *= 2;
        enemy.moveSpeed = 0f;
        Debug.Log("enemy movespeed " + enemy.moveSpeed);
        LeanTween.scaleX(killBar, 0, 3f).setOnComplete(AnimateKillBar);

    }
    
    //collide with enemy
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (discreteMove)
            {
                SceneManager.LoadScene("SampleScene");
            }
            else
            {
                collision.gameObject.SetActive(false);
                enemy.Die(collision.gameObject.transform.position);
                audio.pitch = Random.Range(0.7f, 1.1f);
                audio.Play();
            }
        }

    }

    public void AnimateBar()
    {
        if (!canFire)
        {
            LeanTween.scaleX(bar, 1, time).setOnComplete(FireEnable);
            //watched this tutorial for the bar timer: https://www.youtube.com/watch?v=z7bR_xYcopM

        }

    }

    public void AnimateKillBar()
    {
        
        //reset values
        if(gameObject.transform.localScale.x != objectScale)
        {
            col.radius = 1;
            gameObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);
            gameObject.transform.localRotation = Quaternion.identity;
            body.velocity = Vector3.zero;
            body.angularVelocity = 0f;
            enemy.moveSpeed = 5f;
        }
        discreteMove = true;
        LeanTween.scaleX(killBar, 1, killTime).setOnComplete(ContinuousMove);
        //watched this tutorial for the bar timer: https://www.youtube.com/watch?v=z7bR_xYcopM

        

    }

    void FireEnable()
    {
        canFire = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //does this work?
        Debug.Log("Collision with Wall WORKS");
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("PLEASE WORK");
            body.velocity = Vector3.zero; 
        }
    }

}
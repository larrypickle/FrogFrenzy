using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DentedPixel;

enum MoveState
{
    Idle = 0,
    Discrete,
    Unlocked
}

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
    public GameObject vfx;

    //for collision

    //timer
    [Header("Timer")]
    public GameObject bar;
    public float time = 2f;
    public bool canFire = false;

    //killbar
    [Header("Killbar")]
    public GameObject killBar;
    public float killTime = 5f;
    //public bool canKill = false;

    //continous move
    [Header("Continuous Move")]
    public bool discreteMove = true;
    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    public float runSpeed = 20.0f;
    Rigidbody2D body;
    CircleCollider2D col;

    [Header("Components")]
    //slowing enemies down in continuous move
    public EnemyMovement enemy;
    public AudioSource audio;
    public AudioSource powerup;


    private MoveState _moveState = MoveState.Idle;
    private Vector2 _moveInput;
    private Grid _board;

    [SerializeField] private Vector2Int currPos = new Vector2Int(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        CanMove = true;
        AnimateBar();
        body = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();


        _board = GameManager.instance.GetGrid;
        _board.SetPos(currPos, (int) ObjectTypes.Player);
        transform.position = _board.GetPosition(currPos, Vector2.zero);

        AnimateKillBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (_moveState != MoveState.Idle)
        {

        }
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
            Vector2Int moveDir = Vector2Int.zero;
            if      (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))      moveDir = Vector2Int.down;
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))    moveDir = Vector2Int.up;
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))    moveDir = Vector2Int.left;
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))   moveDir = Vector2Int.right;

            if (CanMove && moveDir != Vector2Int.zero && IsTileEmpty(moveDir))
            {
                _board.MoveItem(currPos, currPos + moveDir);
                currPos = currPos + moveDir;
                startPosition = transform.position;
                targetPosition = _board.GetPosition(currPos, Vector2.zero);
                moving = true;
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

            //        if (transform.localScale.x < 0)
            //        {
            //            Vector3 temp = transform.localScale;
            //            temp.x *= -1;
            //            transform.localScale = temp;
            //        }
        }
        else
        {
            _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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
            if (_moveInput.x != 0 && _moveInput.y != 0)
            {
                _moveInput *= moveLimiter; // 70% speed
            }
            body.velocity = _moveInput * runSpeed;
        }
    }
    private bool IsTileEmpty(Vector2Int direction)
    {
        // makes sure the new position is within bounds
        if (!_board.ValidPos(currPos + direction)) return false;

        // makes sure that the new space is empty
        ObjectTypes type = (ObjectTypes) _board.AtPos(currPos + direction);
        if (type == ObjectTypes.None) return true;
        return false;


        //RaycastHit2D[] hits = new RaycastHit2D[10];
        //ContactFilter2D filter = new ContactFilter2D();

        //// visualise the direction we are testing for
        ////Debug.DrawRay(transform.position, r.direction, Color.blue, rayLength);
        //int numHits = col.Cast(direction, filter, hits, distance);
        //for(int i = 0; i < numHits; i++)
        //{
        //    if (!hits[i].collider.isTrigger)
        //    {
        //        Debug.Log("hit wall");
        //        return false;
        //    }
        //}
        //return true; //return true if it hits nothing
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
        //.Log("enemy movespeed " + enemy.moveSpeed);
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
        if(gameObject.transform.localScale.x != 0.4f)
        {
            col.radius = 1;
            gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            gameObject.transform.localRotation = Quaternion.identity;
            body.velocity = Vector3.zero;
            body.angularVelocity = 0f;
            enemy.moveSpeed = 5f;
        }
        discreteMove = true;
       // Debug.Log("HERE");

        // snapping back to the grid
        Vector2 test = _board.GetNearestPos(transform.position);
        _board.MoveItem(currPos, new Vector2Int((int) test.x, (int) test.y));
        currPos = new Vector2Int((int) test.x, (int) test.y);
        transform.position = _board.GetPosition(currPos, Vector2.zero);
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
       // Debug.Log("Collision with Wall WORKS");
        if (collision.gameObject.CompareTag("Wall"))
        {
          //  Debug.Log("PLEASE WORK");
            body.velocity = Vector3.zero; 
        }
    }

}
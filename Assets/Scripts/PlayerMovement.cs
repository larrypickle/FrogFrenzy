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
    private bool CanMove = true;
    public float MoveDelayTime = 0.5f;
    //attack
    public GameObject attack;
    GameObject attackObj;
    public float attackLength = 0.25f;
    public float OGObjectScale = 0.3f;
    public float currentObjectScale;
    public GameObject vfx;

    //for collision

    //timer
    [Header("Timer")]
    public GameObject bar;
    public float pushTime = 2f;
    private bool canFire = false;

    //killbar
    [Header("Killbar")]
    public GameObject killBar;
    public float killTime = 5f;
    //public bool canKill = false;

    //continous move
    [Header("Continuous Move")]
    public bool discreteMove = true;
    // private bool discreteMove = true;
    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    public float runSpeed = 20.0f;
    Rigidbody2D body;
    public CircleCollider2D col;

    [Header("Components")]
    //slowing enemies down in continuous move
    public EnemyMovement enemy;
    public AudioSource audio;
    public AudioSource hatPickup;
    public AudioSource powerup;
    public AudioSource hatDestroy;
    public AudioSource croak;
    public AudioSource ouch;
    public Text gameOver;

    // hats
    public GameObject [] hats; // only for rendering looks!
    public float hatSpeedMultiplier = 2.0f;
    private float lostALifeTimer;
    private float invincibilityTime = 2.0f;
    private Stack<GameObject> activeHatStack = new Stack<GameObject>();
    public int lives = 0;
    public enum FrogSize { //tracks what level it is
        regular,
        small,
        smallest
    }
    public FrogSize frogSize = FrogSize.regular;
    private float ogEnemyMoveSpeed;

    private MoveState _moveState = MoveState.Idle;
    private Vector2 _moveInput;
    private Grid _board;

    [Header("PowerUp Specifics")]
    //powerups
    public float pushSize = 1;
    public GameObject fire;
    public GameObject fx;

    [SerializeField] private Vector2Int currPos = new Vector2Int(0, 0);
    bool dead = false;

    [Header("Game Over Screen")]
    public GameObject replay;
    public GameObject quit;

    // Start is called before the first frame update
    void Start()
    {
        currentObjectScale = OGObjectScale;
        ogEnemyMoveSpeed = enemy.GetComponent<EnemyMovement>().moveSpeed;
        CanMove = true;
        AnimateBar();
        body = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();

        _board = GameManager.instance.GetGrid;
        _board.SetPos(currPos, (int) ObjectTypes.Player);
        transform.position = _board.GetPosition(currPos, Vector2.zero);

        AnimateKillBar();

        if (GameManager.instance != null)
        {
            GameManager.instance.SetPlayer(this);
        }
        lostALifeTimer = 0;
        gameOver.text = "";

        replay.SetActive(false);
        quit.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (lostALifeTimer > 0) {
            lostALifeTimer -= Time.deltaTime;
        }

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
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveDir = Vector2Int.down;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveDir = Vector2Int.up;
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                moveDir = Vector2Int.left;
                /*if (transform.localScale.x < 0)
                {
                    Vector3 temp = transform.localScale;
                    temp.x *= -1;
                    transform.localScale = temp;
                }*/
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                moveDir = Vector2Int.right;
            }
            
            if (CanMove && moveDir != Vector2Int.zero && IsTileEmpty(moveDir))
            {
                _board.MoveItem(currPos, currPos + moveDir);
                currPos = currPos + moveDir;
                startPosition = transform.position;
                targetPosition = _board.GetPosition(currPos, Vector2.zero);
                moving = true;

                //shmovement
                Vector3 temp = transform.localScale;
                temp.x *= -1;
                transform.localScale = temp;
                //start coroutine movedelay!
                StartCoroutine("MoveDelay");
            }

            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)) && canFire == true)
            {
                // attack
                croak.Play();
                attackObj = Instantiate(attack, gameObject.transform.position, Quaternion.identity);
                attackObj.transform.localScale *= pushSize;
                //timer = 3.0f;
                Destroy(attackObj, attackLength);
                canFire = false;
                bar.transform.localScale = new Vector3(0, 1, 1); //resets the timer bar
                AnimateBar();
            } 
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
            transform.Rotate(Vector3.forward * runSpeed);
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

    public void ContinuousMove()
    {
        if (!dead)
        {
            powerup.Play();
        }
        //activates continuous movement
        discreteMove = false;
        col.isTrigger = false;
        //increase size of collider
        col.radius += 1;
        //increase size of player
        killBar.GetComponent<Image>().color = Color.yellow;
        gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        LeanTween.scaleX(killBar, 0, 3f).setOnComplete(AnimateKillBar);
    }

    private IEnumerator playerHitFlashRed() {
        SpriteRenderer toFlash = gameObject.GetComponent<SpriteRenderer>();
        float flashingFor = 0;
        float flashSpeed = 0.1f;
        float flashTime = invincibilityTime;
        var flashColor = Color.red;
        var newColor = flashColor;
        var originalColor = Color.white;
        while(flashingFor < flashTime)
        {
            toFlash.color = newColor;
            flashingFor += Time.deltaTime;
            yield return new WaitForSeconds(flashSpeed);
            flashingFor += flashSpeed;
            if(newColor == flashColor)
            {
                newColor = originalColor;
            }
            else
            {
                newColor = flashColor;
            }
        }
        toFlash.color = originalColor;
    }

    private IEnumerator hatGrowShrink(GameObject hat) {
        float growTime = 0.16f;
        float shrinkTime = 0.16f;
        float flashSpeed = 0.02f;

        while(growTime > 0)
        {
            hat.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);            
            yield return new WaitForSeconds(flashSpeed);
            growTime -= flashSpeed;
        }
        while(shrinkTime > 0)
        {
            hat.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);            
            yield return new WaitForSeconds(flashSpeed);
            shrinkTime -= flashSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.SetActive(false);
            enemy.Die(collision.gameObject.transform.position);
            audio.pitch = Random.Range(0.7f, 1.1f);
            audio.Play();
        }
    }
    //collide with enemy
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.GetComponent<EnemyMovement>().Phase != EnemyPhase.Active)
            {
                return;
            }
            if (discreteMove)
            {
                if (lostALifeTimer <= 0) { // no more invincibility
                    if (lives <= 0) {
                        // SceneManager.LoadScene("SampleScene");
                        Object.Destroy(this.gameObject);
                        ouch.Play();
                        gameOver.text = "Game Over";
                        replay.SetActive(true);
                        quit.SetActive(true);
                        dead = true;
                    } else {
                        StartCoroutine (playerHitFlashRed());
                        ouch.Play();
                        lives--;
                        removeAHat();
                        lostALifeTimer = invincibilityTime;
                    }
                }
                this.gameObject.transform.localRotation = Quaternion.identity;
            }
            else
            {
                collision.gameObject.SetActive(false);
                enemy.Die(collision.gameObject.transform.position);
                audio.pitch = Random.Range(0.7f, 1.1f);
                audio.Play();
            }
        }

        else if (collision.gameObject.CompareTag("Hat"))
        {
            if (!discreteMove)
            {
                Destroy(collision.gameObject);
                hatDestroy.Play();
            }
            else if(discreteMove)
            {
                powerup.pitch = Random.Range(0.7f, 1.3f);
                hatPickup.Play();
                powerup.pitch = 1;
                // show hat
                for (int i = 0; i < hats.Length; i++)
                {
                    if (hats[i].GetComponent<HatBehavior>().hatType == collision.gameObject.GetComponent<HatBehavior>().hatType && !activeHatStack.Contains(hats[i]))
                    {
                        if(activeHatStack.Count >= 3) {
                            removeAHat();
                            lives--;
                        }
                        collision.gameObject.GetComponent<HatBehavior>().activateHat();
                        addAHat(hats[i]);
                    }
                }
                // collision.gameObject.GetComponent<HatBehavior>().activateHat();
                Destroy(collision.gameObject);
            }   
        }
    }

    void addAHat(GameObject hat) {
        Vector3 pos = hat.transform.position;
        GameObject newHat = Instantiate(hat, pos, Quaternion.identity);
        StartCoroutine(hatGrowShrink(newHat));
        
        newHat.transform.SetParent(transform);
        newHat.GetComponent<SpriteRenderer>().sortingOrder = 40 + activeHatStack.Count;
        newHat.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        // adjust hat height on frog
        pos.y += 0.1f*activeHatStack.Count;
        newHat.transform.position = pos;

        activeHatStack.Push(newHat);
        newHat.SetActive(true);
        if (hat.GetComponent<HatBehavior>().hatType == HatBehavior.HatType.Cowboy) {
            // demo the push
            // attack
                croak.Play();
                attackObj = Instantiate(attack, gameObject.transform.position, Quaternion.identity);
                attackObj.transform.localScale *= pushSize;
                //timer = 3.0f;
                Destroy(attackObj, attackLength);
        }

        lives ++;
    }

    void removeAHat() {
        if (activeHatStack.Count > 0) {
            GameObject temp = activeHatStack.Pop();
            // Debug.Log(Time.time+" REMOVING HAT!!" + temp.GetComponent<HatBehavior>().hatType);
            if (temp.GetComponent<HatBehavior>().hatType == HatBehavior.HatType.Flash) {
                MoveDelayTime *= hatSpeedMultiplier;
                if(fx != null)
                {
                    //remove fire doesnt work for some reason if u have 2 hats
                    Destroy(fx);
                }
                //Destroy(fx);
                runSpeed -= 10f;
            }
            if(temp.GetComponent<HatBehavior>().hatType == HatBehavior.HatType.Winter)
            {
                bar.GetComponent<Image>().color = Color.red;
                pushTime *= 2f;
            }
            if (temp.GetComponent<HatBehavior>().hatType == HatBehavior.HatType.Witch)
            {
                //blue color is reset in continuous move
                killTime -= 3.0f;
            }
            if (temp.GetComponent<HatBehavior>().hatType == HatBehavior.HatType.Cowboy)
            {
                pushSize -= 1f;
                attack.LeanColor(Color.black, 0.1f);
                attack.LeanAlpha(0.8f, 0.1f);
                
            }
            if (temp.GetComponent<HatBehavior>().hatType == HatBehavior.HatType.Nurse)
            {
                IEnumerator enumerator = activeHatStack.GetEnumerator(); 
                int numNurseHats = 0;
                while (enumerator.MoveNext()) { 
                    GameObject current = (GameObject)enumerator.Current;
                    if (current.GetComponent<HatBehavior>().hatType == HatBehavior.HatType.Nurse) {
                        numNurseHats++;
                    }
                } 
                if (numNurseHats < 2) {
                    if(frogSize == FrogSize.smallest) {
                        Debug.Log("Making small!");
                        frogSize = FrogSize.small;
                        currentObjectScale = OGObjectScale - 0.1f;
                    } else if (frogSize == FrogSize.small) {
                        Debug.Log("Making smallest!");
                        frogSize = FrogSize.regular;
                        currentObjectScale = OGObjectScale;
                    }
                    Debug.Log("CurrentObjectScale: " + currentObjectScale);
                    transform.localScale = new Vector3(currentObjectScale, currentObjectScale, currentObjectScale);
                    col.radius *= 1.7f;

                    // reset enemy move speed
                    GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach(GameObject enemy in allEnemies) {
                        enemy.GetComponent<EnemyMovement>().moveSpeed = ogEnemyMoveSpeed;
                    }
                }
            }
            Destroy(temp);
        }
    }

    public void AnimateBar()
    {
        if (!canFire)
        {
            LeanTween.scaleX(bar, 1, pushTime).setOnComplete(FireEnable);
            //watched this tutorial for the bar timer: https://www.youtube.com/watch?v=z7bR_xYcopM
        }
    }

    public void AnimateKillBar()
    {
        //reset values
        if(gameObject.transform.localScale.x != currentObjectScale)
        {
            col.radius = 1;
            col.isTrigger = true;
            gameObject.transform.localScale = new Vector3(currentObjectScale, currentObjectScale, currentObjectScale);
            gameObject.transform.localRotation = Quaternion.identity;
            body.velocity = Vector3.zero;
            body.angularVelocity = 0f;
            enemy.moveSpeed = 5f;
        }
        discreteMove = true;
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

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        //does this work?
       // Debug.Log("Collision with Wall WORKS");
        if (collision.gameObject.CompareTag("Wall"))
        {
          //  Debug.Log("PLEASE WORK");
            body.velocity = Vector3.zero; 
        }
    }*/

    // public void Shrink() // not used currently
    // {
    //     if (transform.localScale.x >= 0.1f)
    //     {
    //         currentObjectScale = OGObjectScale - 0.1f;
    //         transform.localScale = new Vector3(currentObjectScale, currentObjectScale, currentObjectScale);
    //         col.radius /= 1.7f;
    //     }
    // }
}
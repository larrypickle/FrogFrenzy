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

public enum FrogSize
{ //tracks what level it is
    regular,
    small,
    smallest
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
    public GameObject attackObj;
    public float attackLength = 0.25f;
    public float OGObjectScale = 0.3f;
    public float currentObjectScale;

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
    public int lives = 0;

    public FrogSize frogSize = FrogSize.regular;
    public float ogEnemyMoveSpeed;

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


    HatStack _hatStack;
    public HatStack Hats => _hatStack;

    private void Awake()
    {
        _hatStack = new HatStack();
    }

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
            collision.gameObject.GetComponent<EnemyMovement>().Die(collision.gameObject.transform.position);
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
                        RemoveHat();
                        lostALifeTimer = invincibilityTime;
                    }
                }
                this.gameObject.transform.localRotation = Quaternion.identity;
            }
            else
            {
                collision.gameObject.GetComponent<EnemyMovement>().Die(collision.gameObject.transform.position);
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
                return;
            }

            powerup.pitch = Random.Range(0.7f, 1.3f);
            hatPickup.Play();
            powerup.pitch = 1;

            HatBehavior hatB = collision.GetComponent<HatBehavior>();
            //if (_hatStack.GetNumHat(hatB.Type) ==  0) {
            if (_hatStack.NumHats >= 3)
            {
                RemoveHat();
                lives--;
            }

            AddHat(hatB);
            //}
            Destroy(collision.gameObject);
        }
    }

    void AddHat(HatBehavior hat)
    {
        GameObject hatPrefab = hats[(int)hat.Type];

        Vector3 pos = hatPrefab.transform.position;
        GameObject newHat = Instantiate(hatPrefab, pos, Quaternion.identity);
        StartCoroutine(hatGrowShrink(newHat));

        newHat.transform.SetParent(transform);
        newHat.GetComponent<SpriteRenderer>().sortingOrder = 40 + _hatStack.NumHats;
        newHat.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        // adjust hat height on frog

        float newY = 0.1f * _hatStack.NumHats;
        pos.y += 0.1f * _hatStack.NumHats;
        newHat.transform.position = pos;
        newHat.SetActive(true);

        Hat hatObj = hat.CreateHat(newHat);

        _hatStack.AddHat(hatObj);

        //Debug.Log("Num Hats: " + _hatStack.NumHats);

        hatObj.Activate(this);
        hatObj.Attach(this);
    }

    void RemoveHat()
    {
        if (_hatStack.NumHats == 0) return;
        Hat topHat = _hatStack.Pop();
        topHat.Detach(this);
        
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

    public void PickupSound()
    {

    }

    Stack<GameObject> fireFX = new Stack<GameObject>();

    public void DestroyFire()
    {
        if (fireFX.Count != 0) {
            GameObject fx = fireFX.Pop();
            Destroy(fx);
        }
    }

    public void PlayFire()
    {

        GameObject fx = Instantiate(fire, transform.position, transform.rotation);
        fx.transform.parent = transform;
        fireFX.Push(fx);
    }

    public void PlayAttack()
    {
        attackObj = Instantiate(attack, gameObject.transform.position, Quaternion.identity);
        attackObj.transform.localScale *= pushSize;
        //timer = 3.0f;
        Destroy(attackObj, attackLength);
    }
}
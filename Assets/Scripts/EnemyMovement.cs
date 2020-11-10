using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum EnemyPhase
{
    Unknown = 0,
    Spawning,
    Active,
}

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyData data;


    [Header("Enemy Info")]
    public float moveSpeed = 5f;
    public bool canMove = true;
    protected Vector3 forward;
    float timer;

    [Header("VFX")]
    public GameObject vfx;
    public GameObject vfx2;
    //score update
    public PlayerMovement player;
    public float lengthIncrease = 0.25f;
    public AudioSource hit;
    public float pushForce = 1f;

    private GameManager gameManager;
    private EnemyPhase phase = EnemyPhase.Unknown;
    private SpriteRenderer renderer;
    private Collider2D collider;
    Color currColor;

    public EnemyPhase Phase => phase;

    protected void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        currColor = renderer.color;
        collider = GetComponent<Collider2D>();

    }
    // Start is called before the first frame update
    protected void Start()
    {
        
        float randAngle = Random.Range(0, Mathf.Deg2Rad * 360);
        forward = new Vector3(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (phase == EnemyPhase.Active)
        {
            transform.position += moveSpeed * forward * Time.deltaTime;
        }
        //accelerate enemies and make them go a different direction

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        //when it gets hit by the push
        if (collision.gameObject.CompareTag("Attack"))
        {
            GameObject spawnedfx = Instantiate(vfx2, transform.position, Quaternion.identity) as GameObject;
            hit.Play();

            Vector3 dir = collision.transform.position - transform.position;
            forward = -dir.normalized;
            if(gameObject != null)
            {
                gameObject.transform.position += forward * player.pushSize;

            }



        }
    }
    public void Die(Vector3 pos)
    {
        HandleHit();
        GameObject spawnedfx = Instantiate(vfx, pos, Quaternion.identity) as GameObject;
        //forward *= -1;
        gameManager.UpdateScore();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Wall"))
        {
            //Debug.Log("wall detected");
            forward = Vector3.Reflect(forward, collision.contacts[0].normal);
        }
    }

    [SerializeField, Range(0, 5)] private float spawnDuration;
    private void OnEnable()
    {
        phase = EnemyPhase.Spawning;
        StartCoroutine(ChangeOpacity(0f, 1f, spawnDuration));
    }

    private void OnDisable()
    {
        phase = EnemyPhase.Unknown;
    }
    IEnumerator ChangeOpacity(float start, float end, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            currColor.a = Mathf.Lerp(start, end, normalizedTime);
            renderer.color = currColor;
            yield return null;
        }
        currColor.a = end;
        renderer.color = currColor;
        phase = EnemyPhase.Active;
    }

    public void HandleHit()
    {
        StartCoroutine(FlashHit());
    }
    IEnumerator FlashHit()
    {
        renderer.material = data.HitMat;
        yield return new WaitForSeconds(data.FlashTime);
        renderer.material = data.DefaultMat;
        this.gameObject.SetActive(false);
    }
}



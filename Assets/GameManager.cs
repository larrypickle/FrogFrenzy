using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

enum ObjectTypes
{
    None = 0,
    Player = 1,
    Enemy
}

enum EnemyType
{
    Snake = 0,
    Turtle,
    Apple
}

public class GameManager : MonoBehaviour
{
    float timer = 0f;
    //public GameObject enemy;
    public float enemyTimerRate = 2f;
    

    private float currScore = 0f;
    public TextMeshProUGUI scoreText;

    private Grid gameBoard;
    [SerializeField] private Vector2Int boardDimension = new Vector2Int(10, 10);
    [SerializeField] private Vector2 startPos = new Vector2(-7, 4.6f);

    public static GameManager instance;

    public Grid GetGrid => gameBoard;

    private float SpawnRate;


    private PlayerMovement player;

    private void Awake()
    {
        instance = this;
        gameBoard = new Grid(boardDimension, startPos);
    }

    //different enemy spawn bools
    private bool AppleEnemy = false;
    private bool SpinningEnemy = false;
    
    // Start is called before the first frame update
    void Start()
    {
        currScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        //spawns more enemies over time
        if (timer <= 0)
        {
            Vector3 positionToSpawn = gameBoard.GetPosition(gameBoard.RandPos(Vector2Int.one), Vector2.zero);
            ObjectPooler.Instance.SpawnFromPool(EnemyType.Snake.ToString(), positionToSpawn, Quaternion.identity);
            //GameObject enemyObj = Instantiate(enemy, new Vector3(0f, 0f), Quaternion.identity) as GameObject;
            if (SpinningEnemy)
            {
                if(Random.Range(0f, 1f) <= SpawnRate)
                {
                    //ObjectPooler.Instance.SpawnFromPool(Enemy[1], new Vector3(0, 0), Quaternion.identity);
                    ObjectPooler.Instance.SpawnFromPool(EnemyType.Turtle.ToString(), new Vector3(0f, 0f), Quaternion.identity);
                }
            }

            if (AppleEnemy)
            {
                //ObjectPooler.Instance.SpawnFromPool(Enemy[2], new Vector3(Random.Range(-5f, 5f), 4f), Quaternion.identity);
                ObjectPooler.Instance.SpawnFromPool(EnemyType.Apple.ToString(), new Vector3(Random.Range(-5f, 5f), 4f), Quaternion.identity);
            }
            
            timer = enemyTimerRate;
        }

        if (Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene("SampleScene");
        }

        
    }
    public void UpdateScore()
    {
        currScore++;
        scoreText.SetText("Score: " + currScore.ToString());
        if (currScore > 15f)
        {
            enemyTimerRate = 0.8f;
            //player.moveSpeed += 1f;
            //player.time -= 0.2f;
            SpinningEnemy = true;
            SpawnRate = 0.3f;
        }
        if (currScore > 30f)
        {
            enemyTimerRate = 0.6f;
            SpawnRate += 0.3f;
        }
        if (currScore > 50f)
        {
            enemyTimerRate = 0.45f;
            SpawnRate = 1;
        }

        if(currScore > 70f)
        {
            enemyTimerRate = 0.2f;
        }
    }

    public void SetPlayer(PlayerMovement player)
    {
        this.player = player;
    }
}

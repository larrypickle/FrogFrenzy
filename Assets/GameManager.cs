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

public class GameManager : MonoBehaviour
{
    float timer = 0f;
    //public GameObject enemy;
    public float enemyTimerRate = 2f;
    public string[] Enemy;
    

    private float currScore = 0f;
    public TextMeshProUGUI scoreText;
    public PlayerMovement player;

    private Grid gameBoard;
    [SerializeField] private Vector2Int boardDimension = new Vector2Int(10, 10);
    [SerializeField] private Vector2 startPos = new Vector2(-7, 4.6f);

    public static GameManager instance;

    public Grid GetGrid => gameBoard;

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
            ObjectPooler.Instance.SpawnFromPool(Enemy[0], new Vector3(0, 0), Quaternion.identity);
            //GameObject enemyObj = Instantiate(enemy, new Vector3(0f, 0f), Quaternion.identity) as GameObject;
            if (SpinningEnemy)
            {
                ObjectPooler.Instance.SpawnFromPool(Enemy[1], new Vector3(0, 0), Quaternion.identity);
            }

            if (AppleEnemy)
            {
                ObjectPooler.Instance.SpawnFromPool(Enemy[2], new Vector3(Random.Range(-5f, 5f), 4f), Quaternion.identity);
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
        }
        if (currScore > 20f)
        {
            enemyTimerRate = 0.6f;
        }
        if (currScore > 30f)
        {
            enemyTimerRate = 0.45f;
            AppleEnemy = true;
        }
    }
}

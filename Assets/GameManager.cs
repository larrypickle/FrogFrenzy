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
    public GameObject enemy;
    public float enemyTimerRate = 2f;
    public string Enemy;

    public float currScore = 0f;
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
            ObjectPooler.Instance.SpawnFromPool(Enemy, new Vector3(0, 0), Quaternion.identity);
            //GameObject enemyObj = Instantiate(enemy, new Vector3(0f, 0f), Quaternion.identity) as GameObject;
            timer = enemyTimerRate;
        }

        if (Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene("SampleScene");
        }

        if(currScore > 10f)
        {
            enemyTimerRate = 0.8f;
            //player.moveSpeed += 1f;
            //player.time -= 0.2f;
        }
        if(currScore > 20f)
        {
            enemyTimerRate = 0.6f;
        }
        if(currScore > 30f)
        {
            enemyTimerRate = 0.45f;
        }
    }
    public void UpdateScore()
    {
        currScore++;
        scoreText.SetText("Score: " + currScore.ToString());
    }
}

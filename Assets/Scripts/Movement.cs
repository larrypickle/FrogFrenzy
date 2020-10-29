using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 0.82f;

    private Rigidbody2D rb;
    int[,] grid = new int[25, 25];
    const int PLAYER = 1;
    const int SWITCH = 2;
    const int GOAL = 3;
    int PLAYER_X = 0;
    int PLAYER_Y = 0;
    int PLAYER_MOVES_REMAINING = 100;
    [SerializeField] AudioSource tick;
    [SerializeField] AudioSource tock;
    bool ticktock = true;
    Vector3 oldPos = Vector3.zero;

    private UnityEngine.UI.Text stepText;
    private bool gameEnded = false;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        for (int i = 0; i < 25; ++i)
        {
            int[] rows = new int[25];
            for (int j = 0; j < 25; ++j)
            {
                grid[i, j] = 0;
            }
        }
        grid[0, 0] = PLAYER;
        grid[0, 13] = SWITCH;
        grid[12, 23] = GOAL;
        oldPos = transform.position;

        stepText = GameObject.Find("StepText").GetComponent<UnityEngine.UI.Text>();
        UpdateSteps();
        gameEnded = false;
    }

    private void Update()
    {
        if (gameEnded) return;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) 
        {
            /*
            if (Physics.Linecast(transform.position, (transform.position + (Vector3.down * speed*Time.fixedDeltaTime))))
            {
                Debug.Log("Something's in the way!");
            }
            else
                transform.position += Vector3.down * speed*Time.deltaTime;
            */
            rb.MovePosition(rb.position + (Vector2.down * speed));
            if(!(transform.position == oldPos))
            {
                PLAYER_Y += 1;
                PLAYER_MOVES_REMAINING -= 1;
                UpdateSteps();
                oldPos = transform.position;
                if (ticktock)
                {
                    tick.Play();
                    ticktock = false;
                }
                else
                {
                    tock.Play();
                    ticktock = true;
                }
            }
            // transform.position += Vector3.down * speed * Time.fixedDeltaTime;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            rb.MovePosition(rb.position + (Vector2.up * speed));
            if (!(transform.position == oldPos))
            {
                PLAYER_Y -= 1;
                // transform.position += Vector3.up * speed;
                // transform.position += Vector3.up * speed * Time.fixedDeltaTime;
                PLAYER_MOVES_REMAINING -= 1;
                UpdateSteps();
                oldPos = transform.position;
                if (ticktock)
                {
                    tick.Play();
                    ticktock = false;
                }
                else
                {
                    tock.Play();
                    ticktock = true;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            rb.MovePosition(rb.position + (Vector2.right * speed));
            if (!(transform.position == oldPos))
            {
                PLAYER_X += 1;
                // transform.position += Vector3.right * speed;
                // transform.position += Vector3.right * speed * Time.fixedDeltaTime;
                PLAYER_MOVES_REMAINING -= 1;
                UpdateSteps();
                oldPos = transform.position;
                if (ticktock)
                {
                    tick.Play();
                    ticktock = false;
                }
                else
                {
                    tock.Play();
                    ticktock = true;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            rb.MovePosition(rb.position + (Vector2.left * speed));
            if (!(transform.position == oldPos))
            {
                PLAYER_X -= 1;
                // transform.position += Vector3.left * speed;
                // transform.position += Vector3.left * speed * Time.fixedDeltaTime;
                PLAYER_MOVES_REMAINING -= 1;
                UpdateSteps();
                oldPos = transform.position;
                if (ticktock)
                {
                    tick.Play();
                    ticktock = false;
                }
                else
                {
                    tock.Play();
                    ticktock = true;
                }
            }
        }
    }

    private void UpdateSteps()
    {
        if (PLAYER_MOVES_REMAINING > 0)
        {
            stepText.text = PLAYER_MOVES_REMAINING.ToString() + " Steps Left";
        }
        else
        {
            stepText.text = "0 Steps Left";
            gameEnded = true;
            // GameObject.Find("GameManager").GetComponent<GameManager>().OnNoStepsLeft();
        }
    }
}

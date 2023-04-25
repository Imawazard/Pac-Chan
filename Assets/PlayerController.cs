using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MovementController movementController;

    public SpriteRenderer sprite;
    public Animator animator;

    public GameObject startNode;

    public GameManager gameManager;

    private KeyCode upMovementKey;
    private KeyCode downMovementKey;
    private KeyCode leftMovementKey;
    private KeyCode rightMovementKey;

    public bool isDead = false; 

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        movementController = GetComponent<MovementController>();
        startNode = movementController.currentNode; 
    }

    public void Setup(Vector2 startPos, KeyCode moveUp, KeyCode moveLeft, KeyCode moveDown, KeyCode moveRight, string movementDirection)
    {
        isDead = false;
        animator.SetBool("dead", false);
        animator.SetBool("moving", false);
        movementController.currentNode = startNode;
        movementController.direction = movementDirection;
        movementController.lastMovingDirection = movementDirection;
        sprite.flipX = false;
        transform.position = startPos;
        animator.speed = 1;
        upMovementKey = moveUp;
        downMovementKey = moveDown;
        rightMovementKey = moveRight;
        leftMovementKey = moveLeft;
    }

    public void Stop()
    {
        animator.speed = 0; 
    }

    // Update is called once per frame
    void Update()
    {

        if (!gameManager.gameIsRunning)
        {
            if (!isDead)
            {
                animator.speed = 0; 
            }
            
            return; 
        }

        if (isDead)
            return;

        animator.speed = 1; 

        animator.SetBool("moving", true);

        if (Input.GetKeyDown(leftMovementKey))
        {
            movementController.SetDirection("left");
        }
        if (Input.GetKeyDown(rightMovementKey))
        {
            movementController.SetDirection("right");
        }
        if (Input.GetKeyDown(upMovementKey))
        {
            movementController.SetDirection("up");
        }
        if (Input.GetKeyDown(downMovementKey))
        {
            movementController.SetDirection("down");
        }



        bool flipX = false;
        bool flipY = false; 
        if (movementController.lastMovingDirection == "left")
        {
            animator.SetInteger("direction", 0);
        }
        else if (movementController.lastMovingDirection == "right")
        {
            animator.SetInteger("direction", 0);
            flipX = true;
        }
        else if (movementController.lastMovingDirection == "up")
        {
            animator.SetInteger("direction", 1);
        }
        else if (movementController.lastMovingDirection == "down")
        {
            animator.SetInteger("direction", 1);
            flipY = true;
        }

        sprite.flipY = flipY;
        sprite.flipX = flipX;
    }

    public void Death()
    {
        isDead = true;
        animator.SetBool("moving", false);
        animator.speed = 1; 
        animator.SetBool("dead", true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGboss : MonoBehaviour
{
    public float idleSpeed;
    public Vector2 idleMoveDir;
    public float attackSpeed;
    public Vector2 attackMoveDir;
    public float attackPlayerSpeed;
    Transform player;
    private Vector2 playerPos;
    public Transform groundCheckUp;
    public Transform groundCheckDown;
    public Transform groundCheckWall;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isTouchingUp;
    private bool isTouchingDown;
    private bool isTouchingWall;
    private bool goingUp = true;
    private bool facingLeft = true;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance.transform;
        idleMoveDir.Normalize();
        attackMoveDir.Normalize();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingUp = Physics2D.OverlapCircle(groundCheckUp.position, groundCheckRadius, groundLayer);
        isTouchingDown = Physics2D.OverlapCircle(groundCheckDown.position, groundCheckRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(groundCheckWall.position, groundCheckRadius, groundLayer);
        IdleState();
    }

    void IdleState()
    {
        if(isTouchingUp && goingUp)
        {
            ChangeDirection();
        }
        else if(isTouchingDown && !goingUp)
        {
            ChangeDirection();
        }

        if (isTouchingWall)
        {
            if (facingLeft)
            {
                Flip();
            }
            else if (!facingLeft)
            {
                Flip();
            }
        }
        rb.velocity = idleSpeed * idleMoveDir;
    }

    void attackUpAndDown()
    {
        if (isTouchingUp && goingUp)
        {
            ChangeDirection();
        }
        else if (isTouchingDown && !goingUp)
        {
            ChangeDirection();
        }

        if (isTouchingWall)
        {
            if (facingLeft)
            {
                Flip();
            }
            else if (!facingLeft)
            {
                Flip();
            }
        }
        rb.velocity = attackSpeed * attackMoveDir;
    }

    void attackPlayer()
    {
        //take player pos
        //normalize player pos
        //attack that pos
        flipTowardsPlayer();
        //??
        playerPos = player.position - transform.position;
        playerPos.Normalize();
        rb.velocity = playerPos * attackPlayerSpeed;
    }

    void flipTowardsPlayer()
    {
        float playerDir = player.position.x - transform.position.x;

        if(playerDir > 0 && facingLeft)
        {
            Flip();
        }
        else if(playerDir < 0 && !facingLeft)
        {
            Flip();
        }
    }

    void ChangeDirection()
    {
        goingUp = !goingUp;
        idleMoveDir.y *= -1;
        attackMoveDir.y *= -1;
    }

    void Flip()
    {
        facingLeft = !facingLeft;
        idleMoveDir.x *= -1;
        attackMoveDir.x *= -1;
        transform.Rotate(0, 180, 0);
    }

}

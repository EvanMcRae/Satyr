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
    public Transform groundCheckUp;
    public Transform groundCheckDown;
    public Transform groundCheckWall;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isTouchingUp;
    private bool isTouchingDown;
    private bool isTouchingWall;
    private bool goingUp = true;
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
        rb.velocity = idleSpeed * idleMoveDir;
    }

    void ChangeDirection()
    {
        goingUp = !goingUp;
        idleMoveDir.y *= -1;
        attackMoveDir.y *= -1;
    }

}

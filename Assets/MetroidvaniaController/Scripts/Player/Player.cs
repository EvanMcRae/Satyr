using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player controller;
    public static GameObject instance;
    public Transform camTarget;

    [SerializeField] private float m_JumpForce = 2000f;                         // Amount of force added when the player jumps.
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;   // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] public Transform m_GroundCheck;                            // A position marking where to check if the player is grounded.
    [SerializeField] public Transform m_LeftGroundCheck, m_RightGroundCheck;    // Positions marking where to check if the player is solidly grounded.
    [SerializeField] private Transform m_WallCheck;								//Posicion que controla si el personaje toca una pared

    [SerializeField] private AudioManager am;
    [SerializeField] private Transform groundParticle;

    const float k_GroundedRadius = .12f; // Radius of the overlap circle to determine if grounded
    public bool m_Grounded;            // Whether or not the player is grounded.
    public bool m_leftGrounded, m_rightGrounded; // Whether or not the player is solidly grounded.
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 velocity = Vector3.zero;
    private float limitFallSpeed = 30f; // Limit fall speed


    public bool wallSlide_Unlocked = false;
    public bool doubleJump_Unlocked = false;
    public bool canDoubleJump = false; //If player can double jump
    [SerializeField] private float m_DashForce = 25f;
    public bool canDash = false;
    public bool isDashing = false; //If player is dashing
    private bool speedBoost = false; //Gives player speed boost during dash
    private bool m_IsWall = false; //If there is a wall in front of the player
    private bool isWallSliding = false; //If player is sliding in a wall
    private bool oldWallSlidding = false; //If player is sliding in a wall in the previous frame
    private float prevVelocityX = 0f;
    private bool canCheck = false; //For check if player is wallsliding
    public float wallSlideSpeed = 0;
    // the default wallSlidSpeed was -5

    private float cantMove = 0f;
    public float life = 10f; //Life of the player
    public bool invincible = false; //If player can die
    public bool dead = false; // Dead status
    private bool canMove = true; //If player can move
    public bool isJumping = false;
    public bool isJumpingDJ = false;
    public bool resetting = false;
    public float jumpTime;
    private bool holdingJump = false;

    public float stunDuration = 0.25f;
    public float iFrames = 1f;
    public float lastOnLand = 0f;
    public float jumpCooldown = 0f;
    public float beenOnLand = 0f;
    private Transform reset_point;
    public bool inDeathZone;
    private Vector3 lastOnLandLocation;

    public Ground.GroundType currentGround = Ground.GroundType.ROCK;
    public Ground.GroundType previousGround = Ground.GroundType.ROCK;
    public bool switchedGround;

    private Animator animator;
    public ParticleSystem particleJumpUp; //Trail particles
    public ParticleSystem particleJumpDown; //Explosion particles
    public ParticleSystem particleLand; //Big landing particles

    private float jumpWallStartX = 0;
    private float jumpWallDistX = 0; //Distance between player and wall
    private bool limitVelOnWallJump = false; //For limit wall jump distance with low fps

    // one time events
    public bool explorer = false;
    public bool initialFall = false;

    public PhysicsMaterial2D slippery, friction;

    [Header("Events")]
    [Space]

    public UnityEvent OnFallEvent;
    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    private void Awake()
    {
        inDeathZone = false;

        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (OnFallEvent == null)
            OnFallEvent = new UnityEvent();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        // reset_point.position = new Vector3(m_GroundCheck.position.x, m_GroundCheck.position.y + 2f, m_GroundCheck.position.z);

        // Singleton design pattern
        if (controller != null && controller != this)
        {
            // Destroy(gameObject);
        }
        else
        {
            controller = this;
            instance = gameObject;
            // camTarget = GameObject.FindGameObjectWithTag("CamTarget").transform;
            DontDestroyOnLoad(gameObject);
        }

        // // TODO this code may be redundant/useless and possibly problematic if we want to do multiplayer someday
        // // keeping it in for now cause it's pretty standard but yeah
        // if (FindObjectsOfType<Player>().Length > 1)
        // {
        //     Destroy(gameObject);
        // }
    }

    private void FixedUpdate()
    {
        if (isDashing) {
            GetComponent<Attack>().DoDashDamage(0.0f);
        }

        if (!dead) {
            animator.SetBool("IsDead", false);
        }

        if (controller == null)
        {
            controller = this;
            instance = gameObject;
            // camTarget = GameObject.FindGameObjectWithTag("CamTarget").transform;
        }

        GameObject rp = GameObject.FindGameObjectWithTag("Reset Point");
        if (rp != null)
            reset_point = rp.transform;

        if (lastOnLand == 0.0f)
        {
            lastOnLandLocation = transform.position;
        }

        lastOnLand += Time.fixedDeltaTime;

        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        bool identifiedGround = false;
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "Enemy")
            {
                m_Grounded = true;
                lastOnLand = 0f;
                OnLandEvent.Invoke();

                if (colliders[i].gameObject.transform.position.x > this.transform.position.x)
                {
                    //ApplyDamage(1.0f, new Vector3(colliders[i].gameObject.transform.position.x * 1.01f, colliders[i].gameObject.transform.position.y, colliders[i].gameObject.transform.position.z), 100f);
                    //this.GetComponent<health>().playerHealth -= 1;
                    m_Rigidbody2D.AddForce(new Vector2(-10f, 5f));
                }
                else
                {
                    //this.GetComponent<health>().playerHealth -= 1;
                    m_Rigidbody2D.AddForce(new Vector2(10f, 5f));
                }

            }
            if (colliders[i].gameObject != gameObject && (colliders[i].gameObject.tag == "obstacle"))
            {
                m_Grounded = true;
                canDoubleJump = false;

                if (!wasGrounded && jumpCooldown <= 0.1f)
                    jumpCooldown = 0.05f;

                if (!wasGrounded && !holdingJump && !(m_Rigidbody2D.velocity.y > 0f))
                {
                    if (!m_IsWall && !isDashing)
                    {
                        particleJumpDown.Play();
                    }

                    if (m_Rigidbody2D.velocity.y < 0f)
                        limitVelOnWallJump = false;
                }
            }
            else if (colliders[i].gameObject != gameObject && (colliders[i].gameObject.tag == "Ground" || colliders[i].gameObject.tag == "Wall" || colliders[i].gameObject.tag == "Breakable Wall")) {
                m_Grounded = true;
                lastOnLand = 0f;
                canDoubleJump = false;

                // identifies ground material for run sounds
                if (!identifiedGround)
                {
                    previousGround = currentGround;
                    Ground thisGround = colliders[i].gameObject.GetComponent<Ground>();
                    if (thisGround != null)
                        currentGround = thisGround.type;
                    else
                        currentGround = Ground.GroundType.ROCK;
                    switchedGround = currentGround != previousGround;
                    identifiedGround = true;
                }

                if (!wasGrounded && jumpCooldown <= 0.1f) {
                    jumpCooldown = 0.05f;
                }
                    
                if (!wasGrounded && !holdingJump && !(m_Rigidbody2D.velocity.y > 0f))
                {
                    if (!m_IsWall && !isDashing)
                    {
                        particleJumpDown.Play();
                    }

                    if (m_Rigidbody2D.velocity.y < 0f)
                        limitVelOnWallJump = false;
                }
            }
        }

        // check left position
        Collider2D[] leftColliders = Physics2D.OverlapCircleAll(m_LeftGroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        m_leftGrounded = false;
        for (int i = 0; i < leftColliders.Length; i++)
        {
            if (leftColliders[i].gameObject != gameObject && (leftColliders[i].gameObject.tag == "Ground" || leftColliders[i].gameObject.tag == "Wall"))
            {
                m_leftGrounded = true;
                break;
            }
        }

        // check right position
        Collider2D[] rightColliders = Physics2D.OverlapCircleAll(m_RightGroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        m_rightGrounded = false;
        for (int i = 0; i < rightColliders.Length; i++)
        {
            if (rightColliders[i].gameObject != gameObject && (rightColliders[i].gameObject.tag == "Ground" || rightColliders[i].gameObject.tag == "Wall"))
            {
                m_rightGrounded = true;
                break;
            }
        }

        // if left, right, and middle are all grounded, you are solidly grounded. update reset point pos
        if (m_leftGrounded && m_rightGrounded && m_Grounded && !inDeathZone)
        {
            reset_point.position = new Vector3(m_GroundCheck.position.x, m_GroundCheck.position.y + 2f, m_GroundCheck.position.z);
        }

        m_IsWall = false;

        if (!m_Grounded)
        {
            beenOnLand = 0f;
            OnFallEvent.Invoke();
            Collider2D[] collidersWall = Physics2D.OverlapCircleAll(m_WallCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < collidersWall.Length; i++)
            {
                if (collidersWall[i].gameObject != null)
                {
                    isDashing = false;
                    m_IsWall = true;
                }
            }
            prevVelocityX = m_Rigidbody2D.velocity.x;
        }
        else
        {
            if (beenOnLand < 5f)
                beenOnLand += Time.fixedDeltaTime;
            if (!(m_Rigidbody2D.velocity.y > 0f)) {
                OnLandEvent.Invoke();
                isJumping = false;
                isJumpingDJ = false;
                jumpTime = 0f;
            }
            if (jumpCooldown > 0f)
                jumpCooldown -= Time.fixedDeltaTime;
        }

        if (limitVelOnWallJump)
        {
            if (m_Rigidbody2D.velocity.y < -0.5f)
                limitVelOnWallJump = false;
            jumpWallDistX = (jumpWallStartX - transform.position.x) * transform.localScale.x;
            if (jumpWallDistX < -0.5f && jumpWallDistX > -1f)
            {
                canMove = true;
            }
            else if (jumpWallDistX < -1f && jumpWallDistX >= -2f)
            {
                canMove = true;
                m_Rigidbody2D.velocity = new Vector2(1f * transform.localScale.x, m_Rigidbody2D.velocity.y);
            }
            else if (jumpWallDistX < -2f)
            {
                limitVelOnWallJump = false;
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
            }
            else if (jumpWallDistX > 0)
            {
                limitVelOnWallJump = false;
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
            }
        }

        // Prevent jumping off screen during cutscenes
        if (Statue.cutscening)
        {
            if (transform.position.y >= (Statue.currStatue.position.y + 1.75f) && m_Rigidbody2D.velocity.y > 0)
            {
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
            }
        }
    }

    public void Move(float move, bool jump, bool dash, bool releaseJump)
    {
        if (isJumping && !canDoubleJump && doubleJump_Unlocked)
        {
            canDoubleJump = true;
        }

        if (releaseJump && canDoubleJump)
        {
            if (isJumping)
                isJumpingDJ = true;
            isJumping = false;
            jumpTime = 0;
        }

        if (canMove)
        {
            if (GetComponent<Attack>().specialCooldown > 5f) {
                canDash = true;
            }
            if (dash && canDash && !isWallSliding)
            {
                StartCoroutine(DashCooldown());
            }
            // If crouching, check to see if the character can stand up
            if (speedBoost)
            {
                m_Rigidbody2D.velocity = new Vector2(transform.localScale.x * m_DashForce, 0);
                // Prevent moving off screen during cutscenes
                if (Statue.cutscening)
                {
                    if (transform.position.x <= (Statue.currStatue.position.x - 6.5f) && m_Rigidbody2D.velocity.x < 0)
                    {
                        m_Rigidbody2D.velocity = Vector2.zero;
                    }
                    if (transform.position.x >= (Statue.currStatue.position.x + 6.5f) && m_Rigidbody2D.velocity.x > 0)
                    {
                        m_Rigidbody2D.velocity = Vector2.zero;
                    }
                }
            }
            //only control the player if grounded or airControl is turned on
            else if (m_Grounded || m_AirControl)
            {
                if (m_Rigidbody2D.velocity.y < -limitFallSpeed)
                    m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -limitFallSpeed);
                
                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);

                // Prevent moving off screen during cutscenes
                if (Statue.cutscening)
                {
                    if (transform.position.x <= (Statue.currStatue.position.x - 6.5f) && targetVelocity.x < 0)
                    {
                        targetVelocity.x = 0;
                        m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
                    }
                    if (transform.position.x >= (Statue.currStatue.position.x + 6.5f) && targetVelocity.x > 0)
                    {
                        targetVelocity.x = 0;
                        m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
                    }
                }

                // And then smoothing it out and applying it to the character
                if (move == 0.0 && m_Rigidbody2D.velocity.x != 0.0f)
                {
                    GetComponent<CapsuleCollider2D>().sharedMaterial = friction;
                    m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing*2.5f);
                }
                else
                {
                    GetComponent<CapsuleCollider2D>().sharedMaterial = slippery;
                    m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);
                }

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight && !isWallSliding)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight && !isWallSliding)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...
            if ((lastOnLand < 0.1f) && jump && !isJumping && !canDoubleJump) // incorporates coyote time with lastOnLand
            {
                // Add a vertical force to the player.
                animator.SetBool("JumpUp", true);
                animator.SetBool("IsJumping", true);

                m_Grounded = false;
                if (!isJumping)
                {
                    particleJumpDown.Play();
                    particleJumpUp.Play();
                    holdingJump = true;
                }
                isJumping = true;
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * .7f)); //force added during a jump
                if (doubleJump_Unlocked) { canDoubleJump = true; }
            }
            else if (!m_Grounded && jump && canDoubleJump && !isWallSliding && !isJumping)
            {
                if (doubleJump_Unlocked) { canDoubleJump = false; }
                holdingJump = true;
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce / 1.2f));
                animator.SetBool("IsDoubleJumping", true);
            }

            else if (m_IsWall && !m_Grounded && wallSlide_Unlocked) // looks like this is where wall sliding is managed
            {
                if (!oldWallSlidding && m_Rigidbody2D.velocity.y < 0 || isDashing)
                {
                    isWallSliding = true;
                    m_WallCheck.localPosition = new Vector3(-m_WallCheck.localPosition.x, m_WallCheck.localPosition.y, 0);
                    // If the input is moving the player right and the player is facing right...
                    if (move > 0 && m_FacingRight)
                    {
                        // ... flip the player.
                        Flip();
                        animator.SetBool("IsWallSliding", true);
                    }
                    // Otherwise if the input is moving the player left and the player is facing left...
                    else if (move < 0 && !m_FacingRight)
                    {
                        // ... flip the player.
                        Flip();
                        animator.SetBool("IsWallSliding", true);
                    }
                    StartCoroutine(WaitToCheck(0.1f));
                    // if (doubleJump_Unlocked) { canDoubleJump = true; }
                }
                isDashing = false;

                if (isWallSliding)
                {
                    if (move * transform.localScale.x > 0.1f)
                    {
                        StartCoroutine(WaitToEndSliding());

                    }
                    else if (move == 0)
                    { // previously cancelled slide but that is not necessary - however this else statement still is lol
                        // StartCoroutine(WaitToEndSliding());
                        // isWallSliding = false;
                        // animator.SetBool("IsWallSliding", false);
                        // oldWallSlidding = false;
                        // m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                    }
                    else
                    {
                        // so this is where the wall sliding happens - aidan
                        oldWallSlidding = true;
                        m_Rigidbody2D.velocity = new Vector2(-transform.localScale.x * 2, wallSlideSpeed);
                        //	print("wall sliding?");
                    }
                }

                if (jump && isWallSliding && !isJumping)
                {
                    animator.SetBool("IsJumping", true);
                    animator.SetBool("JumpUp", true);
                    if (!isJumping)
                        holdingJump = true;
                    isJumping = true;
                    m_Rigidbody2D.velocity = new Vector2(0f, 0f);
                    m_Rigidbody2D.AddForce(new Vector2(transform.localScale.x * m_JumpForce * 0.5f, m_JumpForce * 0.8f));
                    jumpWallStartX = transform.position.x;
                    if ((move > 0 && !m_FacingRight) || (move < 0 && m_FacingRight) && move != 0)
                    {
                        limitVelOnWallJump = true;
                        canMove = false;
                    }

                    if (doubleJump_Unlocked) { canDoubleJump = true; }
                    isWallSliding = false;
                    animator.SetBool("IsWallSliding", false);
                    oldWallSlidding = false;
                    m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                }
                else if (dash && canDash)
                {
                    isWallSliding = false;
                    animator.SetBool("IsWallSliding", false);
                    oldWallSlidding = false;
                    m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                    // if (doubleJump_Unlocked) { canDoubleJump = true; }
                    StartCoroutine(DashCooldown());
                }
            }
            else if (isWallSliding && !m_IsWall && canCheck)
            {
                isWallSliding = false;
                animator.SetBool("IsWallSliding", false);
                oldWallSlidding = false;
                m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
                // if (doubleJump_Unlocked) { canDoubleJump = true; }
            }
        }
        else if (!dead && !resetting) // fix being stuck unable to move
        {
            cantMove += 0.01f;
            if (cantMove > stunDuration)
            {
                canMove = true;
                cantMove = 0f;
            }
        }

        if (isJumping || isJumpingDJ) // this code is absolutely gross but necessary
        {
            jumpTime += 0.1f;
            // if (jumpTime > 4.5f)
            // {
            //     if (jumpCooldown <= 0.1f)
            //         jumpCooldown = 0.1f;
            //     isJumping = false;
            //     isJumpingDJ = false;
            //     jumpTime = 0f;
            // }
        }

        if (releaseJump)
        {
            holdingJump = false;
        }

        //hold jump distance extentions
        if (holdingJump)
        {
            if (isJumping || isJumpingDJ)
            {
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce / 80 / jumpTime));
            }
        }

    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void ApplyDamage(float damage, Vector3 position, float knockBack)
    {
        ApplyDamage(damage, position, knockBack, false);
    }

    public void ApplyDamage(float damage, Vector3 position, float knockBack, bool bypass)
    {
        if ((invincible && bypass) || !invincible)
        {
            animator.SetBool("Hit", true);
            //		life -= damage;  orignial code
            // need to change to int, original is using float
            int integerDamageValue = (int)damage;
            this.GetComponent<health>().playerHealth -= (integerDamageValue);
            Vector2 damageDir = Vector3.Normalize(transform.position - position) * 85f;
            damageDir.y /= 2f; // reduces vertical knockback
                               // m_Rigidbody2D.velocity = Vector2.zero;
                               //Debug.Log(damageDir);
            m_Rigidbody2D.AddForce(damageDir * knockBack);
            if (this.GetComponent<health>().playerHealth <= 0 && !dead)
            {
                StartCoroutine(WaitToDead());
            }
            else
            {
                if (!bypass)
                    GetComponent<SimpleFlash>().Flash(iFrames, 3);
                StartCoroutine(Stun(stunDuration));
                StartCoroutine(MakeInvincible(iFrames));
            }
        }
    }

    IEnumerator DashCooldown()
    {
        isDashing = true;
        speedBoost = true;
        animator.SetBool("IsDashing", true);
        canDash = false;
        if (GetComponent<Attack>().specialCooldown >= 5f)
            GetComponent<Attack>().specialCooldown -= 5f;
        yield return new WaitForSeconds(0.1f);
        speedBoost = false;
        yield return new WaitForSeconds(0.3f);
        isDashing = false;
        GetComponent<Attack>().ignoredEnemies.Clear();
    }

    IEnumerator Stun(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        if (!resetting)
            canMove = true;
    }

    public void Invincible(float time) {
        StartCoroutine(MakeInvincible(time));
    }

    IEnumerator MakeInvincible(float time)
    {
        invincible = true;

        yield return new WaitForSeconds(time);
        if (!resetting)
        {
            invincible = false;
        }
    }
    IEnumerator WaitToMove(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerator WaitToCheck(float time)
    {
        canCheck = false;
        yield return new WaitForSeconds(time);
        canCheck = true;
    }

    IEnumerator WaitToEndSliding()
    {
        yield return new WaitForSeconds(0.1f);
        // if (doubleJump_Unlocked) { canDoubleJump = true; }
        isWallSliding = false;
        animator.SetBool("IsWallSliding", false);
        oldWallSlidding = false;
        m_WallCheck.localPosition = new Vector3(Mathf.Abs(m_WallCheck.localPosition.x), m_WallCheck.localPosition.y, 0);
    }

    IEnumerator WaitToDead()
    {
        animator.SetBool("IsDead", true);
        canMove = false;
        invincible = true;
        dead = true;
        GetComponent<Attack>().enabled = false;
        yield return new WaitForSeconds(0.4f);
        m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
        GameObject.Find("Crossfade").GetComponent<Animator>().SetTrigger("start");
        StartCoroutine(am.PitchDown());
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(GetComponent<Spawnpoint>().scene);
        // SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        dead = false;
        animator.SetBool("IsDead", false);
        //canMove = true;
        invincible = false;
        // transform.position = GameObject.Find("PlayerCheck").transform.position;
        transform.position = GetComponent<Spawnpoint>().position;
        // TODO may want to change these depending on if we have health boost effects
        GetComponent<health>().playerHealth = 5;
        GetComponent<health>().numberOfHearts = 5;
        GetComponent<Attack>().enabled = true;
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    public void GoToResetPoint()
    {
        if (this.GetComponent<health>().playerHealth > 0)
        {
            if (resetting || dead)
            {
                return;
            }
            StartCoroutine(ResetPoint());
        }
    }

    IEnumerator ResetPoint()
    {
        animator.SetBool("IsDead", true);
        m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
        resetting = true;
        StartCoroutine(WaitToMove(1));
        invincible = true;
        GameObject.Find("Crossfade").GetComponent<Animator>().SetTrigger("start");
        yield return new WaitForSeconds(1f);
        animator.SetBool("IsDead", false);
        transform.position = reset_point.position;
        FindObjectOfType<CameraFollow>().Snap(transform.position);
        m_Rigidbody2D.velocity = Vector2.zero;
        resetting = false;
        yield return new WaitForSeconds(2f);
        invincible = false;
    }
    
    public void LandParticles() { 
        particleLand.Play();
    }

}

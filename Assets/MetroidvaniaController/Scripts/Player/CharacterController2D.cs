using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

public class CharacterController2D : MonoBehaviour
{

    public static CharacterController2D instance;
    public static Transform camTarget;

	[SerializeField] private float m_JumpForce = 2000f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] public Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_WallCheck;								//Posicion que controla si el personaje toca una pared

    [SerializeField] private AudioManager am;
    [SerializeField] private Transform groundParticle;

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	public bool m_Grounded;            // Whether or not the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 velocity = Vector3.zero;
	private float limitFallSpeed = 25f; // Limit fall speed


	public bool wallSlide_Unlocked = false;
	public bool doubleJump_Unlocked = false;
	public bool canDoubleJump = false; //If player can double jump
	[SerializeField] private float m_DashForce = 25f;
	public bool canDash = false;
	private bool isDashing = false; //If player is dashing
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
    private bool dead = false; // Dead status
	private bool canMove = true; //If player can move
    public bool isJumping = false;
    private bool resetting = false;
    private float jumpTime;
    private bool holdingJump = false;

	public float stunDuration = 0.25f;
	public float iFrames = 1f;
    public float lastOnLand = 0f;
    public float jumpCooldown = 0f;
    private Transform reset_point;
    private float reset_point_update = 0f;
    private Vector3 lastOnLandLocation;

	private Animator animator;
	public ParticleSystem particleJumpUp; //Trail particles
	public ParticleSystem particleJumpDown; //Explosion particles

	private float jumpWallStartX = 0;
	private float jumpWallDistX = 0; //Distance between player and wall
	private bool limitVelOnWallJump = false; //For limit wall jump distance with low fps

	public bool explorer = false;

	[Header("Events")]
	[Space]

	public UnityEvent OnFallEvent;
	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		if (OnFallEvent == null)
			OnFallEvent = new UnityEvent();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		// reset_point.position = new Vector3(m_GroundCheck.position.x, m_GroundCheck.position.y + 2f, m_GroundCheck.position.z);

        // Singleton design pattern
        if (instance != null && instance != this) 
        {
            // Destroy(gameObject);
        }
        else
        {
            instance = this;
            camTarget = GameObject.FindGameObjectWithTag("CamTarget").transform;
            DontDestroyOnLoad(gameObject);
        }

        // // TODO this code may be redundant/useless and possibly problematic if we want to do multiplayer someday
        // // keeping it in for now cause it's pretty standard but yeah
        // if (FindObjectsOfType<CharacterController2D>().Length > 1)
        // {
        //     Destroy(gameObject);
        // }


	}


	private void FixedUpdate()
	{
        if (instance == null) {
            instance = this;
            camTarget = GameObject.FindGameObjectWithTag("CamTarget").transform;
        }

        reset_point = GameObject.FindGameObjectWithTag("Reset Point").transform;
        
        if (lastOnLand == 0.0f) {
            lastOnLandLocation = transform.position;
        }

        lastOnLand += Time.fixedDeltaTime;
        reset_point_update += Time.fixedDeltaTime;

		bool wasGrounded = m_Grounded;
		m_Grounded = false;
    
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject.tag == "Enemy")
            {
                m_Grounded = true;
                lastOnLand = 0f;

				if(colliders[i].gameObject.transform.position.x > this.transform.position.x)
                {
					//ApplyDamage(1.0f, new Vector3(colliders[i].gameObject.transform.position.x * 1.01f, colliders[i].gameObject.transform.position.y, colliders[i].gameObject.transform.position.z), 100f);
					//this.GetComponent<health>().playerHealth -= 1;
					m_Rigidbody2D.AddForce(new Vector2(-10f,5f));
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
                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();
                    jumpCooldown = 0.1f;
                    if (!m_IsWall && !isDashing)
                    {
                        particleJumpDown.Play();
                    }

                    if (m_Rigidbody2D.velocity.y < 0f)
                        limitVelOnWallJump = false;
                }
            }
            else if (colliders[i].gameObject != gameObject && (colliders[i].gameObject.tag == "Ground" || colliders[i].gameObject.tag == "Wall" || colliders[i].gameObject.tag == "Breakable Wall"))
                m_Grounded = true;
                lastOnLand = 0f;
                canDoubleJump = false;

                if (reset_point_update >= 3f && colliders[i].gameObject.tag != "obstacle")
                {
                    reset_point_update = 0f;
                    reset_point.position = new Vector3(m_GroundCheck.position.x, m_GroundCheck.position.y + 2f, m_GroundCheck.position.z);
                }

				if (!wasGrounded)
				{
					OnLandEvent.Invoke();
                    jumpCooldown = 0.1f;
					if (!m_IsWall && !isDashing) {
                        particleJumpDown.Play();
                    }
						
					if (m_Rigidbody2D.velocity.y < 0f)
						limitVelOnWallJump = false;
				}
		}

		m_IsWall = false;

		if (!m_Grounded)
		{
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
		} else {
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
	}


	public void Move(float move, bool jump, bool dash, bool releaseJump)
	{
        if (isJumping && !canDoubleJump && doubleJump_Unlocked) {
            canDoubleJump = true;
        }

        if (releaseJump && canDoubleJump) {
            isJumping = false;
            jumpTime = 0;
        }
        
		if (canMove) {
			if (dash && canDash && !isWallSliding)
			{
				m_Rigidbody2D.AddForce(new Vector2(transform.localScale.x * m_DashForce, 0f));
				StartCoroutine(DashCooldown());
			}
			// If crouching, check to see if the character can stand up
			if (isDashing)
			{
				m_Rigidbody2D.velocity = new Vector2(transform.localScale.x * m_DashForce, 0);
			}
			//only control the player if grounded or airControl is turned on
			else if (m_Grounded || m_AirControl)
			{
				if (m_Rigidbody2D.velocity.y < -limitFallSpeed)
					m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, -limitFallSpeed);
				// Move the character by finding the target velocity
				Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
				// And then smoothing it out and applying it to the character
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);

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
                if (!isJumping) {
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
                if (!isJumping)
                    holdingJump = true;
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce / 1.2f));
				animator.SetBool("IsDoubleJumping", true);
			}

			else if (m_IsWall && !m_Grounded && wallSlide_Unlocked == true) // looks like this is where wall sliding is managed
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
						
					} else if (move == 0) { // previously cancelled slide but that is not necessary - however this else statement still is lol
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
					m_Rigidbody2D.AddForce(new Vector2(transform.localScale.x * m_JumpForce *0.5f, m_JumpForce));
					jumpWallStartX = transform.position.x;
                    if ((move > 0 && !m_FacingRight) || (move < 0 && m_FacingRight) && move != 0) {
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
            if (cantMove > stunDuration) {
                canMove = true;
                cantMove = 0f;
            }
        }

        if (isJumping) // this code is absolutely gross but necessary
        {
            jumpTime += 0.1f;
            if (jumpTime > 4.5f) {
                jumpCooldown = 0.1f;
                isJumping = false;
                jumpTime = 0f;
            }
        }

        if (releaseJump)
        {
            holdingJump = false;
        }

		//hold jump distance extentions
        if (holdingJump) {
			if(isJumping){
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce / 80 / jumpTime));
			}
			else{
				//falling while still holding jump
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce / 200));
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
            Vector2 damageDir = Vector3.Normalize(transform.position - position) * 85f ;
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
                StartCoroutine(Stun(stunDuration));
                StartCoroutine(MakeInvincible(iFrames));
            }
        }
	}

	IEnumerator DashCooldown()
	{
		animator.SetBool("IsDashing", true);
		isDashing = true;
		canDash = false;
		yield return new WaitForSeconds(0.1f);
		isDashing = false;
		yield return new WaitForSeconds(0.5f);
		canDash = true;
	}

	IEnumerator Stun(float time) 
	{
		canMove = false;
		yield return new WaitForSeconds(time);
        if (!resetting)
		    canMove = true;
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
        yield return new WaitForSeconds(1f);
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        dead = false;
        animator.SetBool("IsDead", false);
        canMove = true;
        invincible = false;
        transform.position = GameObject.Find("PlayerCheck").transform.position;
        // TODO may want to change these depending on if we have health boost effects
        GetComponent<health>().playerHealth = 5;
        GetComponent<health>().numberOfHearts = 5;
        GetComponent<Attack>().enabled = true;
        yield return new WaitForSeconds(1f);
	}

    public void GoToResetPoint() {
        if (this.GetComponent<health>().playerHealth > 0) {
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
        GameObject.Find("actual camera").GetComponent<CameraFollow>().Snap();
        m_Rigidbody2D.velocity = Vector2.zero;
        resetting = false;
        yield return new WaitForSeconds(2f);
        invincible = false;
    }
}

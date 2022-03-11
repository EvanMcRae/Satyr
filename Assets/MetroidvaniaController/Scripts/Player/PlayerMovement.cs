using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public Animator animator;

	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;
    bool releaseJump = false;
	bool dash = false;

	public bool dash_Unlocked = false;

	//bool dashAxis = false;

	// Update is called once per frame
	void Update () {

		if (Input.GetAxisRaw("Horizontal") > 0.3 || Input.GetAxisRaw("Horizontal") < -0.3)
			horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
		else
			horizontalMove = 0f;

		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

		if (Input.GetKeyDown(KeyCode.Space))
		{
            if (Player.controller.beenOnLand >= 0.1f || Player.controller.canDoubleJump || Player.controller.lastOnLand < 0.1f)
			    jump = true;
		}

        if ((Input.GetKey("space") || Input.GetKey("z") || Input.GetKey("joystick button 0")) && Player.controller.m_Grounded)
        {
            if (Player.controller.jumpCooldown <= 0f && !Player.controller.isJumping && !Player.controller.isJumpingDJ)
                jump = true;
			//print("tries to reset point?");
			//reset_point.position = new Vector3(m_GroundCheck.position.x, m_GroundCheck.position.y, m_GroundCheck.position.z);
			// Player.controller.reset_point.position = Player.controller.m_GroundCheck.position;
		}

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp("z") || Input.GetKeyUp("joystick button 0"))
        {
            releaseJump = true;
            Player.controller.jumpCooldown = 0f;
        }

		if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftShift)) && dash_Unlocked == true)
		{
			dash = true;
		}

		/*if (Input.GetAxisRaw("Dash") == 1 || Input.GetAxisRaw("Dash") == -1) //RT in Unity 2017 = -1, RT in Unity 2019 = 1
		{
			if (dashAxis == false)
			{
				dashAxis = true;
				dash = true;
			}
		}
		else
		{
			dashAxis = false;
		}
		*/

	}

	public void OnFall()
	{
		animator.SetBool("IsJumping", true);
	}

	public void OnLanding()
	{
        if (!Player.controller.isJumping && !Player.controller.isJumpingDJ) {
            animator.SetBool("IsJumping", false);
        }
	}

	void FixedUpdate ()
	{
		// Move our character
		Player.controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash, releaseJump);
        if (animator.GetBool("IsJumping") && Player.controller.m_Grounded && !Player.controller.isJumping && !Player.controller.isJumpingDJ) {
            animator.SetBool("IsJumping", false);
        }
		jump = false;
		dash = false;
        releaseJump = false;
	}
}

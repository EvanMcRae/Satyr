using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
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
			jump = true;
		}

        if ((Input.GetKey("space") || Input.GetKey("z") || Input.GetKey("joystick button 0")) && controller.m_Grounded)
        {
            jump = true;
			//print("tries to reset point?");
			//reset_point.position = new Vector3(m_GroundCheck.position.x, m_GroundCheck.position.y, m_GroundCheck.position.z);
			// controller.reset_point.position = controller.m_GroundCheck.position;
		}

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp("z") || Input.GetKeyUp("joystick button 0"))
        {
            releaseJump = true;
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
		animator.SetBool("IsJumping", false);
	}

	void FixedUpdate ()
	{
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash, releaseJump);
		jump = false;
		dash = false;
        releaseJump = false;
	}
}

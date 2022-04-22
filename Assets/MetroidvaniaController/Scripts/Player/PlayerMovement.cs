using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public Animator animator;
    public static bool paused = false;
    public GameObject pauseOverlay;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;
    bool releaseJump = false;
    bool dash = false;

    public bool dash_Unlocked = false;

    //bool dashAxis = false;

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 7"))
        {
            TogglePause();
        }

        if (!paused)
        {
            if (Input.GetAxisRaw("Horizontal") > 0.3 || Input.GetAxisRaw("Horizontal") < -0.3)
                horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            else
                horizontalMove = 0f;

            if (Player.controller.dead || Player.controller.resetting)
            {
                animator.SetBool("IsDead", true);
                horizontalMove = 0f;
            }

            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

            if (Input.GetButton("Jump"))
            {
                if (((Player.controller.m_IsFarWall || Player.controller.isWallSliding) && Player.controller.wallSlide_Unlocked) || Player.controller.beenOnLand >= 0.05f || Player.controller.lastOnLand < 0.15f || Player.controller.canDoubleJump)
                    jump = true;

                // fixes multi double jumping on rock platforms
                if (Player.controller.isJumpingDJ && !Player.controller.canDoubleJump)
                    jump = false;
                
                // fixes jumping beneath low ceilings
                if (Player.controller.m_Roofed)
                    jump = false;
            }

            if (Input.GetButton("Jump") && Player.controller.m_Grounded)
            {
                if (!Player.controller.m_Roofed && Player.controller.jumpCooldown <= 0f && !Player.controller.isJumping && !Player.controller.isJumpingDJ)
                    jump = true;

                //print("tries to reset point?");
                //reset_point.position = new Vector3(m_GroundCheck.position.x, m_GroundCheck.position.y, m_GroundCheck.position.z);
                // Player.controller.reset_point.position = Player.controller.m_GroundCheck.position;
            }

            if (Input.GetButtonUp("Jump"))
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
    }

    public void OnFall()
    {
        animator.SetBool("IsJumping", true);
    }

    public void OnLanding()
    {
        // this might have caused more sprite flickering
        // if (!Player.controller.isJumping && !Player.controller.isJumpingDJ) {
        //     animator.SetBool("IsJumping", false);
        // }
    }

    public void TogglePause() {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        CanvasToggle canv = FindObjectOfType<CanvasToggle>();

        if (!paused)
        {
            pauseOverlay.SetActive(true);
            paused = true;
            Time.timeScale = 0;
            Cursor.visible = true;
            canv.Pause();
            foreach (AudioSource source in sources)
            {
                source.Pause();
            }
        }
        else {
            pauseOverlay.SetActive(false);
            paused = false;
            Time.timeScale = 1;
            Cursor.visible = false;
            canv.UnPause();
            foreach (AudioSource source in sources)
            {
                source.UnPause();
            }
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

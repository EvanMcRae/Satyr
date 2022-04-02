using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupwalljump : MonoBehaviour
{
    bool playerIsInRange = false;
    string labelText = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.T) || Input.GetKeyDown("joystick button 1"))
        {
            if (playerIsInRange == true)
            {
                Player.controller.wallSlide_Unlocked = true;
                Destroy(this.gameObject);
            }
        }
    }

    public void OnGUI()
    {
        if (playerIsInRange == true)
        {
            GUI.Box(new Rect(140, Screen.height - 50, Screen.width - 300, 120), (labelText));
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsInRange = true;
            labelText = "Press T to pick up item";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerIsInRange = false;
    }
}

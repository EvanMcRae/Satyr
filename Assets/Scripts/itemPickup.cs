using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour
{
    public Item thisItem;
    Item item;
    bool playerIsInRange = false;
    string labelText = "";
    // Start is called before the first frame update
    void Start()
    {
        item = thisItem;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.T) || Input.GetKeyDown("joystick button 1"))
        {
            if (playerIsInRange == true)
            {
                playerInventory inventory = Player.instance.GetComponent<playerInventory>();

                if (item.itemID == "3")
                {
                    Player.controller.doubleJump_Unlocked = true;
                }
                else if (item.itemID == "5")
                {
                    Player.controller.wallSlide_Unlocked = true;
                }
                else if (item.itemID == "10")
                {
                    Player.controller.specialAttack_Unlocked = true;
                }
                else if (item.itemID == "4")
                {
                    Player.instance.GetComponent<PlayerMovement>().dash_Unlocked = true;
                }
                else if (item.itemID == "6")
                {
                    Player.instance.GetComponent<Attack>().shooting_Unlocked = true;
                }

                if (inventory.addToInventory(item))
                {
                    Destroy(this.gameObject);
                }
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

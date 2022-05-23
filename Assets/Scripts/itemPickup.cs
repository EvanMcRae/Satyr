using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour
{
    public Item thisItem;
    Item item;
    public bool playerIsInRange = false;
    string labelText = "";
    public GameObject trigger;
    public string method = "";

    public float speed;
    public float verticalSpeed;
    public float maxRotation;
    public float x = 0;
    public float y = 0;
    // Start is called before the first frame update
    void Start()
    {
        item = thisItem;
        if (item.itemID == "10" && Player.controller.reya) Destroy(gameObject);
        if (item.itemID == "11" && Player.controller.froggy) Destroy(gameObject);
        if (item.itemID == "7" && Player.instance.GetComponent<Attack>().shooting_Unlocked) Destroy(gameObject);
        if (item.itemID == "9" && Player.controller.wallSlide_Unlocked) Destroy(gameObject);
        if (item.itemID == "4" && Player.controller.doubleJump_Unlocked) Destroy(gameObject);
        if (item.itemID == "3" && Player.instance.GetComponent<PlayerMovement>().dash_Unlocked) Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerMovement.paused && Input.GetKey(KeyCode.T) || Input.GetKeyDown("joystick button 1"))
        {
            if (playerIsInRange == true)
            {
                playerInventory inventory = Player.instance.GetComponent<playerInventory>();

                if (item.itemID == "4")
                {
                    Player.controller.doubleJump_Unlocked = true;
                }
                else if (item.itemID == "9")
                {
                    Player.controller.wallSlide_Unlocked = true;
                }
                else if (item.itemID == "10")
                {
                    Player.controller.specialAttack_Unlocked = true;
                }
                else if (item.itemID == "3")
                {
                    Player.instance.GetComponent<PlayerMovement>().dash_Unlocked = true;
                }
                else if (item.itemID == "7")
                {
                    Player.instance.GetComponent<Attack>().shooting_Unlocked = true;
                    Player.instance.GetComponent<Attack>().canShoot = true;
                }
                else if (item.itemID == "11")
                {
                    Player.controller.froggy = true;
                }

                if (trigger != null)
                {
                    trigger.BroadcastMessage(method);
                }

                if (inventory.addToInventory(item))
                {
                    Destroy(this.gameObject);
                }
            }
        }

        transform.rotation = Quaternion.Euler(0f, 0f, maxRotation * Mathf.Sin(Time.time * speed));

        transform.position = new Vector3(x, (.3f * Mathf.Sin(Time.time * verticalSpeed)) + y, 0f);

    }

    public void OnGUI()
    {
        if (!PlayerMovement.paused && playerIsInRange)
        {
            GUI.Box(new Rect(140, Screen.height - 50, Screen.width - 300, 120), (labelText));
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsInRange = true;
            labelText = "Press B button or T key to pick up item";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerIsInRange = false;
    }

}

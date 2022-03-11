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
        if (Input.GetKey(KeyCode.W))
        {
            if (playerIsInRange == true)
            {
                //add item to players inventory
                print(item);
                playerInventory inventory = Player.instance.GetComponent<playerInventory>();
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
            labelText = "Press W to pick up item";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerIsInRange = false;
    }

}

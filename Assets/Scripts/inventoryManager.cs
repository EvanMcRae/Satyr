using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventoryManager : MonoBehaviour
{
    public Transform anchor;

    public Transform inventory;

    float x = 0f;

    public Item item;

    public Text itemText;

    public Image itemImage;

    public Image buttonImage;

    bool isDisplayed = false; // whether or not the inventory screen is up
    
    // Start is called before the first frame update
    void Start()
    {
        if (item == null)
        {
            gameObject.GetComponent<Image>().enabled = false;
        }

        if (item != null)
        {
            buttonImage.sprite = item.itemSprite;
            if (item.name == "DoubleJump")
            {
                //print("Double Jump is in the inventory");
                if (GetComponent<Player>().doubleJump_Unlocked != true)
                {
                    gameObject.GetComponent<Image>().enabled = false;
                }
            }
            else if (item.name == "Dash")
            {
                //print("Dash is in the inventory");
                if (GetComponent<PlayerMovement>().dash_Unlocked != true)
                {
                    gameObject.GetComponent<Image>().enabled = false;
                }

            }
            else if (item.name == "WallJump")
            {
                //print("WallJump is in the inventory");
                if (GetComponent<Player>().wallSlide_Unlocked != true)
                {
                    gameObject.GetComponent<Image>().enabled = false;
                }
            }

        }

        x = transform.position.x;
        inventory.position = new Vector3(inventory.position.x, 5000f, inventory.position.z);


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown("joystick button 7"))
        {
            isDisplayed = !isDisplayed;
            Cursor.visible = isDisplayed;
            inventory.position = new Vector3(inventory.position.x, isDisplayed ? anchor.position.y : 5000f, inventory.position.z);
            
            if (item == null)
            {
                // don't do anything lol
            }
            else if (item.name == "DoubleJump")
            {
                // print("Double Jump is in the inventory");
                if (GetComponent<Player>().doubleJump_Unlocked == true)
                {
                    gameObject.GetComponent<Image>().enabled = true;
                }
            }
            else if (item.name == "Dash")
            {
                // print("Dash is in the inventory");
                if (GetComponent<PlayerMovement>().dash_Unlocked == true)
                {
                    gameObject.GetComponent<Image>().enabled = true;
                }

            }
            else if (item.name == "WallJump")
            {
                // print("WallJump is in the inventory");
                if (GetComponent<Player>().wallSlide_Unlocked == true)
                {
                    gameObject.GetComponent<Image>().enabled = true;
                }
            }
            else
            {
                buttonImage.sprite = item.itemSprite;
                gameObject.GetComponent<Image>().enabled = true;
            }
        }
    }

    public void showItem()
    {
        if (item != null)
        {
            itemText.text = item.itemDescription;
            itemImage.sprite = item.itemSprite;
            Debug.Log("clicked the item");
            print("clicked the button");
        }
    }

}

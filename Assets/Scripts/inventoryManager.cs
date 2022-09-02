using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class inventoryManager : MonoBehaviour
{
    public Transform anchor;

    public Transform inventory;

    float x = 0f;

    public Item item;

    public Text itemText;

    public Image itemImage;

    public Image buttonImage;

    public bool isDisplayed = false; // whether or not the inventory screen is up
    
    // Start is called before the first frame update
    void Start()
    {
        if (item == null)
        {
            gameObject.GetComponentInChildren<Image>().enabled = false;
        }

        if (item != null)
        {
            gameObject.GetComponentInChildren<Image>().sprite = item.itemSprite;
        }

        x = transform.position.x;
        inventory.position = new Vector3(inventory.position.x, 5000f, inventory.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDisplayed && PlayerMovement.paused)
        {
            isDisplayed = false;
            inventory.position = new Vector3(inventory.position.x, isDisplayed ? anchor.position.y : 5000f, inventory.position.z);
        }

        if (!PlayerMovement.paused && Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown("joystick button 6"))
        {
            isDisplayed = !isDisplayed;
            Cursor.visible = isDisplayed;
            inventory.position = new Vector3(inventory.position.x, isDisplayed ? anchor.position.y : 5000f, inventory.position.z);
        }

        if (item != null)
        {
            GetComponent<Button>().enabled = false;
            buttonImage.sprite = item.itemSprite;
            transform.Find("Image").GetComponent<Image>().color = Color.white;
            GetComponent<Button>().enabled = true;
        }
        else
        {
            transform.Find("Image").GetComponent<Image>().color = Color.clear;
            GetComponent<Button>().enabled = false;
        }


        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            showItem();
        }
    }

    public void showItem()
    {
        if (item != null)
        {
            itemText.text = item.itemDescription;
            itemImage.sprite = item.itemSprite;
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryNavigator : MonoBehaviour
{
    public GameObject firstSlot;
    public static bool selected;

    // Start is called before the first frame update
    void Start()
    {
        firstSlot = GameObject.FindObjectOfType<GridLayoutGroup>().transform.Find("Button").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (firstSlot.GetComponent<inventoryManager>().isDisplayed && !selected)
        {
            EventSystem.current.SetSelectedGameObject(firstSlot);
            selected = true;
            GetComponent<Player>().canMove = false;
        }
        
        if (!firstSlot.GetComponent<inventoryManager>().isDisplayed && selected && !PlayerMovement.paused)
        {
            EventSystem.current.SetSelectedGameObject(null);
            selected = false;
            GetComponent<Player>().canMove = true;
        }

        if (selected && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(firstSlot);
        }
    }
}

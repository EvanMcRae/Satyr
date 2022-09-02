using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//keeps track of the player's inventory
//connected to DrawCharacter Variant
public class playerInventory : MonoBehaviour
{
    public List<Item> items_added = new List<Item>();
    public List<GameObject> inventory_spaces;
    public const int INVENTORY_SIZE = 15; // # of slots

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        updateInventory();
    }

    //adds an item to inventory
    //used by item pickup objects?
    public bool addToInventory(Item item)
    {
        // first tries to stack item in existing slot
        foreach (Item i in items_added)
        {
            if (i.itemID == item.itemID && i.itemCount <= i.maxStack)
            {
                i.itemCount++;
                return true;
            }
        }

        // then tries to add in new slot, assuming there is space
        if (items_added.Count < INVENTORY_SIZE)
        {
            items_added.Add(item);
            return true;
        }

        // if this doesn't work, notify user of failure to add to inventory
        print("inventory is too full!"); // temporary
        return false;
    }

    //updates the veiw of the inventory
    //used by 
    public void updateInventory()
    {
        GameObject[] slots = GameObject.FindGameObjectsWithTag("Slot");
        Array.Sort(slots, CompareObNames);
        inventory_spaces = new List<GameObject>(slots);
        

        // wipe previous items from canvas
        foreach (GameObject slot in inventory_spaces)
        {
            slot.GetComponent<inventoryManager>().item = null;
        }

        // display current items
        for (int index = 0; index < items_added.Count; index++)
        {
            if (index < inventory_spaces.Count) {
                inventoryManager im = inventory_spaces[index].GetComponent<inventoryManager>();
                im.item = items_added[index];
                im.buttonImage.sprite = im.item.itemSprite;
                inventory_spaces[index].transform.GetComponentInChildren<Image>().enabled = true;
            }
        }
    }

    int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void updateInventory()
    {
        inventory_spaces = new List<GameObject>(GameObject.FindGameObjectsWithTag("Slot"));

        // wipe previous items from canvas
        foreach (GameObject slot in inventory_spaces)
        {
            slot.GetComponent<inventoryManager>().item = null;
        }

        // display current items
        for (int index = 0; index < items_added.Count; index++)
        {
            inventoryManager im = inventory_spaces[index].GetComponent<inventoryManager>();
            im.item = items_added[index];
            im.buttonImage.sprite = im.item.itemSprite;
            inventory_spaces[index].GetComponent<Image>().enabled = true;
        }
    }
}

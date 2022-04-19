using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public ItemDatabase items;
    private static Database instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static Item GetItemByID(string ID)
    {
        foreach(Item item in instance.items.allItems)
        {
            if (item.itemID == ID)
                return item;
        }

        return null;
    }

}

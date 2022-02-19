using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Assets/Databases/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> allItems;
}

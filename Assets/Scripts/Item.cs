using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName = "Assets/Item")]
public class Item : ScriptableObject
{
    public string itemID;
    public string itemName;
    [TextArea]
    public string itemDescription;
    public Sprite itemSprite;
    public int itemCount = 0;
    public int maxStack = 64; // default, can change depending on item
}

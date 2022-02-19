using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemReference : MonoBehaviour
{
    public Item thisItem;
    public static Item item;
    public static Image buttonImage;
    public Image thisButton;
    // Start is called before the first frame update
    void Start()
    {
        item = thisItem;
        buttonImage = thisButton;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

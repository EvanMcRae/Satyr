using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cordyceps : MonoBehaviour
{
    public int count;
    // can be used to define bag thresholds
    public readonly int[] FILL_LEVELS = {0, 5, 10, 20, 40};
    public GameObject[] bagSprites;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        bagSprites = GameObject.FindGameObjectsWithTag("Bag");
        UpdateBag();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "CordycepsItem")
        {
            count++;
            Destroy(gameObject);
        }
    }

    public void UpdateBag()
    {
        foreach (GameObject sprite in bagSprites)
        {
            sprite.GetComponent<Image>().enabled = false;
        }
        for (int i = 4; i >= 0; i--)
        {
            if (count >= FILL_LEVELS[i])
            {
                bagSprites[i].GetComponent<Image>().enabled = true;
                return;
            }
        }
    }
}

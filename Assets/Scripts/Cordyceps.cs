using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cordyceps : MonoBehaviour
{
    public int count = 0;
    // can be used to define bag thresholds
    public readonly int[] FILL_LEVELS = {0, 5, 10, 20, 40};
    public GameObject[] bagSprites;
    public AudioClip harvestCordyceps;
    public bool animating;
    private GameObject currentSprite;

    // Update is called once per frame
    void FixedUpdate()
    {
        bagSprites = GameObject.FindGameObjectsWithTag("Bag");
        Array.Sort(bagSprites, CompareObNames);
        UpdateBag();

        GameObject[] items = GameObject.FindGameObjectsWithTag("CordycepsItem");
        foreach (GameObject item in items)
        {
            if ((item.transform.position - transform.position).magnitude < 0.5f) 
            {
                Destroy(item);
                count++;
                UpdateBag();

                // squishy animation
                if (!animating)
                    StartCoroutine(BagAnimation());

                // play sound
                Player.controller.PlaySound(harvestCordyceps);
            }
        }
    }

    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.tag == "CordycepsItem")
    //     {
    //         count++;
    //         Destroy(gameObject);
    //     }
    // }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "CordycepsItem")
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
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
                if (bagSprites.Length > i)
                {
                    bagSprites[i].GetComponent<Image>().enabled = true;
                    currentSprite = bagSprites[i];
                }
                return;
            }
        }
    }

    IEnumerator BagAnimation() {
        animating = true;
        currentSprite.GetComponent<Animator>().SetTrigger("start");
        yield return new WaitForSeconds(0.25f);
        animating = false;
    }

    int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }

}

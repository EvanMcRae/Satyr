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
    public AudioClip harvestCordyceps;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        bagSprites = GameObject.FindGameObjectsWithTag("Bag");
        
        GameObject[] items = GameObject.FindGameObjectsWithTag("CordycepsItem");
        if (items.Length == 0)
        {
            UpdateBag();
            return;
        }
        
        foreach (GameObject item in items)
        {
            if ((item.transform.position - transform.position).magnitude < 0.5f) 
            {
                Destroy(item);
                count++;
                UpdateBag();

                // squishy animation
                foreach (GameObject sprite in bagSprites)
                {
                    if (sprite.GetComponent<Image>().enabled)
                    {
                        sprite.GetComponent<Animator>().SetTrigger("start");
                    }
                }

                // play sound
                AudioSource[] audioSource = transform.GetComponents<AudioSource>();
                foreach (AudioSource source in audioSource)
                {
                    if (source.clip == harvestCordyceps && source.isPlaying)
                    {
                        if (source.time < 0.2f) return;
                        else source.Stop();
                    }
                }
                foreach (AudioSource source in audioSource)
                {
                    if (!source.isPlaying)
                    {
                        source.clip = harvestCordyceps;
                        source.loop = false;
                        source.Play();
                        return;
                    }
                }
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
                    bagSprites[i].GetComponent<Image>().enabled = true;
                return;
            }
        }
    }
}

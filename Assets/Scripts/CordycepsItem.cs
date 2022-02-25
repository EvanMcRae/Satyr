using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CordycepsItem : MonoBehaviour
{
    bool playerIsInRange = false;
    private Transform playerPos;
    public Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0,9)];
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        if (playerIsInRange == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerPos.position, 0.03f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerIsInRange = false;
    }

}

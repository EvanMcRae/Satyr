using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crusher : MonoBehaviour
{

    public BoxCollider2D hitBox;
    public SpriteRenderer sr;
    private int timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += 1;
        if(timer > 200 && hitBox.enabled == true)
        {
            timer = 0;
            hitBox.enabled = false;
            sr.enabled = false;
        }
        else if(timer > 200 && hitBox.enabled == false)
        {
            timer = 0;
            hitBox.enabled = true;
            sr.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().ApplyDamage(1.0f, this.transform.position, 30f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().ApplyDamage(1.0f, this.transform.position, 30f);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CordycepsItem : MonoBehaviour
{
    bool playerIsInRange = false;
    private Transform playerPos;
    public Sprite[] sprites;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0,9)];
        
        var bounds = GetComponent<SpriteRenderer>().sprite.bounds;
        var factor = 0.75f / bounds.size.y;
        transform.localScale = new Vector3(factor, factor, factor);

        boxCollider = GetComponent<BoxCollider2D>();
        var S = bounds.size;
        boxCollider.size = S;
        boxCollider.offset = new Vector2((S.x / 2), 0);

        rb.velocity = Random.insideUnitCircle * 5f;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = Player.instance.transform;
        float distance = (transform.position - playerPos.position).magnitude;
        playerIsInRange = distance < 6.4f;
        if (playerIsInRange)
        {
            rb.AddForce((playerPos.transform.position - transform.position).normalized * 750f * Time.smoothDeltaTime);
            transform.position = Vector2.MoveTowards(transform.position, playerPos.position, 0.03f / distance);
        }
    }

    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.tag == "Player")
    //     {
    //         playerIsInRange = true;
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D collision)
    // {
    //     playerIsInRange = false;
    // }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

}

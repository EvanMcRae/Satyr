using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowMushroom : Enemy
{
    public Animator mushAnim;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = Player.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (target.position.x > this.transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (target.position.x < this.transform.position.x && facingRight)
        {
            Flip();
        }*/
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            print("player entered radius");
            mushAnim.SetBool("isAttacking", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            mushAnim.SetBool("isAttacking", false);
            print("player exited");
        };
    }

}

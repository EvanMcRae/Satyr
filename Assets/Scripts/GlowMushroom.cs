using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowMushroom : Enemy
{
    public Animator mushAnim;
    public Transform target;

    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;

    public int dropsCordycep = 2;
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

        if (life <= 0)
        {
            StartCoroutine(DestroyEnemy());
        }

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

    public void Attack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * -attackOffset.x;
        pos += transform.up * attackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            if (colInfo.tag == "Player")
            {
                colInfo.GetComponent<Player>().ApplyDamage(1.0f, this.transform.position, 30f);
            }
        }
    }

    public override void ApplyDamage(float damage, float knockback = 1.0f)
    {
        GetComponent<SimpleFlash>().Flash(0.4f, 1, true);
        float direction = damage / Mathf.Abs(damage);
        damage = Mathf.Abs(damage);
        // transform.GetComponent<Animator>().SetBool("Hit", true);
        life -= damage;
        if (life < 0) life = 0;
        //rb.velocity = Vector2.zero;
        //rb.AddForce(new Vector2(direction * (1500f + (speed * 800)), 300f) * knockback);
        //StartCoroutine(HitTime());

    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(0.25f);
        for (int i = 0; i < dropsCordycep; i++)
        {
            Instantiate(cordyceps, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

}

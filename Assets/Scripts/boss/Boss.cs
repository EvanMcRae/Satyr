using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy
{
    public float maxLife;
    Transform player;
    public Image bossBarImage;

    public bool isFlipped = false;

    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;
    private Rigidbody2D rb;
    public bool isHitted = false;
    public bool dead = false;

    //
    [SerializeField] float jumpHeight;
    [SerializeField] Transform groundCheck;
    [SerializeField] Vector2 boxSize;
    [SerializeField] LayerMask groundLayer;
    private bool isGrounded;

    private bool shouldJump = true;
    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance.transform;
        rb = GetComponent<Rigidbody2D>();
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        bossBarImage.fillAmount = Mathf.Lerp(bossBarImage.fillAmount, (float)life / maxLife, 0.1f);

        // ignore collision with player depending on several factors
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Player.instance.GetComponent<Collider2D>(), dead || Player.controller.invincible || Player.controller.isDashing || Statue.cutscening || Player.controller.resetting || Player.controller.dead || !Player.instance.GetComponent<Attack>().canAttack);

        if (life <= 0)
        {
            // print("enemy died");
            isHitted = true;
            if (!dead)
                StartCoroutine(DestroyEnemy());
        }

        //isGrounded = Physics2D.OverlapBox(groundCheck.position, boxSize, 0, groundLayer);

        /*if (Input.GetKeyDown(KeyCode.N))
        {
            JumpAttack();
        }*/

        if (shouldJump == true && player.transform.position.y < 80)
        {
            shouldJump = false;
            StartCoroutine(Jump());
        }

    }

    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if(transform.position.x < player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if(transform.position.x > player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    void JumpAttack()
    {
        float distanceFromPlayer = player.position.x - transform.position.x;

        if (isGrounded)
        {
            rb.AddForce(new Vector2(distanceFromPlayer, jumpHeight), ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    public void Attack()
    {
       // print("boss attacks");
        Vector3 pos = transform.position;
        //pos += transform.right * attackOffset.x;
        pos += transform.right * -attackOffset.x;
        pos += transform.up * attackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            if(colInfo.tag == "Player")
            {
                colInfo.GetComponent<Player>().ApplyDamage(1.0f, this.transform.position, 30f);
            }
        }
    }

    public override void ApplyDamage(float damage, float knockback = 1.0f)
    {
        isHitted = true;
        // MethodBase methodBase = MethodBase.GetCurrentMethod();
        // Debug.Log(methodBase.Name);
        float direction = damage / Mathf.Abs(damage);
        damage = Mathf.Abs(damage);
        // transform.GetComponent<Animator>().SetBool("Hit", true);
        life -= damage;
        if (life < 0) life = 0;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(direction * 1000f, 200f) * knockback);
        StartCoroutine(HitTime());
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
        else if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<Player>().ApplyDamage(1.0f, this.transform.position, 60f);
        }
    }

    IEnumerator HitTime()
    {
        GetComponent<SimpleFlash>().Flash(0.4f, 1, true);
        isHitted = true;
        //   isInvincible = true;
        yield return new WaitForSeconds(.2f);
        isHitted = false;
        // isInvincible = false;
    }

    IEnumerator Jump()
    {
        yield return new WaitForSeconds(2f);
        JumpAttack();
        yield return new WaitForSeconds(3f);
        shouldJump = true;
    }

    IEnumerator DestroyEnemy()
    {
        dead = true;
        //CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
        //  capsule.size = new Vector2(1f, 0.25f);
        //   capsule.offset = new Vector2(0f, -0.8f);
        //capsule.direction = CapsuleDirection2D.Horizontal;
        yield return new WaitForSeconds(0.25f);
        rb.velocity = new Vector2(0, rb.velocity.y);
        // yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

}

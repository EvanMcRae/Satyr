using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class patrol : Enemy
{
    public float distance;
    public float distance2;
    private bool movingRight = true;
    public Transform groundDetection;

    public Transform wallDetection;

    private Rigidbody2D rb;
    
    private bool isHitted = false;

    bool frozen = false;
    bool dead = false;


  // bool rayCast_hits_player = false;

    bool isHunterMode = false;

    public Transform target;

    public bool justPatrols = false;
    public int dropsCordycep = 2;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    //update calls every frame?
    void Update()
    {
        target = Player.instance.transform;

        // ignore collision with player depending on several factors
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Player.instance.GetComponent<Collider2D>(), dead || Player.controller.invincible || Player.controller.isDashing || Statue.cutscening || Player.controller.resetting || Player.controller.dead || !Player.instance.GetComponent<Attack>().canAttack);

        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     print(isHunterMode);
        
        // }

        if (!frozen)
        {
            if (!isHitted && isHunterMode == false)
            {
                // print("prtol code is runing");
                transform.Translate(Vector2.right * speed * Time.deltaTime);

                RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance);

                /*
                 for detecting walls and turning around using ray casts, works
                without errors, but for some reason casuses tons of Null refrence
                RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, transform.right, distance2);


                if(wallInfo.collider.gameObject.tag == "Player")
                {
                    rayCast_hits_player = true;
                }
                else
                {
                    rayCast_hits_player = false;
                }  
                || (wallInfo == true && rayCast_hits_player == false)
                 */


                if (((groundInfo.collider == false) || groundInfo.collider.tag == "obstacle") && !isHunterMode)
                {

                    if (movingRight == true)
                    {
                        transform.eulerAngles = new Vector3(0, -180, 0);
                        movingRight = false;
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        movingRight = true;
                    }
                }


            }
            else if (isHunterMode == true && isHitted == false)
            {
                if (target.position.x > this.transform.position.x)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    transform.Translate(Vector2.right * speed * Time.deltaTime);
                    movingRight = true;
                }
                else if (target.position.x < this.transform.position.x)
                {
                    transform.eulerAngles = new Vector3(0, -180, 0);
                    transform.Translate(Vector2.right * speed * Time.deltaTime);
                    movingRight = false;
                }
            }
        }
        
        if(life <= 0)
        {
           // print("enemy died");
            isHitted = true;
            if (!dead)
                StartCoroutine(DestroyEnemy());

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(justPatrols == false)
        {
            if (collision.gameObject.tag == "Player")
            {
                isHunterMode = true;
                //print(isHunterMode);
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
            if (collision.gameObject.tag == "Player")
            {
                isHunterMode = false;
                // print(isHunterMode);
            }
        
        
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        //  Debug.Log("OnCollisionEnter2D");
      //  print(col.gameObject.tag);
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<Player>().ApplyDamage(1.0f, this.transform.position,60f);
            StartCoroutine(Freeze());
        }
        else if ((col.gameObject.tag == "Wall" || col.gameObject.tag == "Breakable Wall") && isHunterMode == false)
        {
            //print("prtol collison with ground is working");
            if (movingRight == true)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }
            else if(movingRight == false)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // if(isHunterMode == false)
        // {
        //     if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Breakable Wall")
        //     {
        //         if (movingRight == true)
        //         {
        //             transform.eulerAngles = new Vector3(0, -180, 0);
        //             movingRight = false;
        //         }
        //         else if (movingRight == false)
        //         {
        //             transform.eulerAngles = new Vector3(0, 0, 0);
        //             movingRight = true;
        //         }
        //     }
        // }
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().ApplyDamage(1.0f, this.transform.position, 60f);
            StartCoroutine(Freeze());
        }
       
        
    }

    public override void ApplyDamage(float damage, float knockback = 1.0f)
    {
       // MethodBase methodBase = MethodBase.GetCurrentMethod();
       // Debug.Log(methodBase.Name);
        float direction = damage / Mathf.Abs(damage);
            damage = Mathf.Abs(damage);
            // transform.GetComponent<Animator>().SetBool("Hit", true);
            life -= damage;
            if (life < 0) life = 0;
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(direction * 1000f, 200f)*knockback);
            StartCoroutine(HitTime());
        
    }

    IEnumerator Freeze()
    {
        frozen = true;
        yield return new WaitForSeconds(1f);
        frozen = false;
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

    IEnumerator DestroyEnemy()
    {
        dead = true;
        //CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
      //  capsule.size = new Vector2(1f, 0.25f);
     //   capsule.offset = new Vector2(0f, -0.8f);
        //capsule.direction = CapsuleDirection2D.Horizontal;
        yield return new WaitForSeconds(0.25f);
        rb.velocity = new Vector2(0, rb.velocity.y);
        //  yield return new WaitForSeconds(3f);
        for (int i = 0; i < dropsCordycep; i++)
        {
            Instantiate(cordyceps, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

}

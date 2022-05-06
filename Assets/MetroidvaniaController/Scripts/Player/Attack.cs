using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float dmgValue = 4;
    public GameObject throwableObject;
    public Transform attackCheck;
    public Transform botAttackCheck;
    public Transform topAttackCheck;
    public Transform currentAttackCheck;
    public Transform wallCheck;
    private Rigidbody2D m_Rigidbody2D;
    public Animator animator;
    public bool canAttack = true;
    public bool canShoot = true;
    private bool shotDuringPause = false;
    public bool isTimeToCheck = false;
    public ParticleSystem particleAttack;
    public ParticleSystem particleSpecialAttack;

    Vector2 startPoint, endPoint; // for arrow trajectory calculation

    public List<Collider2D> ignoredEnemies = new List<Collider2D>();

    public GameObject cam;

    public bool shooting_Unlocked = false;
    public float shootStrength = 0.0f;
    public float verticalAim = 0.25f;

    //public SpecialBar specialBar;
    public float specialCooldown = 0.0f;
    public float specialMaxCooldown = 10.0f;
    public health playerHealth;
    public Cordyceps cordyceps;
    private int countToHeal = 5;
    public AudioClip swordClash, specialSound, arrowRelease;


    private void Awake()
    {
        specialCooldown = specialMaxCooldown;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentAttackCheck = attackCheck;
        particleAttack.transform.position = currentAttackCheck.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerMovement.paused)
        {
            cam = GameObject.Find("Main Camera pre Variant");

            if (specialCooldown < specialMaxCooldown)
                specialCooldown += Time.deltaTime;

            if (Input.GetButton("Attack") && canAttack && shootStrength <= 0.0f)
            {
                currentAttackCheck = attackCheck;
                if (Input.GetAxisRaw("Vertical") < -0.3)
                {
                    currentAttackCheck = botAttackCheck;
                }
                if (Input.GetAxisRaw("Vertical") > 0.3)
                {
                    currentAttackCheck = topAttackCheck;
                }
                particleAttack.transform.position = currentAttackCheck.position;
                canAttack = false;
                animator.SetBool("IsAttacking", true);
                StartCoroutine(AttackCooldown());
            }

            if (!Player.controller.m_IsWall && (Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown(1) || Input.GetKeyDown("joystick button 4")) && canShoot && shooting_Unlocked)
            {
                Player.controller.canMove = false;
                var velocity = GetComponent<Rigidbody2D>().velocity;
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, velocity.y);
                animator.SetBool("IsBowAttacking", true);
            }

            if (!Player.controller.m_IsWall && (Input.GetKey(KeyCode.K) || Input.GetMouseButton(1) || Input.GetKey("joystick button 4")) && canShoot && shooting_Unlocked)
            {
                if (!animator.GetBool("IsBowAttacking"))
                {
                    animator.SetBool("IsBowAttacking", true);
                }
                Player.controller.canMove = false;
                var velocity = GetComponent<Rigidbody2D>().velocity;
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, velocity.y);
                if (shootStrength <= 1.0f)
                {
                    shootStrength += Time.deltaTime;
                }

                // camera looks forward when player is aiming
                Vector3 newLocalPos = Player.controller.camTarget.localPosition;
                newLocalPos.x = Mathf.Lerp(newLocalPos.x, 4.0f, 0.005f);
                Player.controller.camTarget.localPosition = newLocalPos;

                if (Input.GetAxisRaw("Vertical") > 0.5 && verticalAim < 2.0f)
                    verticalAim += Time.deltaTime;

                if (Input.GetAxisRaw("Vertical") < -0.5 && verticalAim > -2.0f)
                    verticalAim -= Time.deltaTime;

                Vector2 direction = new Vector2(transform.localScale.x, verticalAim);
                float speed = 20f * (shootStrength + 0.1f);
                Vector2 force = 2.4f * direction * speed;
                if (shootStrength >= 0.25f)
                {
                    GetComponentInChildren<Trajectory>().Show();
                    GetComponentInChildren<Trajectory>().UpdateDots(force);
                }

                GetComponent<CapsuleCollider2D>().sharedMaterial = Player.controller.friction;
            }
            else
            {
                // camera returns to normal position
                Vector3 newLocalPos = Player.controller.camTarget.localPosition;
                newLocalPos.x = Mathf.Lerp(newLocalPos.x, 0f, 0.1f);
                Player.controller.camTarget.localPosition = newLocalPos;
            }

            if (((Input.GetKeyUp(KeyCode.K) || Input.GetMouseButtonUp(1) || Input.GetKeyUp("joystick button 4")) && canShoot && shooting_Unlocked) || shotDuringPause)
            {
                if (shootStrength >= 0.25f)
                {
                    canShoot = false;
                    GameObject throwableWeapon = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 0.5f, 0.2f), Quaternion.identity) as GameObject;
                    Vector2 direction = new Vector2(transform.localScale.x, verticalAim);
                    float speed = 20f * (shootStrength + 0.1f);

                    throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction;
                    throwableWeapon.GetComponent<ThrowableWeapon>().speed = speed;

                    if (direction.x < 0)
                    {
                        throwableWeapon.GetComponent<SpriteRenderer>().flipX = true;
                        var x = throwableWeapon.GetComponent<BoxCollider2D>().offset.x;
                        var y = throwableWeapon.GetComponent<BoxCollider2D>().offset.y;
                        throwableWeapon.GetComponent<BoxCollider2D>().offset = new Vector2(-x, y);
                    }

                    throwableWeapon.name = "ThrowableWeapon";
                    Player.controller.PlaySound(arrowRelease);
                }
                shootStrength = 0.0f;
                verticalAim = 0.25f;
                shotDuringPause = false;
                StartCoroutine(ShootCooldown());
                animator.SetBool("IsBowAttacking", false);
                animator.SetBool("BowReleased", true);
                GetComponentInChildren<Trajectory>().Hide();
            }

            if (Player.controller.specialAttack_Unlocked && !ReyaCutscene.cutscening && !Statue.cutscening && specialCooldown >= specialMaxCooldown && Input.GetButton("Special") && shootStrength <= 0.0f)
            {
                particleSpecialAttack.Play();
                specialCooldown = 0.0f;
                animator.SetBool("IsSattacking", true);
                cam.GetComponent<CameraFollow>().ShakeCamera(0.2f);
                gameObject.GetComponent<Player>().Invincible(1f);

                Player.controller.PlaySound(specialSound);
            }
            // else if (Input.GetKeyUp(KeyCode.Y) || Input.GetKeyUp("joystick button 3"))
            // {
            // 	special_attack_hitbox.enabled = false;
            // }

            if ((Input.GetKeyUp(KeyCode.H) || Input.GetKeyUp("joystick button 5") || Input.GetKeyUp(KeyCode.V)) && cordyceps.count >= countToHeal && playerHealth.playerHealth < playerHealth.numberOfHearts)
            {
                playerHealth.playerHealth += 1;
                cordyceps.count -= 5;
            }
        }
        else
        {
            if ((Input.GetKeyUp(KeyCode.K) || Input.GetMouseButtonUp(1) || Input.GetKeyUp("joystick button 4")) && canShoot && shooting_Unlocked)
            {
                shotDuringPause = true;   
            }
            if ((Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown(1) || Input.GetKeyDown("joystick button 4")) && canShoot && shooting_Unlocked)
            {
                shotDuringPause = false;
            }
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        ignoredEnemies.Clear();
        canAttack = true;
    }

    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        animator.SetBool("IsBowAttacking", false);
        animator.SetBool("BowReleased", false);
        Player.controller.canMove = true;
        canShoot = true;
    }

    public void DoDashDamage(float knockback = 1.0f)
    {
        dmgValue = Mathf.Abs(dmgValue);
        Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(currentAttackCheck.position, 1.4f);

        particleAttack.Play();
        for (int i = 0; i < collidersEnemies.Length; i++)
        {   
            if (collidersEnemies[i].gameObject.tag == "Enemy" && !(ignoredEnemies.Contains(collidersEnemies[i])))
            {
                if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
                {
                    dmgValue = -dmgValue;
                }

                if (collidersEnemies[i].GetComponent<Enemy>() != null)
                {
                    collidersEnemies[i].GetComponent<Enemy>().ApplyDamage(dmgValue, knockback);
                }

                //collidersEnemies[i].gameObject.SendMessage("ApplyDamage", dmgValue);
                // this code is for camera shake on attack?
                //cam.GetComponent<CameraFollow>().ShakeCamera();


                Vector2 damageDir = Vector3.Normalize(transform.position - collidersEnemies[i].transform.position) * 85f;
                m_Rigidbody2D.velocity = Vector2.zero;
                int direction = 0;
                if (collidersEnemies[i].transform.position.x > transform.position.x) { direction = -1; } else { direction = 1; }
                m_Rigidbody2D.AddForce(new Vector2(direction * 2000f, 200f));
                //m_Rigidbody2D.AddForce(damageDir * 10);
            }
            else if(collidersEnemies[i].gameObject.tag == "Breakable Wall" && !(ignoredEnemies.Contains(collidersEnemies[i])))
            {
                if (collidersEnemies[i].GetComponent<breakableWall>() != null)
                {
                    collidersEnemies[i].GetComponent<breakableWall>().ApplyDamage(dmgValue);
                }

                cam.GetComponent<CameraFollow>().ShakeCamera(0.2f);
                Vector2 damageDir = Vector3.Normalize(transform.position - collidersEnemies[i].transform.position) * 85f;
                m_Rigidbody2D.velocity = Vector2.zero;
                m_Rigidbody2D.AddForce(damageDir * 10);
            }
            ignoredEnemies.Add(collidersEnemies[i]);
        }

        Collider2D[] collidersWalls = Physics2D.OverlapCircleAll(wallCheck.position, 0.6f);
        for (int i = 0; i < collidersWalls.Length; i++)
        {
            if (collidersWalls[i].gameObject.tag == "Wall" || collidersWalls[i].gameObject.tag == "Ground" || collidersWalls[i].gameObject.tag == "GroundNoSlide")
            {
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
                int direction = 0;
                if (collidersWalls[i] is EdgeCollider2D)
                {
                    continue;
                }
                if (collidersWalls[i].transform.position.x > transform.position.x) { direction = -1; } else { direction = 1; }
                m_Rigidbody2D.AddForce(new Vector2(direction * 1000f, 0f));
                Player.controller.PlaySound(swordClash);
            }
        }
    }
}

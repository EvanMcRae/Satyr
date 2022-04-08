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
	public bool isTimeToCheck = false;
	public ParticleSystem particleAttack;
    public ParticleSystem particleSpecialAttack;

	public List<Collider2D> ignoredEnemies = new List<Collider2D>();

	public GameObject cam;

	public CircleCollider2D special_attack_hitbox;

	public bool shooting_Unlocked = false;

	//public SpecialBar specialBar;
    public float specialCooldown = 0.0f;
	public float specialMaxCooldown = 10.0f;
	public health playerHealth;
	public Cordyceps cordyceps;
	private int countToHeal = 5;
    public AudioClip swordClash, specialSound;


	private void Awake()
	{
        specialCooldown = specialMaxCooldown;
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
	}

	// Start is called before the first frame update
	void Start()
    {
		special_attack_hitbox.enabled = false;
        currentAttackCheck = attackCheck;
        particleAttack.transform.position = currentAttackCheck.position;
    }

    // Update is called once per frame
    void Update()
    {
        cam = GameObject.Find("Main Camera pre Variant");

        if (specialCooldown < specialMaxCooldown)
            specialCooldown += Time.deltaTime;/////////////////////////////
			//specialBarFill = ((3.0f - specialCooldown)/3.0f) * 100;
			//specialBar.UpdateBar();

		if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetMouseButtonDown(0) || Input.GetKeyDown("joystick button 2")) && canAttack)
		{
			currentAttackCheck = attackCheck;
			if (Input.GetAxisRaw("Vertical") < -0.3) {
				currentAttackCheck = botAttackCheck;
			}
			if (Input.GetAxisRaw("Vertical") > 0.3) {
				currentAttackCheck = topAttackCheck;
			}
			particleAttack.transform.position = currentAttackCheck.position;
			canAttack = false;
			animator.SetBool("IsAttacking", true);
			StartCoroutine(AttackCooldown());
		}

		if ((Input.GetKeyDown(KeyCode.V) || Input.GetMouseButtonDown(1)) && canShoot && shooting_Unlocked == true)
		{
			canShoot = false;
			GameObject throwableWeapon = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 0.5f,-0.2f), Quaternion.identity) as GameObject; 
			Vector2 direction = new Vector2(transform.localScale.x, 0);
			throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction;
			print(direction);
			if(direction.x < 0)
            {
				throwableWeapon.GetComponent<SpriteRenderer>().flipX = true;
            }
			throwableWeapon.name = "ThrowableWeapon";
			StartCoroutine(ShootCooldown());
		}

        if (specialCooldown >= specialMaxCooldown && (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown("joystick button 3")) && !special_attack_hitbox.enabled)
        {
			special_attack_hitbox.enabled = true;
            particleSpecialAttack.Play();
            specialCooldown = 0.0f;
			animator.SetBool("IsSattacking", true);
			cam.GetComponent<CameraFollow>().ShakeCamera(0.2f);
            gameObject.GetComponent<Player>().Invincible(1f);

            AudioSource[] audioSource = transform.GetComponents<AudioSource>();
            foreach (AudioSource source in audioSource)
            {
                if (source.clip == swordClash && source.isPlaying)
                {
                    if (source.time < 0.2f) return;
                    else source.Stop();
                }
            }
            foreach (AudioSource source in audioSource)
            {
                if (!source.isPlaying)
                {
                    source.clip = specialSound;
                    source.loop = false;
                    source.Play();
                }
            }
		}
		else if (Input.GetKeyUp(KeyCode.Y) || Input.GetKeyUp("joystick button 3"))
        {
			special_attack_hitbox.enabled = false;
		}
		
		if (Input.GetKeyUp(KeyCode.H) && cordyceps.count >= countToHeal && playerHealth.playerHealth < playerHealth.numberOfHearts)
        {
			playerHealth.playerHealth += 1;
			cordyceps.count -= 5;
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
		yield return new WaitForSeconds(1f);
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
			if (collidersWalls[i].gameObject.tag == "Wall" || collidersWalls[i].gameObject.tag == "Ground")
			{
				m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
				int direction = 0;
                if (collidersWalls[i] is EdgeCollider2D)
                {
                    continue;
                }
				if (collidersWalls[i].transform.position.x > transform.position.x) { direction = -1; } else { direction = 1; }
				m_Rigidbody2D.AddForce(new Vector2(direction * 1000f, 0f));
                AudioSource[] audioSource = transform.GetComponents<AudioSource>();
                foreach (AudioSource source in audioSource)
                {
                    if (source.clip == swordClash && source.isPlaying)
                    {
                        if (source.time < 0.2f) return;
                        else source.Stop();
                    } 
                }
                foreach (AudioSource source in audioSource)
                {
                    if (!source.isPlaying)
                    {
                        // Debug.Log(playingClash);
                        source.clip = swordClash;
                        source.loop = false;
                        source.Play();
                    }
                }
			}
		}
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	public float dmgValue = 4;
	public GameObject throwableObject;
	public Transform attackCheck;
	public Transform wallCheck;
	private Rigidbody2D m_Rigidbody2D;
	public Animator animator;
	public bool canAttack = true;
	public bool canShoot = true;
	public bool isTimeToCheck = false;

	public GameObject cam;

	public BoxCollider2D special_attack_hitbox;

	public bool shooting_Unlocked = false;
    public float specialCooldown = 0.0f;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
	}

	// Start is called before the first frame update
	void Start()
    {
		special_attack_hitbox.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        cam = GameObject.Find("actual camera");

        if (specialCooldown < 3.0f)
            specialCooldown += Time.deltaTime;

		if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetMouseButtonDown(0) || Input.GetKeyDown("joystick button 2")) && canAttack)
		{
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

        if (specialCooldown > 3.0f && Input.GetKeyDown(KeyCode.Y) && !special_attack_hitbox.enabled)
        {
			special_attack_hitbox.enabled = true;
            specialCooldown = 0.0f;
			cam.GetComponentInChildren<CameraFollow>().ShakeCamera();
		}
		else if (Input.GetKeyUp(KeyCode.Y))
        {
			special_attack_hitbox.enabled = false;
		}
	}

	IEnumerator AttackCooldown()
	{
		yield return new WaitForSeconds(0.25f);
		canAttack = true;
	}

	IEnumerator ShootCooldown()
    {
		yield return new WaitForSeconds(1f);
		canShoot = true;
    }

	public void DoDashDamage()
	{
		dmgValue = Mathf.Abs(dmgValue);
		Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, 0.9f);
		for (int i = 0; i < collidersEnemies.Length; i++)
		{
			if (collidersEnemies[i].gameObject.tag == "Enemy")
			{
				if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
				{
					dmgValue = -dmgValue;
				}

				if (collidersEnemies[i].GetComponent<Enemy>() != null)
				{
					collidersEnemies[i].GetComponent<Enemy>().ApplyDamage(dmgValue);
				}

                //collidersEnemies[i].gameObject.SendMessage("ApplyDamage", dmgValue);
                // this code is for camera shake on attack?
                //cam.GetComponentInChildren<CameraFollow>().ShakeCamera();


                Vector2 damageDir = Vector3.Normalize(transform.position - collidersEnemies[i].transform.position) * 85f;
				m_Rigidbody2D.velocity = Vector2.zero;
				int direction = 0;
				if (collidersEnemies[i].transform.position.x > transform.position.x) { direction = -1; } else { direction = 1; }
				m_Rigidbody2D.AddForce(new Vector2(direction * 2000f, 200f));
				//m_Rigidbody2D.AddForce(damageDir * 10);
			}
			else if(collidersEnemies[i].gameObject.tag == "Breakable Wall")
            {
				if (collidersEnemies[i].GetComponent<breakableWall>() != null)
				{
					collidersEnemies[i].GetComponent<breakableWall>().ApplyDamage(dmgValue);
				}

				cam.GetComponentInChildren<CameraFollow>().ShakeCamera();
				Vector2 damageDir = Vector3.Normalize(transform.position - collidersEnemies[i].transform.position) * 85f;
				m_Rigidbody2D.velocity = Vector2.zero;
				m_Rigidbody2D.AddForce(damageDir * 10);
			}
			
		}


		Collider2D[] collidersWalls = Physics2D.OverlapCircleAll(wallCheck.position, 0.6f);
		for (int i = 0; i < collidersWalls.Length; i++)
		{
            
				if (collidersWalls[i].gameObject.tag == "Wall" || collidersWalls[i].gameObject.tag == "Ground")
				{
				m_Rigidbody2D.velocity = Vector2.zero;
				int direction = 0;
				if (collidersWalls[i].transform.position.x > transform.position.x) { direction = -1; } else { direction = 1; }
				m_Rigidbody2D.AddForce(new Vector2(direction * 1000f, 200f));
			}
			
			
		}
		

	}
}

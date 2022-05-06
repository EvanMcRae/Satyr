﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableWeapon : MonoBehaviour
{
    private Rigidbody2D rb;
	public Vector2 direction;
	public bool hasHit = false;
	public float speed = 10f;
    public float rotation = 0f;
    public AudioSource launchSound, hitSound;
    private float hitTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = 2.4f * direction * speed;
        rb.SetRotation(Quaternion.LookRotation(rb.velocity));
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), Player.instance.GetComponent<Collider2D>(), true);
        if (rb.velocity != Vector2.zero)
            rb.SetRotation(Quaternion.LookRotation(rb.velocity));

        if (hasHit)
        {
            hitTime += Time.fixedDeltaTime;
            if (hitTime >= 0.5f)
            {
                var color = GetComponent<SpriteRenderer>().color;
                GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, Mathf.Lerp(1.0f, 0.0f, (hitTime - 0.5f)*2f));
            }
            
        }
    }

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Enemy")
		{
            
            hasHit = true;
			collision.gameObject.GetComponent<Enemy>().ApplyDamage(Mathf.Sign(direction.x) * 2f, 1f);
            Destroy(gameObject);
		}
		else if (collision.gameObject.tag != "Player")
		{
            launchSound.Stop();
            hitSound.Play();
            hasHit = true;
			StartCoroutine(KillArrow());
		}
	}

    IEnumerator KillArrow() {
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponent<Rigidbody2D>().simulated = false;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}

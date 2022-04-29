using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableWeapon : MonoBehaviour
{
    private Rigidbody2D rb;
	public Vector2 direction;
	public bool hasHit = false;
	public float speed = 10f;
    public float rotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = 2.4f * direction * speed;
        // rb.AddForce(direction*speed*100f);
        rb.AddTorque(-30*Mathf.Sign(direction.x));
        rb.SetRotation(rotation*Mathf.Sign(direction.x));

        // rb.centerOfMass = new Vector2(3.0f, 0.0f);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), Player.instance.GetComponent<Collider2D>(), true);
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
            hasHit = true;
			StartCoroutine(KillArrow());
		}
	}

    IEnumerator KillArrow() {
        GetComponent<Rigidbody2D>().simulated = false;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}

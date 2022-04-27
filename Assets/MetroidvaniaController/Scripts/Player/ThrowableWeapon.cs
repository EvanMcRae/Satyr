using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableWeapon : MonoBehaviour
{
	public Vector2 direction;
	public bool hasHit = false;
	public float speed = 10f;
    public float rotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = 2.5f*direction * speed;
        // GetComponent<Rigidbody2D>().AddForce(direction*speed*100f);
        GetComponent<Rigidbody2D>().AddTorque(-30*Mathf.Sign(direction.x));
        GetComponent<Rigidbody2D>().SetRotation(rotation*Mathf.Sign(direction.x));

        // GetComponent<Rigidbody2D>().centerOfMass = new Vector2(3.0f, 0.0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), Player.instance.GetComponent<Collider2D>(), true);
		
        // if ( !hasHit)
		
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

using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {

	public float life = 10;

	protected bool facingRight = true;
	
	public float speed = 5f;

	public bool isInvincible = false;

	void Awake () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	}

	public void Flip (){
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    public virtual void ApplyDamage(float damage) {}

	void OnCollisionStay2D(Collision2D collision)
	{
	}
}

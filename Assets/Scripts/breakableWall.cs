using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakableWall : MonoBehaviour
{

    public float life = 7;
    public SpriteRenderer sr;
    public Sprite damagedSprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(life < 7)
        {
            sr.sprite = damagedSprite;
        }

        if(life <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ApplyDamage(float damage)
    {

        float direction = damage / Mathf.Abs(damage);
        damage = Mathf.Abs(damage);
        // transform.GetComponent<Animator>().SetBool("Hit", true);
        life -= damage;
      //  rb.velocity = Vector2.zero;
       // rb.AddForce(new Vector2(direction * 500f, 100f));
       // StartCoroutine(HitTime());

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;

    public Transform player;
    private Vector2 target;
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        float singleStep = speed * Time.deltaTime;
        player = Player.instance.transform;
        Vector3 targetDirection = player.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DestroyProjectile();
            collision.gameObject.GetComponent<Player>().ApplyDamage(1.0f, this.transform.position, 30f);
        }
        else if (collision.CompareTag("Wall"))
        {
            DestroyProjectile();
        }
        else if (collision.CompareTag("Ground"))
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    

}

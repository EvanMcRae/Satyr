using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    Transform player;

    public bool isFlipped = false;

    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;
    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if(transform.position.x < player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if(transform.position.x > player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    public void Attack()
    {
        print("boss attacks");
        Vector3 pos = transform.position;
        //pos += transform.right * attackOffset.x;
        pos += transform.right * -attackOffset.x;
        pos += transform.up * attackOffset.y;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            if(colInfo.tag == "Player")
            {
                colInfo.GetComponent<Player>().ApplyDamage(1.0f, this.transform.position, 30f);
            }
        }
    }

}

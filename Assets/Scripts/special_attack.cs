using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class special_attack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        float dmgValue = 7;
        if (collision.transform.position.x - transform.position.x < 0)
        {
            dmgValue = -dmgValue;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().ApplyDamage(dmgValue, 1);
        }
        else if (collision.gameObject.tag == "Breakable Wall")
        {
            collision.gameObject.GetComponent<breakableWall>().life -= Mathf.Abs(dmgValue);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deathblocks : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<CharacterController2D>().ApplyDamage(1.0f, this.transform.position, 0f);
            // print("this is working");
            col.gameObject.GetComponent<CharacterController2D>().GoToResetPoint();
            // col.gameObject.transform.position = new Vector3(col.gameObject.GetComponent<CharacterController2D>().reset_point.position.x, col.gameObject.GetComponent<CharacterController2D>().reset_point.position.y, col.gameObject.GetComponent<CharacterController2D>().reset_point.position.z);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<CharacterController2D>().ApplyDamage(1.0f, this.transform.position, 0f);
            // print("this is working");
            col.gameObject.GetComponent<CharacterController2D>().GoToResetPoint();
            // col.gameObject.transform.position = new Vector3(col.gameObject.GetComponent<CharacterController2D>().reset_point.position.x, col.gameObject.GetComponent<CharacterController2D>().reset_point.position.y, col.gameObject.GetComponent<CharacterController2D>().reset_point.position.z);
        }
    }

}

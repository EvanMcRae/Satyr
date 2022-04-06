using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafSpawn : MonoBehaviour
{
    public Transform leaves;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            GetComponent<AudioSource>().PlayDelayed(0.2f);
            Vector3 currentPosition = leaves.transform.position;
            currentPosition.x = Player.instance.transform.position.x;
            leaves.position = currentPosition;

            foreach (GameObject leaf in GameObject.FindGameObjectsWithTag("Leaf")) {
                leaf.GetComponent<ParticleSystem>().Play();
            }
        }
    }
}

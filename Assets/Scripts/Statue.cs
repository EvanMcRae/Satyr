using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public bool beenUsed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!beenUsed)
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<Spawnpoint>().SetSpawnpoint(transform);
                GetComponent<GameSaver>().SaveGame();
            }
            beenUsed = true;
        }
    }
}

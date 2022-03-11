using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerCheck : MonoBehaviour
{
    public GameObject audioManager;
    public GameObject database;

    // Start is called before the first frame update
    void Start()
    {
        if (!FindObjectOfType<AudioManager>())
        {
            Instantiate(audioManager, transform.position, transform.rotation);
        }

        if (!FindObjectOfType<Database>()) {
            Instantiate(database, transform.position, transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

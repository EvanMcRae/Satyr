using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSound : MonoBehaviour
{
    private bool triggeredEnter, triggeredExit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !triggeredEnter)
        {
            AudioManager.instance.FadeOutCurrent();
            GetComponent<AudioSource>().Play();
            triggeredEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !triggeredExit)
        {
            GetComponent<AudioSource>().Stop();
            triggeredExit = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WindSound : MonoBehaviour
{
    private bool triggeredEnter, triggeredExit;
    public BoxCollider2D invisibleWall, cameraBounds;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<Volume>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.controller.initialFall) {
            triggeredEnter = true;
            triggeredExit = true;
            invisibleWall.enabled = false;
            cameraBounds.enabled = false;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !triggeredEnter)
        {
            invisibleWall.enabled = true;
            cameraBounds.enabled = true;
            AudioManager.instance.FadeOutCurrent();
            GetComponent<AudioSource>().Play();
            triggeredEnter = true;
            GetComponentInChildren<Volume>().enabled = true;
            Player.controller.limitFallSpeed = 30f;
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player" && triggeredEnter && !triggeredExit) {
            if (Player.controller.isWallSliding && Player.instance.GetComponent<Rigidbody2D>().velocity.y > -2.0f) {
                GetComponent<AudioSource>().Stop();
            } else if (!GetComponent<AudioSource>().isPlaying) {
                GetComponent<AudioSource>().Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !triggeredExit)
        {
            invisibleWall.enabled = false;
            cameraBounds.enabled = false;
            GetComponent<AudioSource>().Stop();
            triggeredExit = true;
            GetComponentInChildren<Volume>().enabled = false;
            Player.controller.limitFallSpeed = 20f;
        }
    }
}

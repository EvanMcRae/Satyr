using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingSound : MonoBehaviour
{
    public bool triggered;

    // Start is called before the first frame update
    void Start()
    {
        triggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.controller.initialFall) triggered = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !triggered)
        {
            GetComponent<AudioSource>().Play();
            Player.controller.LandParticles();
            AudioManager.instance.FadeInCurrent();
            FindObjectOfType<CameraFollow>().ShakeCamera(0.3f);
            triggered = true;
            Player.controller.initialFall = true;
        }
    }
}

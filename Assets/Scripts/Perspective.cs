using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perspective : MonoBehaviour
{
    private float startpos;
    public GameObject cam;
    public float perspectiveEffect;
    public float offset;

    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        startpos = transform.position.x;
        float dist = (cam.transform.position.x * perspectiveEffect);
        transform.position = new Vector3(startpos - dist, transform.position.y, transform.position.z);
    }
    void Update()
    {
        if (cam == null){
            cam = GameObject.FindGameObjectWithTag("MainCamera");
        }
        // float temp = (cam.transform.position.x * (1 - perspectiveEffect));
        float dist = (cam.transform.position.x * perspectiveEffect);
        Vector3 newPosition = new Vector3(startpos - dist, transform.position.y, transform.position.z);
        transform.position = newPosition;
        // if (temp > startpos + (length - offset))
        // {
        //     startpos += length;
        // }
        // else if (temp < startpos - (length - offset))
        // {
        //     startpos -= length;
        // }
    }
}
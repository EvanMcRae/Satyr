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
        cam = CharacterController2D.instance.gameObject;
        startpos = transform.position.x;
    }
    void Update()
    {
        // float temp = (cam.transform.position.x * (1 - perspectiveEffect));
        float dist = (cam.transform.position.x * perspectiveEffect);
        transform.position = new Vector3(startpos - dist, transform.position.y, transform.position.z);
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
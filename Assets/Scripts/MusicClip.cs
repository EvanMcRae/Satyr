using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicClip : MonoBehaviour
{
    public int musicBPM, timeSignature, barsLength;
    public AudioClip clip;
    private float loopPointMinutes, loopPointSeconds;

    // Start is called before the first frame update
    void Start()
    {
        loopPointMinutes = (barsLength * timeSignature) / musicBPM;
        loopPointSeconds = loopPointMinutes * 60;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

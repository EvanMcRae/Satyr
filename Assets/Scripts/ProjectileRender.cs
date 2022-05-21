using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRender : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.localRotation = transform.localRotation * Quaternion.Inverse(transform.rotation);
    }
}

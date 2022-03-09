using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vector3Serialization
{
    public float x, y, z;

    public Vector3Serialization(Vector3 position)
    {
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
    }

    public Vector3 GetValue()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public class Vector2Serialization
{
    public float x, y;

    public Vector2Serialization(Vector2 position)
    {
        this.x = position.x;
        this.y = position.y;
    }

    public Vector2 GetValue()
    {
        return new Vector2(x, y);
    }
}
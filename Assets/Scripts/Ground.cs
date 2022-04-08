using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public enum GroundType {
        ROCK, GRASS, WOOD
    }

    public GroundType type;
}

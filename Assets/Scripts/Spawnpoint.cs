using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawnpoint : MonoBehaviour
{
    // Start is called before the first frame update
    public int scene;
    public Vector2 position;

    private void Start() {
        scene = 2;
        position = GameObject.Find("PlayerCheck").transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpawnpoint(Transform statue) {
        scene = SceneManager.GetActiveScene().buildIndex;
        position = statue.position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnManager : MonoBehaviour
{
    public static string spawningAt;
    private Transform player;

    private Transform spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance.transform;
        if (spawningAt != null && !PlayerSaver.loading) {
            spawnPoint = GameObject.Find(spawningAt).transform;
            player.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
            transform.position = player.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

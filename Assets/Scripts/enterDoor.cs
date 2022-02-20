using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class enterDoor : MonoBehaviour
{
    bool playerIsInRange = false;
    string labelText = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if(playerIsInRange == true)
            {
                SceneManager.LoadScene("Tree_house");
                // Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                // Transform spawnpoint = GameObject.Find("PlayerCheck").GetComponent<Transform>();
                // player.position = spawnpoint.position;
            }
        }
    }


    public void OnGUI()
    {
        if (playerIsInRange == true)
        {
            GUI.Box(new Rect(140, Screen.height - 50, Screen.width - 300, 120), (labelText));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsInRange = true;
            labelText = "Enter";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerIsInRange = false;
    }


}

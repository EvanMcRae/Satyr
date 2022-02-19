using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explorer : MonoBehaviour
{
    public Transform textBox;

    float x = 0;

    bool hasCollided = false;

    bool needsPrompt = true;

    string labelText = "";

    DialougeTrigger[] dialogueTriggers;

    public Transform anchor;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        x = transform.position.x;
        textBox.position = new Vector3(textBox.position.x, 5000f, textBox.position.z);

        dialogueTriggers = GetComponents<DialougeTrigger>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && hasCollided == true && needsPrompt == true)
        {
            textBox.position = new Vector3(textBox.position.x, anchor.position.y, textBox.position.z);
            needsPrompt = false;
            if(player.GetComponent<CharacterController2D>().explorer == false)
            {
                dialogueTriggers[0].TriggerDialogue();
            }
            else if(player.GetComponent<CharacterController2D>().explorer == true)
            {
              
                dialogueTriggers[1].TriggerDialogue();
            }
      

        }
        else if (Input.GetKeyDown(KeyCode.W) && hasCollided == true && needsPrompt == false)
        {

            GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().DisplayNextSentence();

            if (DialougeManager.convoEnded == true)
            {
                player.GetComponent<CharacterController2D>().explorer = true;
                textBox.position = new Vector3(textBox.position.x, 5000f, textBox.position.z);
                
            }
        }
    }

    public void OnGUI()
    {
        if (hasCollided == true && needsPrompt == true)
        {
            GUI.Box(new Rect(140, Screen.height - 50, Screen.width - 300, 120), (labelText));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            hasCollided = true;
            labelText = "Hit W to listen";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hasCollided = false;
        needsPrompt = true;
        textBox.position = new Vector3(textBox.position.x, 5000f, textBox.position.z);
        DialougeManager.convoEnded = true;
    }
}

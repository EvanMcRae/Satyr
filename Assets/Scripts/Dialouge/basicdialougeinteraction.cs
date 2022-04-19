using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicdialougeinteraction : MonoBehaviour
{

    public Transform textBox;

    float x = 0;

    bool hasCollided = false;

    bool needsPrompt = true;

    string labelText = "";

    DialougeTrigger[] dialogueTriggers;

    public Transform anchor;

    // Start is called before the first frame update
    void Start()
    {
        x = transform.position.x;
        textBox.position = new Vector3(textBox.position.x, 5000f, textBox.position.z);

        dialogueTriggers = GetComponents<DialougeTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerMovement.paused)
        {
            if ((Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown("joystick button 1")) && hasCollided == true && needsPrompt == true)
            {

                textBox.position = new Vector3(textBox.position.x, anchor.position.y, textBox.position.z);
                needsPrompt = false;
                //   this.GetComponent<DialougeTrigger>().TriggerDialogue();
                /*  foreach(DialougeTrigger dt in dialogueTriggers)
                  {
                      dt.TriggerDialogue();
                      break;
                  }  */
                dialogueTriggers[0].TriggerDialogue();

            }
            else if ((Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown("joystick button 1")) && hasCollided == true && needsPrompt == false)
            {
                if (DialougeManager.stillSpeaking)
                {
                    GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().FinishSentence();
                }
                else
                {
                    GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().DisplayNextSentence();

                    if (DialougeManager.convoEnded)
                    {
                        textBox.position = new Vector3(textBox.position.x, 5000f, textBox.position.z);
                    }
                }
            }
        }
    }

    public void OnGUI()
    {
        if (!PlayerMovement.paused && hasCollided && needsPrompt)
        {
            GUI.Box(new Rect(140, Screen.height - 50, Screen.width - 300, 120), (labelText));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            hasCollided = true;
            labelText = "Press T to listen";
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class basicdialougeinteraction : MonoBehaviour
{

    public Transform textBox;

    float x = 0;

    bool hasCollided = false;

    bool needsPrompt = true;
    public bool autoStart = false;
    public bool anyKey = false;

    [SerializeField] string labelText = "Press B button or T key to listen.";

    DialougeTrigger[] dialogueTriggers;

    public Transform anchor;
    public GameObject InfoBox, InfoText;

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
            if ((Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown("joystick button 1") || (anyKey && Input.anyKeyDown) || autoStart) && hasCollided == true && needsPrompt == true)
            {
                if (InfoText.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("textfade_start"))
                {
                    InfoText.GetComponent<Animator>().SetTrigger("stop");
                    InfoBox.GetComponent<Animator>().SetTrigger("stop");
                }
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
            else if ((Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown("joystick button 1") || (anyKey && Input.anyKeyDown)) && hasCollided == true && needsPrompt == false)
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            hasCollided = true;
            if (!PlayerMovement.paused && hasCollided && needsPrompt && !autoStart && InfoText.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("textfade_hold"))
            {
                InfoText.GetComponent<Text>().text = labelText;
                InfoText.GetComponent<Animator>().SetTrigger("start");
                InfoBox.GetComponent<Animator>().SetTrigger("start");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (InfoText.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("textfade_start"))
        {
            InfoText.GetComponent<Animator>().SetTrigger("stop");
            InfoBox.GetComponent<Animator>().SetTrigger("stop");
        }
        hasCollided = false;
        needsPrompt = true;
        textBox.position = new Vector3(textBox.position.x, 5000f, textBox.position.z);
        DialougeManager.convoEnded = true;
    }

}

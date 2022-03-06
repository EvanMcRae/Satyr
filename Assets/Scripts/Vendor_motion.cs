using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendor_motion : MonoBehaviour
{
    public Transform player;
    public SpriteRenderer sr;

    public float speed;
    public float verticalSpeed;
    public float maxRotation;

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
        player = CharacterController2D.instance.gameObject.transform;
        if(player.transform.position.x > this.transform.position.x)
        {
            // transform.eulerAngles = new Vector3(0, -180, 0);
            sr.flipX = true;
        }
        else
        {
            // transform.eulerAngles = new Vector3(0, 0, 0);
            sr.flipX = false;
        }


        transform.rotation = Quaternion.Euler(0f, 0f, maxRotation * Mathf.Sin(Time.time * speed));

        transform.position = new Vector3(x, .3f * Mathf.Sin(Time.time * verticalSpeed), 0f);

        if(Input.GetKeyDown(KeyCode.W) && hasCollided == true && needsPrompt == true)
        {
            
            textBox.position = new Vector3(textBox.position.x, anchor.position.y, textBox.position.z);
            needsPrompt = false;
            //   this.GetComponent<DialougeTrigger>().TriggerDialogue();
            /*  foreach(DialougeTrigger dt in dialogueTriggers)
              {
                  dt.TriggerDialogue();
                  break;
              }  */

            if(CharacterController2D.instance.doubleJump_Unlocked == false)
            {
                dialogueTriggers[0].TriggerDialogue();
            }
            else if(dialogueTriggers.Length > 1)
            {
                dialogueTriggers[1].TriggerDialogue();
            }
            

        }
        else if (Input.GetKeyDown(KeyCode.W) && hasCollided == true && needsPrompt == false)
        {
            
            GameObject.Find("Dialouge Manager").GetComponent<DialougeManager>().DisplayNextSentence();

            // for unlocking movement in the test scene
            CharacterController2D.instance.gameObject.GetComponent<PlayerMovement>().dash_Unlocked = true;
            CharacterController2D.instance.doubleJump_Unlocked = true;

            if (DialougeManager.convoEnded == true)
            {
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
        if(collision.gameObject.tag == "Player")
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

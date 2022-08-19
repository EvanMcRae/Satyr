using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class enterDoor : MonoBehaviour
{
    bool playerIsInRange = false;
    string labelText = "";


    public string scene;
    public string spawn;
    private Animator crossfade;
    public GameObject InfoBox, InfoText;

    // Start is called before the first frame update
    void Start()
    {
        crossfade = GameObject.Find("Crossfade").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown("joystick button 1")) && playerIsInRange && !changeScene.changingScene)
        {
            StartCoroutine(LoadNextScene());
            changeScene.changingScene = true;
        }
        Debug.Log(gameObject.name + " " + "running");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (GetComponent<enterDoor>().enabled && !PlayerMovement.paused && collision.gameObject.tag == "Player" && InfoText.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("textfade_hold"))
        {
            playerIsInRange = true;
            labelText = "Press B button or T key to enter.";
            InfoText.GetComponent<Text>().text = labelText;
            InfoText.GetComponent<Animator>().SetTrigger("start");
            InfoBox.GetComponent<Animator>().SetTrigger("start");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GetComponent<enterDoor>().enabled)
        {
            playerIsInRange = false;
            InfoText.GetComponent<Animator>().SetTrigger("stop");
            InfoBox.GetComponent<Animator>().SetTrigger("stop");
        }
    }

    IEnumerator LoadNextScene()
    {
        changeScene.changingScene = true;
        crossfade.SetTrigger("start");
        yield return new WaitForSeconds(0.9f);
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            GameObject.Destroy(eventSystem.gameObject);
        }
        SceneHelper.LoadScene(scene);
        spawnManager.spawningAt = spawn;
        changeScene.changingScene = false;
    }
}

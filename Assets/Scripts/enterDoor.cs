using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class enterDoor : MonoBehaviour
{
    bool playerIsInRange = false;
    string labelText = "";


    public string scene;
    public string spawn;
    private Animator crossfade;
    // Start is called before the first frame update
    void Start()
    {
        crossfade = GameObject.Find("Crossfade").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey(KeyCode.T) || Input.GetKeyDown("joystick button 1")) && playerIsInRange)
        {
            StartCoroutine(LoadNextScene());
        }
    }


    public void OnGUI()
    {
        if (!PlayerMovement.paused && playerIsInRange)
        {
            GUI.Box(new Rect(140, Screen.height - 50, Screen.width - 300, 120), (labelText));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsInRange = true;
            labelText = "Press B button or T key to Enter";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerIsInRange = false;
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

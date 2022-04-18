using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private Animator crossfade;
    private Coroutine routine;
    public Button loadButton;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        crossfade = GameObject.Find("Crossfade").GetComponent<Animator>();
        Destroy(Player.instance);
    }

    void Update() {
        if (!GetComponent<SaveSystem>().SaveFileExists())
        {
            loadButton.interactable = false;
            loadButton.GetComponentInChildren<TMP_Text>().color = new Color32(118, 118, 118, 255);
        } else {
            loadButton.interactable = true;
            loadButton.GetComponentInChildren<TMP_Text>().color = new Color32(154, 127, 0, 255);
        }
    }
    

    public void PlayGame()
    {
        if (routine == null)
            routine = StartCoroutine(StartGame());
    }

    public void QuitGame()
    {
        Debug.Log("User has quit!");
        Application.Quit();
    }

    IEnumerator StartGame()
    {
        crossfade.SetTrigger("start");
        yield return new WaitForSeconds(0.9f);
        SceneManager.LoadScene("1stScene");
        // SceneManager.LoadScene("Cutscene");
        yield return null;
    }
}

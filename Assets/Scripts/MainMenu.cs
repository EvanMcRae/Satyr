using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private Animator crossfade;
    private Coroutine routine;
    public Button loadButton;
    public GameObject defaultButton;
    static GameObject lastSelected;
    Button[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        crossfade = GameObject.Find("Crossfade").GetComponent<Animator>();
        Destroy(Player.instance);
        buttons = GetComponentsInChildren<Button>();
    }

    void Update()
    {
        if (lastSelected == null || !lastSelected.activeInHierarchy || !lastSelected.GetComponent<Button>().interactable)
        {
            if (defaultButton.activeInHierarchy) lastSelected = defaultButton;
        }

        if (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy)
        {
            if (lastSelected.gameObject.activeInHierarchy && lastSelected.GetComponent<Button>() != null && lastSelected.GetComponent<Button>().interactable)
            {
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }
        }
        else
        {
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }

        if (GetComponent<SaveSystem>() != null && loadButton != null)
        {
            if (!GetComponent<SaveSystem>().SaveFileExists())
            {
                loadButton.interactable = false;
                loadButton.GetComponentInChildren<TMP_Text>().color = new Color32(118, 118, 118, 255);
            }
            else
            {
                loadButton.interactable = true;
                if (lastSelected.GetComponent<Button>() != loadButton)
                {
                    loadButton.GetComponentInChildren<TMP_Text>().color = new Color32(154, 127, 0, 255);
                }
            }
        }

        foreach (Button b in buttons) {
            if (lastSelected == b.gameObject) 
            {
                b.GetComponentInChildren<TMP_Text>().color = new Color32(255, 218, 43, 255);
            }
            else if (b.interactable)
            {
                b.GetComponentInChildren<TMP_Text>().color = new Color32(154, 127, 0, 255);
            }
        }
    }

    public void SelectButton(GameObject btn)
    {
        EventSystem.current.SetSelectedGameObject(btn);
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
        AudioManager.instance.FadeOutCurrent();
        crossfade.SetTrigger("start");
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("1stScene");
        // SceneManager.LoadScene("Cutscene");
        yield return null;
    }
}

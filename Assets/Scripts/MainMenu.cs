using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Animator crossfade;
    private Coroutine routine;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        crossfade = GameObject.Find("Crossfade").GetComponent<Animator>();
        Destroy(Player.instance);
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

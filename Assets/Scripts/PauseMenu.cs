using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private Animator crossfade;
    private Coroutine routine;

    // Start is called before the first frame update
    void Start()
    {
        crossfade = GameObject.Find("Crossfade").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MainMenu() {
        if (routine == null)
            routine = StartCoroutine(GoToMainMenu());
    }

    public void QuitGame()
    {
        if (routine == null)
            routine = StartCoroutine(ExitApplication());
    }

    IEnumerator GoToMainMenu()
    {
        AudioManager.instance.Stop();
        Player.instance.GetComponent<PlayerMovement>().TogglePause();
        GetComponent<GameSaver>().SaveGame();
        crossfade.SetTrigger("start");
        yield return new WaitForSeconds(0.9f);
        SceneManager.LoadScene("Main_Menu");
        yield return null;
    }

    IEnumerator ExitApplication()
    {
        GetComponent<GameSaver>().SaveGame();
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
        yield return null;
    }
}

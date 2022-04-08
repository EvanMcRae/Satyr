using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    public VideoPlayer player;
    private Animator crossfade;
    public string nextScene;
    private Coroutine routine;
    private bool startedPlaying;

    // Start is called before the first frame update
    void Start()
    {
        crossfade = GameObject.Find("Crossfade").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!startedPlaying && player.isPlaying) {
            startedPlaying = true;
        }
        if (startedPlaying && !player.isPlaying && routine == null) {
            routine = StartCoroutine(EndCutscene());
        }
    }

    IEnumerator EndCutscene()
    {
        crossfade.SetTrigger("start");
        yield return new WaitForSeconds(0.9f);
        SceneHelper.LoadScene(nextScene);
        yield return null;
    }
}

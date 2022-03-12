using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public bool beenUsed = false;
    public static bool cutscening = false;
    public static Transform currStatue;
    public GameObject cam;
    public int ID = 0;

    private void Update()
    {
        if (Player.instance.GetComponent<Spawnpoint>().statuesUsed.Contains(ID))
        {
            beenUsed = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Player.instance.GetComponent<Spawnpoint>().statuesUsed.Contains(ID))
        {
            beenUsed = true;
        }
        if (!beenUsed)
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<Spawnpoint>().SetSpawnpoint(transform, ID);
                GetComponent<GameSaver>().SaveGame();
                StartCoroutine(StatueCutscene());
            }
            beenUsed = true;
        }
    }

    IEnumerator StatueCutscene() {
        cutscening = true;
        currStatue = this.transform;
        cam.GetComponentInChildren<CameraFollow>().ShakeCamera();
        cam.GetComponentInChildren<CameraFollow>().shakeDuration = 5f;
        GameObject.FindObjectOfType<CinematicBars>().Show(200, .3f);
        AudioManager.instance.FadeOutCurrent();
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1.1f);
        AudioManager.instance.PauseCurrent();
        yield return new WaitForSeconds(8.9f);
        cutscening = false;
        currStatue = null;
        AudioManager.instance.UnPauseCurrent();
        AudioManager.instance.FadeInCurrent();
        GameObject.FindObjectOfType<CinematicBars>().Hide(.3f);
        GameObject.Find("SaveText").GetComponent<Animator>().SetTrigger("start");
        yield return null;
    }
}

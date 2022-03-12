using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public bool beenUsed = false;
    public static bool cutscening = false;
    public static Transform currStatue;
    public GameObject cam;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!beenUsed)
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<Spawnpoint>().SetSpawnpoint(transform);
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
        yield return new WaitForSeconds(10.0f);
        cutscening = false;
        currStatue = null;
        GameObject.Find("SaveText").GetComponent<Animator>().SetTrigger("start");
        yield return null;
    }
}

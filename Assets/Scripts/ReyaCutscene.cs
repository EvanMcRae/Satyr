using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ReyaCutscene : MonoBehaviour
{
    public GameObject cam;
    public Volume volume;
    public float cutsceneTime;
    public static bool cutscening = false;

    private void FixedUpdate() {
        transform.position = cam.transform.position;

        if (cutscening) {
            GetComponentInChildren<Camera>().orthographicSize = cam.GetComponent<Camera>().orthographicSize;

            cutsceneTime += Time.fixedDeltaTime;

            if (cutsceneTime < 7.5f)
            {
                float shakeAmount = Mathf.Lerp(0.2f, 0.01f, cutsceneTime / 7.5f);
                cam.GetComponent<CameraFollow>().SetShakeAmount(shakeAmount);
                volume.weight = Mathf.Lerp(0f, 1f, cutsceneTime / 2.5f);
            }

            else if (cutsceneTime >= 7.5f)
            {
                volume.weight = Mathf.Lerp(1f, 0f, (cutsceneTime - 7.5f) / 2.5f);
            }
        }
    }   
     
    public void StartCutscene()
    {
        StartCoroutine(Cutscene());
    }

    IEnumerator Cutscene() {
        GetComponentInChildren<Camera>().enabled = true;
        cam.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("TransparentFX"));
        cutscening = true;
        volume.enabled = true;
        Player.controller.canMove = false;
        cam.GetComponent<CameraFollow>().ShakeCamera(5f);
        GameObject.FindObjectOfType<CinematicBars>().Show(200, .3f);
        AudioManager.instance.FadeOutCurrent();
        yield return new WaitForSeconds(1.1f);
        AudioManager.instance.PauseCurrent();
        // you wouldn't want to wait a set amount of seconds here, this is where dialogue would happen
        // once dialogue is done, then unpause and finish cutscene
        // cutscene will not always be 10s long
        yield return new WaitForSeconds(8.9f);
        AudioManager.instance.UnPauseCurrent();
        AudioManager.instance.FadeInCurrent();
        GameObject.FindObjectOfType<CinematicBars>().Hide(.3f);
        cam.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("TransparentFX");
        GetComponentInChildren<Camera>().enabled = false;
        cam.GetComponent<CameraFollow>().SetShakeAmount(0.1f);
        cutscening = false;
        volume.enabled = false;
        Player.controller.canMove = true;
    }
}

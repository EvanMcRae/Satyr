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
    public GameObject Reya;
    public Transform camTarget;
    public static bool cutscening = false;

    private void FixedUpdate() {
        transform.position = cam.transform.position;

        if (cutscening) {
            Player.controller.canMove = false;
            
            GetComponentInChildren<Camera>().orthographicSize = cam.GetComponent<Camera>().orthographicSize;

            if (cutsceneTime <= 7.5f)
                cutsceneTime += Time.fixedDeltaTime;

            if (cutsceneTime >= 7.5f && DialougeManager.convoEnded)
                cutsceneTime += Time.fixedDeltaTime;

            if (cutsceneTime < 7.5f)
            {
                float shakeAmount = Mathf.Lerp(0.2f, 0.01f, cutsceneTime / 7.5f);
                cam.GetComponent<CameraFollow>().SetShakeAmount(shakeAmount);
                volume.weight = Mathf.Lerp(0f, 1f, cutsceneTime / 2.5f);
            }

            if (cutsceneTime > 2.5f && cutsceneTime <= 7.5f)
            {
                Reya.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(0, 1, (cutsceneTime-2.5f)/5.0f));
            }

            if (cutsceneTime > 7.4f && !DialougeManager.convoEnded) {
                Reya.GetComponent<BoxCollider2D>().enabled = true;
            }

            if (DialougeManager.convoEnded && cutsceneTime > 7.5f)
            {
                Reya.GetComponent<BoxCollider2D>().enabled = false;
                Reya.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, (cutsceneTime - 7.5f) / 2.5f));
                volume.weight = Mathf.Lerp(1f, 0f, (cutsceneTime - 7.5f) / 2.5f);
            }
        }
    }   
     
    public void StartCutscene()
    {
        StartCoroutine(Cutscene());
    }

    IEnumerator Cutscene() {
        Player.controller.reya = true;
        GetComponentInChildren<Camera>().enabled = true;

        // position Reya relative to player
        if (Player.controller.m_FacingRight) {
            Reya.transform.position = Player.instance.transform.position + new Vector3(2f, 1f, 0);
        } else {
            Reya.transform.position = Player.instance.transform.position + new Vector3(-2f, 1f, 0);
            Reya.GetComponent<SpriteRenderer>().flipX = true;
        }

        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        
        // position cam target
        camTarget.position = Vector3.Lerp(Player.instance.transform.position, Reya.transform.position, 0.5f);

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
        yield return new WaitUntil(() => DialougeManager.convoEnded);
        yield return new WaitForSeconds(2.5f);
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

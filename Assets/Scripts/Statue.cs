using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Statue : MonoBehaviour
{
    public bool beenUsed = false;
    public static bool cutscening = false;
    public static Transform currStatue;
    public GameObject cam;
    public int ID = 0;
    public Sprite activeSprite;
    public Volume volume;
    MotionBlur motionBlur;
    Bloom bloom;
    ChromaticAberration chromaticAberration;
    public float cutsceneTime;

    private void Start()
    {
        gameObject.GetComponentInChildren<ParticleSystem>().Stop();
    }

    private void Update()
    {
        if (Player.instance.GetComponent<Spawnpoint>().statuesUsed.Contains(ID))
        {
            beenUsed = true;
        }
        if (beenUsed && !cutscening)
        {
            if (!gameObject.GetComponentInChildren<ParticleSystem>().isPlaying)
                gameObject.GetComponentInChildren<ParticleSystem>().Play();
            GetComponent<SpriteRenderer>().sprite = activeSprite;
        }
    }

    private void FixedUpdate()
    {
        if (cutscening)
        {
            cutsceneTime += Time.fixedDeltaTime;
            
            if (cutsceneTime < 5f)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(0, 0.25f, cutsceneTime / 5f);
                motionBlur.intensity.value = Mathf.Lerp(0, 1, cutsceneTime / 5f);
                bloom.intensity.value = Mathf.Lerp(0, 20, cutsceneTime / 5f);
            }
            else if (cutsceneTime > 5f && cutsceneTime < 7.5f)
            {
                bloom.intensity.value = Mathf.Lerp(20, 10, (cutsceneTime - 5) / 2.5f);
                chromaticAberration.intensity.value = 1;
            }
            else if (cutsceneTime > 7.5f)
            {
                bloom.intensity.value = Mathf.Lerp(10, 0, (cutsceneTime - 7.5f) / 2.5f);
                chromaticAberration.intensity.value = Mathf.Lerp(1, 0, (cutsceneTime - 7.5f) / 2.5f);
            }
            else if (cutsceneTime > 10f || cutsceneTime <= 0f)
            {
                motionBlur.intensity.value = 0;
                bloom.intensity.value = 0;
                chromaticAberration.intensity.value = 0;
            }
        }
        else
        {
            cutsceneTime = 0.0f;
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
                beenUsed = true;
            }
        }
    }

    IEnumerator StatueCutscene() {
        cutscening = true;
        currStatue = this.transform;

        // postprocessing
        volume.enabled = true;
        volume.profile.TryGet<MotionBlur>(out motionBlur);
        volume.profile.TryGet<Bloom>(out bloom);
        volume.profile.TryGet<ChromaticAberration>(out chromaticAberration);

        cam.GetComponent<CameraFollow>().ShakeCamera(5f);
        GameObject.FindObjectOfType<CinematicBars>().Show(200, .3f);
        AudioManager.instance.FadeOutCurrent();
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1.1f);
        AudioManager.instance.PauseCurrent();
        yield return new WaitForSeconds(3.9f);
        gameObject.GetComponentInChildren<ParticleSystem>().Play();
        GetComponent<SpriteRenderer>().sprite = activeSprite;
        yield return new WaitForSeconds(5f);
        GetComponentInChildren<Volume>().enabled = false;
        cutscening = false;
        currStatue = null;
        AudioManager.instance.UnPauseCurrent();
        AudioManager.instance.FadeInCurrent();
        GameObject.FindObjectOfType<CinematicBars>().Hide(.3f);
        GameObject.Find("SaveText").GetComponent<Animator>().SetTrigger("start");
        yield return null;
    }
}

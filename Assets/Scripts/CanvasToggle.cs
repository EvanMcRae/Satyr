using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasToggle : MonoBehaviour
{
    public Camera cam;
    private Canvas canvas;
    private GameObject[] pauseText;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindObjectOfType<Camera>();
        canvas = GetComponent<Canvas>();
        pauseText = GameObject.FindGameObjectsWithTag("PauseText");
    }

    public void Pause() {
        canvas.worldCamera = cam;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        foreach (GameObject p in pauseText)
        {
            if (p.GetComponent<Text>() != null)
                p.GetComponent<Text>().enabled = true;
            if (p.GetComponent<Button>() != null)
                p.GetComponent<Button>().enabled = true;
            if (p.GetComponent<Image>() != null)
                p.GetComponent<Image>().enabled = true;
        }
    }

    public void UnPause() {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        foreach (GameObject p in pauseText)
        {
            if (p.GetComponent<Text>() != null)
                p.GetComponent<Text>().enabled = false;
            if (p.GetComponent<Button>() != null)
                p.GetComponent<Button>().enabled = false;
            if (p.GetComponent<Image>() != null)
                p.GetComponent<Image>().enabled = false;
        }
    }
}

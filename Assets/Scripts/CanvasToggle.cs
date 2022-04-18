using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasToggle : MonoBehaviour
{
    public Camera cam;
    private Canvas canvas;
    private Text pauseText;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindObjectOfType<Camera>();
        canvas = GetComponent<Canvas>();
        pauseText = GameObject.FindGameObjectWithTag("PauseText").GetComponent<Text>();
    }

    public void Pause() {
        canvas.worldCamera = cam;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        pauseText.enabled = true;
    }

    public void UnPause() {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        pauseText.enabled = false;
    }
}

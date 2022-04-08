﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    public Camera m_camera;
	public float FollowSpeed = 2f;
	[SerializeField] private Transform Target;

	// How long the object should shake for.
	public float shakeDuration = 0f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.1f;
	public float decreaseFactor = 1.0f;
    public float speedMultiplier = 1.0f;
    public static Vector3 screenPos;
	Vector3 originalPos;

	public bool bounds;
    private GameObject[] boundaries;
    private Bounds[] allBounds;
    private Bounds targetBounds;
    private BoxCollider2D camBox;

	public Vector3 MinCameraPos;
	public Vector3 MaxCameraPos;
    public float xBias;
    public float yBias;

    void Start()
	{
        m_camera = GetComponent<Camera>();
        originalPos = transform.position;
		Cursor.visible = false;

        Target = Player.controller.camTarget;
        Snap(Target.position);
        camBox = GetComponent<BoxCollider2D>();
        FindLimits();
    }

	void OnEnable()
	{
		originalPos = transform.position;
	}

	private void FixedUpdate()
	{
        if (m_camera == null) {
            m_camera = GetComponent<Camera>();
        }

        if (Statue.cutscening)
        {
            Target = Statue.currStatue;
            Vector3 newPosition = Target.position;
            newPosition.z = -10;
            originalPos = Vector3.Lerp(originalPos, newPosition, FollowSpeed * Time.deltaTime);
            transform.position = originalPos;

            if (m_camera.orthographicSize != 4.0f)
            {
                m_camera.orthographicSize = Mathf.Lerp(m_camera.orthographicSize, 4, 0.01f);
                m_camera.orthographicSize -= 0.001f;
            }
            if (m_camera.orthographicSize < 4.005f)
            {
                m_camera.orthographicSize = 4;
            }
        }
        else
        {
            if (m_camera.orthographicSize != 7.0f)
            {
                m_camera.orthographicSize = Mathf.Lerp(m_camera.orthographicSize, 7, 0.01f);
                m_camera.orthographicSize += 0.001f;
            }
            if (m_camera.orthographicSize > 6.995f)
            {
                m_camera.orthographicSize = 7;
            }
            Target = Player.controller.camTarget;

            Vector3 newPosition = Target.position;
            newPosition.z = -10;

            if (Input.GetAxisRaw("Vertical") < -0.5) //&& !Input.GetKey(KeyCode.S)
            {
                Target.localPosition = new Vector3(0.0f, -6.0f, 0.0f); //originally -2
            }
            else if (Input.GetAxisRaw("Vertical") > 0.5) //&& !Input.GetKey(KeyCode.W)
            {
                Target.localPosition = new Vector3(0.0f, 4.0f, 0.0f); //originally 4
            }
            else
            {
                Target.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
            }

            if (Player.instance.GetComponent<Rigidbody2D>().velocity.magnitude > 25f)
            {
                speedMultiplier = Mathf.Lerp(speedMultiplier, 2, 0.01f);
            }
            else
            {
                speedMultiplier = Mathf.Lerp(speedMultiplier, 1, 0.01f);
            }

            if (bounds)
            {
                FindLimits();
                SetOneLimit();
                float xTarget = camBox.size.x < targetBounds.size.x ? Mathf.Clamp(Target.position.x, targetBounds.min.x + camBox.size.x / 2, targetBounds.max.x - camBox.size.x / 2) : (targetBounds.min.x + targetBounds.max.x) / 2;
                float yTarget = camBox.size.y < targetBounds.size.y ? Mathf.Clamp(Target.position.y, targetBounds.min.y + camBox.size.y / 2, targetBounds.max.y - camBox.size.y / 2) : (targetBounds.min.y + targetBounds.max.y) / 2;
                Vector3 boundedTarget = new Vector3(xTarget, yTarget, -10);
                originalPos = Vector3.Lerp(transform.position, boundedTarget, FollowSpeed * Time.deltaTime * speedMultiplier);
            }
            else
            {
                originalPos = Vector3.Lerp(originalPos, newPosition, FollowSpeed * Time.deltaTime * speedMultiplier);
            }
        }

        transform.position = originalPos;

        if (shakeDuration > 0)
        {
            transform.position = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else if (shakeDuration != 0)
        {
            transform.position = originalPos;
            shakeDuration = 0;
        }

        // on screen checks
        var cam = FindObjectOfType<Camera>();
        screenPos = cam.WorldToScreenPoint(Player.instance.transform.position);
        bool onScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;
    }

	public void ShakeCamera(float duration)
	{
		originalPos = transform.position;
		shakeDuration = duration;
	}

    public void Snap(Vector3 position)
    {
        originalPos = position;
        transform.position = position;
    }

    public void FindLimits() {
        boundaries = GameObject.FindGameObjectsWithTag("Boundary");
        allBounds = new Bounds[boundaries.Length];
        for (int i = 0; i < allBounds.Length; i++) {
            allBounds[i] = boundaries[i].gameObject.GetComponent<BoxCollider2D>().bounds;
        }
    }

    void SetOneLimit()
    {
        bool first = true;
        for (int i = 0; i < boundaries.Length; i++) {
            if (withinBounds(boundaries[i])) {
                if (first)
                {
                    targetBounds = boundaries[i].gameObject.GetComponent<BoxCollider2D>().bounds;
                    xBias = boundaries[i].gameObject.GetComponent<CameraBounds>().xBias;
                    yBias = boundaries[i].gameObject.GetComponent<CameraBounds>().yBias;
                    first = false;
                }
                else {
                    combineLimits(boundaries[i]);
                }
            }
        }

    }

    void combineLimits(GameObject newBounds) {
        print("combining limits");
        float x2 = newBounds.gameObject.GetComponent<CameraBounds>().xBias;
        float y2 = newBounds.gameObject.GetComponent<CameraBounds>().yBias;
        Bounds box = newBounds.gameObject.GetComponent<BoxCollider2D>().bounds;
        float xMin = targetBounds.min.x;
        float xMax = targetBounds.max.x;
        float yMin = targetBounds.min.y;
        float yMax = targetBounds.max.y;
        if (x2 > xBias)
        {
            xMin = box.min.x;
            xMax = box.max.x;
        }
        else if (x2 == xBias) {
            if (box.min.x <= targetBounds.min.x) {
                xMin = box.min.x;
            }

            if (box.max.x >= targetBounds.max.x) {
                xMax = box.max.x;
            }
        }
        if (y2 > yBias)
        {
            yMin = box.min.y;
            yMax = box.max.y;
        }
        else if (y2 == yBias)
        {
            if (box.min.y <= targetBounds.min.y)
            {
                yMin = box.min.y;
            }

            if (box.max.y >= targetBounds.max.y)
            {
                yMax = box.max.y;
            }
        }

        targetBounds.min = new Vector3(xMin, yMin, -10);
        targetBounds.max = new Vector3(xMax, yMax, -10);
    }

    bool withinBounds(GameObject boundary) {
        Bounds box = boundary.gameObject.GetComponent<BoxCollider2D>().bounds;
        return (Target.position.x > box.min.x && Target.position.x < box.max.x && Target.position.y > box.min.y && Target.position.y < box.max.y);
        
        
    }
}

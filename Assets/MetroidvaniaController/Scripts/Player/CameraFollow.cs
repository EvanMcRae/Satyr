using System.Collections;
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
    public static Vector3 screenPos;

	Vector3 originalPos;

	public bool bounds;

	public Vector3 MinCameraPos;
	public Vector3 MaxCameraPos;

	void Start()
	{
        m_camera = GetComponent<Camera>();
        originalPos = transform.position;
		Cursor.visible = false;

        Target = Player.controller.camTarget;
        Snap(Target.position);
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

            float speedMultiplier = 1.0f;
            if (Player.instance.GetComponent<Rigidbody2D>().velocity.magnitude > 30f) {
                speedMultiplier = Player.instance.GetComponent<Rigidbody2D>().velocity.magnitude/25f;
                Debug.Log(speedMultiplier);
            }
            originalPos = Vector3.Lerp(originalPos, newPosition, FollowSpeed * Time.deltaTime * speedMultiplier);
            transform.position = originalPos;

            if (bounds)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, MinCameraPos.x, MaxCameraPos.x),
                    Mathf.Clamp(transform.position.y, MinCameraPos.y, MaxCameraPos.y),
                    Mathf.Clamp(transform.position.z, MinCameraPos.z, MaxCameraPos.z));
            }
        }

        if (shakeDuration > 0)
        {
            transform.position = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else if (shakeDuration != -1)
        {
            transform.position = originalPos;
            shakeDuration = -1;
        }

        // on screen checks
        var cam = FindObjectOfType<Camera>();
        screenPos = cam.WorldToScreenPoint(Player.instance.transform.position);
        bool onScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;
    }

	public void ShakeCamera()
	{
		originalPos = transform.position;
		shakeDuration = 0.2f;
	}

    public void Snap(Vector3 position)
    {
        originalPos = position;
        transform.position = position;
    }
}

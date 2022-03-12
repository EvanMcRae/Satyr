using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera m_camera;
	public float FollowSpeed = 2f;
	[SerializeField] private Transform Target;

	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	private Transform camTransform;

	// How long the object should shake for.
	public float shakeDuration = 0f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.1f;
	public float decreaseFactor = 1.0f;

	Vector3 originalPos;

	public bool bounds;

	public Vector3 MinCameraPos;
	public Vector3 MaxCameraPos;

	void Start()
	{
        m_camera = transform.parent.GetComponent<Camera>();
        originalPos = new Vector3(0.0f, 0.0f, 0.0f);
		Cursor.visible = false;
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}

        Target = Player.camTarget;
        if (Target == null)
        {
            Target = GameObject.FindGameObjectWithTag("CamTarget").transform;
            Player.camTarget = Target;
        }
        Snap();
    }

	void OnEnable()
	{
		// originalPos = camTransform.localPosition;
	}

	private void FixedUpdate()
	{
        if (Statue.cutscening)
        {
            Target = Statue.currStatue;
            Vector3 newPosition = Target.position;
            newPosition.z = -10;
            transform.parent.position = Vector3.Lerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);
            if (m_camera.orthographicSize != 4.0f)
            {
                m_camera.orthographicSize = Mathf.Lerp(m_camera.orthographicSize, 4, 0.01f);
            }
            if (m_camera.orthographicSize < 4.02f)
            {
                m_camera.orthographicSize = 4;
            }
        }
        else
        {
            if (m_camera.orthographicSize != 7.0f)
            {
                m_camera.orthographicSize = Mathf.Lerp(m_camera.orthographicSize, 7, 0.01f);
            }
            if (m_camera.orthographicSize > 6.98f)
            {
                m_camera.orthographicSize = 7;
            }
            Target = Player.camTarget;
            if (Target == null)
            {
                Target = GameObject.FindGameObjectWithTag("CamTarget").transform;
                Player.camTarget = Target;
            }

            var cam = FindObjectOfType<Camera>();
            var player = Player.instance;
            var m_renderer = player.GetComponent<Renderer>();
            var screenPos = cam.WorldToScreenPoint(player.transform.position);
            bool onScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;

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

            transform.parent.position = Vector3.Lerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);

            if (bounds)
            {
                transform.parent.position = new Vector3(Mathf.Clamp(transform.parent.position.x, MinCameraPos.x, MaxCameraPos.x),
                    Mathf.Clamp(transform.parent.position.y, MinCameraPos.y, MaxCameraPos.y),
                    Mathf.Clamp(transform.parent.position.z, MinCameraPos.z, MaxCameraPos.z));
            }
        }

        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            camTransform.localPosition = originalPos;
        }
	}

	public void ShakeCamera()
	{
		originalPos = camTransform.localPosition;
		shakeDuration = 0.2f;
	}

    public void Snap()
    {
        transform.parent.position = Target.position;
    }
}

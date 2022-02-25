using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
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

	void Start()
	{
        originalPos = new Vector3(0.0f, 0.0f, 0.0f);
		Cursor.visible = false;
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
        transform.parent.position = Target.position;

        Target = GameObject.FindGameObjectWithTag("CamTarget").GetComponent<Transform>();
    }

	void OnEnable()
	{
		// originalPos = camTransform.localPosition;
	}

	private void Update()
	{
        Target = GameObject.FindGameObjectWithTag("CamTarget").GetComponent<Transform>();

        var cam = FindObjectOfType<Camera>();
        var player = GameObject.FindGameObjectWithTag("Player");
        var m_renderer = player.GetComponent<Renderer>();
        var screenPos = cam.WorldToScreenPoint(player.transform.position);
        bool onScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;

        Vector3 newPosition = Target.position;
        newPosition.z = -10;

        if (Input.GetAxisRaw("Vertical") < -0.5 && !Input.GetKey(KeyCode.S))
        {
            Target.localPosition = new Vector3(0.0f, -2.0f, 0.0f);
        }
        else if (Input.GetAxisRaw("Vertical") > 0.5 && !Input.GetKey(KeyCode.W))
        {
            Target.localPosition = new Vector3(0.0f, 4.0f, 0.0f);
        }
        else
        {
            Target.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
        }

        transform.parent.position = Vector3.Slerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);

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

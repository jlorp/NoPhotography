using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour {

	public static OrbitCamera Instance;

	[SerializeField]
	Transform focus = default;

	[SerializeField, Range (0f ,10f)]
	public float distance;

	[SerializeField, Range(1f, 360f)]
	float rotationSpeed = 90f;

	[SerializeField, Range(-89f, 89f)]
	float minVerticalAngle = -45f, maxVerticalAngle = 45f;

	[SerializeField, Min(0f)]
	float upAlignmentSpeed = 360f;

	[SerializeField]
	LayerMask obstructionMask = -1;

	Camera regularCamera;

	Vector3 focusPoint, previousFocusPoint;

	Vector3 orbitAngles = new Vector2(45f, 0f);

	Quaternion gravityAlignment = Quaternion.identity;

	Quaternion orbitRotation;
	bool desiresFlatTilt;

	public Transform resetPosition;

	public AnimationCurve _boostCurve;

	[HideInInspector]
	public bool isActivated = false;

	Vector3 startPosition;
	Quaternion startRotation;
	float startScreenFOV;

	void Awake () {
		Instance = this;
		regularCamera = GetComponent<Camera>();
		focusPoint = focus.position;
		
		startPosition = transform.position;
		startRotation = transform.localRotation;
		startScreenFOV = Camera.main.fieldOfView;
	}

	public void DeactivateCamera()
	{
		isActivated = false;
	}

	public void GoToRespawnCameraPosition()
	{
		transform.position = resetPosition.position;
		transform.localRotation = resetPosition.localRotation;
	}

	public void GoToStartPosition()
	{
		transform.position = startPosition;
		transform.localRotation = startRotation;
	}
	
	public void LerpToActivation()
	{
		orbitAngles = new Vector3 (0,90,0);
		transform.localRotation = orbitRotation = Quaternion.Euler(orbitAngles);
		isActivated = true;
	}

	Transform CreateResetPositionObject()
	{
		Transform newObject = new GameObject("ResetPosition").transform;
		newObject.position = transform.position;
		newObject.localRotation = transform.rotation;
		return newObject;
	}

	public void CameraBoostLag(float duration, float zoomAmount, float FOVammount)
	{
		StartCoroutine(CameraZoomBump(duration, zoomAmount, FOVammount));
	}

	IEnumerator CameraZoomBump(float duration, float zoomAmount, float FOVammount)
    {
        float time = 0f;
		float startingDistance = distance;
		float startFOV = Camera.main.fieldOfView;

        while (time < duration) 
        {
            time += Time.deltaTime;

			float boostCurve = _boostCurve.Evaluate(time/duration);
			distance = Mathf.Lerp(startingDistance, startingDistance + zoomAmount, boostCurve);
			Camera.main.fieldOfView = Mathf.Lerp(startFOV, startFOV + FOVammount, boostCurve);

            yield return null; 
        }
		distance = startingDistance;
		Camera.main.fieldOfView = startFOV;
		GameManager.Instance.player.boosting = false;
    }

	Vector3 CameraHalfExtends {
		get {
			Vector3 halfExtends;
			halfExtends.y =
				regularCamera.nearClipPlane *
				Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
			halfExtends.x = halfExtends.y * regularCamera.aspect;
			halfExtends.z = 0f;
			return halfExtends;
		}
	}

	void OnValidate () {
		if (maxVerticalAngle < minVerticalAngle) {
			maxVerticalAngle = minVerticalAngle;
		}
	}

	public void ForceFlattenTilt()
	{
		desiresFlatTilt=true;
	}

	void LateUpdate () {

		if(UIManager.gameIsPaused) return;
		if(!isActivated) return;

		UpdateGravityAlignment();
		UpdateFocusPoint();
		if (ManualRotation()) {
			ConstrainAngles();
			orbitRotation = Quaternion.Euler(orbitAngles);
		}
		Quaternion lookRotation = gravityAlignment * orbitRotation;

		Vector3 lookDirection = lookRotation * Vector3.forward;
		Vector3 lookPosition = focusPoint - lookDirection * distance;


		Vector3 rectOffset = lookDirection * regularCamera.nearClipPlane;
		Vector3 rectPosition = lookPosition + rectOffset;
		Vector3 castFrom = focus.position;
		Vector3 castLine = rectPosition - castFrom;
		float castDistance = castLine.magnitude;
		Vector3 castDirection = castLine / castDistance;

		if (Physics.BoxCast(
			castFrom, CameraHalfExtends, castDirection, out RaycastHit hit,
			lookRotation, castDistance, obstructionMask,
			QueryTriggerInteraction.Ignore
		)) {
			rectPosition = castFrom + castDirection * hit.distance;
			lookPosition = rectPosition - rectOffset;
		}

		transform.SetPositionAndRotation(lookPosition, lookRotation);

		if(resetPosition == null)
		{
			resetPosition = CreateResetPositionObject();
		}
	}

	void UpdateGravityAlignment () {
		Vector3 fromUp = gravityAlignment * Vector3.up;
		Vector3 toUp = CustomGravity.GetUpAxis(focusPoint);
		float dot = Mathf.Clamp(Vector3.Dot(fromUp, toUp), -1f, 1f);
		float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
		float maxAngle = upAlignmentSpeed * Time.deltaTime;

		Quaternion newAlignment =
			Quaternion.FromToRotation(fromUp, toUp) * gravityAlignment;
		if (angle <= maxAngle) {
			gravityAlignment = newAlignment;
		}
		else {
			gravityAlignment = Quaternion.SlerpUnclamped(
				gravityAlignment, newAlignment, maxAngle / angle
			);
		}
	}

	void UpdateFocusPoint () {
		previousFocusPoint = focusPoint;
		focusPoint  = focus.position;
	}

	bool ManualRotation () {
		Vector3 input = new Vector3(
			Input.GetAxis("Vertical Camera"),
			Input.GetAxis("Horizontal Camera"),
			-Input.GetAxisRaw("Tilt") * .5f
		);

		const float e = 0.001f;
		if (input.x < -e || input.x > e || input.y < -e || input.y > e || input.z > e || input.z < -e || desiresFlatTilt) {
			orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
			if(desiresFlatTilt)
			{
				orbitAngles.z = 0;	
				desiresFlatTilt = false;
			}
			return true;
		}
		return false;
	}

	void ConstrainAngles () {
		orbitAngles.x =
			Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

		if (orbitAngles.y < 0f) {
			orbitAngles.y += 360f;
		}
		else if (orbitAngles.y >= 360f) {
			orbitAngles.y -= 360f;
		}

		orbitAngles.z = Mathf.Clamp(orbitAngles.z, -45,45);
		if(!UIManager.cameraIsOpen) orbitAngles.z=0;
	}

	static float GetAngle (Vector2 direction) {
		float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
		return direction.x < 0f ? 360f - angle : angle;
	}
}

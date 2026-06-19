using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
	public Vector3 speed = Vector3.zero;
	public Space space = Space.World;

	[SerializeField]
	private bool randomizeOnEnable = false;

	private new Rigidbody rigidbody = null;
	private bool hasRigidbody = false;

	public bool ignoreTimeScale = false;

	public bool fixedUpdate = true;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
		hasRigidbody = rigidbody != null;
	}

	private void OnEnable()
	{
		if (randomizeOnEnable)
			RotateByAngle(Random.value * 360f, speed.normalized);
	}

	private void Update()
	{
		if(!fixedUpdate)
			ApplyRotate();
	}

	private void FixedUpdate()
	{
		if(fixedUpdate)
			ApplyRotate();
	}

	private void ApplyRotate()
	{
		Vector3 axis = speed.normalized;
		float angle = speed.magnitude * (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);

		RotateByAngle(angle, axis);
	}

	private void RotateByAngle(float angle, Vector3 axis)
	{
		Quaternion rotation = hasRigidbody ? rigidbody.rotation : transform.rotation;
        
		if (space == Space.World)
    		rotation = Quaternion.AngleAxis(angle, axis) * rotation;
		else
			rotation = Quaternion.AngleAxis(angle, rotation * axis) * rotation;
		
        if(hasRigidbody)
            rigidbody.MoveRotation(rotation);
        else
            transform.rotation = rotation;
	}
}

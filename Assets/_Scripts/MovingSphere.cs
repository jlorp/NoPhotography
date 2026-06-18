using UnityEngine;

public class MovingSphere : MonoBehaviour {

	public bool lockInput = false;

	[SerializeField]
	Transform playerInputSpace = default;

	[SerializeField]
	public Transform submarine;

	[SerializeField, Range(0f, 100f)]
	float maxSwimSpeed = 5f;

	[SerializeField, Range(0f, 100f)]
	float maxSwimAcceleration = 5f;

	[SerializeField, Range(0f, 30f)]
	float swimRotation = 2f;

	[SerializeField, Range(0, 90)]
	float maxGroundAngle = 25f;

	[SerializeField]
	float submergenceOffset = 0.5f;

	[SerializeField, Min(0.1f)]
	float submergenceRange = 1f;

	[SerializeField, Min(0f)]
	float buoyancy = 1f;

	[SerializeField, Range(0f, 10f)]
	float waterDrag = 1f;

	[SerializeField, Range(0.01f, 1f)]
	float swimThreshold = 0.5f;

	[SerializeField]
	LayerMask probeMask = -1, waterMask = 0;

	[HideInInspector]
	public Rigidbody body, connectedBody, previousConnectedBody;

	[HideInInspector]
	public Vector3 playerInput;

	Vector3 velocity, connectionVelocity;

	Vector3 connectionWorldPosition, connectionLocalPosition;
	
	Vector3 upAxis, rightAxis, forwardAxis;

	Vector3 contactNormal, steepNormal;

	Vector3 lastContactNormal, lastSteepNormal, lastConnectionVelocity;

	int groundContactCount, steepContactCount, climbContactCount;

	bool OnGround => groundContactCount > 0;

	bool OnSteep => steepContactCount > 0;

	[HideInInspector]
	public bool CameraOpen = false;

	bool InWater => submergence > 0f;

	[HideInInspector]
	public bool Swimming => submergence >= swimThreshold;

	[HideInInspector]
	public bool drowning = false;

	float submergence;

	float minGroundDotProduct;

	[Header("Boost")]

	public bool boostUnlocked = true;

	public float boostAmount = 10f;

	[HideInInspector]
	public bool desiredBoost, boosting;

	[Header("Dependencies")]
	
	[SerializeField]
	CameraLogic _camera;

	public BreathLogic _breath;

	MeshRenderer meshRenderer;

	public bool isHeldByClaw;

	void OnValidate () {
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
	}

	void Awake () {
		body = GetComponent<Rigidbody>();
		body.useGravity = false;
		OnValidate();
		CameraOpen = false;

		Cursor.lockState = CursorLockMode.Locked;
    	Cursor.visible = false;
	}

	void HandleCursorLock()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
    	{
        	if (Cursor.lockState == CursorLockMode.Locked)
        	{
            	Cursor.lockState = CursorLockMode.None;
            	Cursor.visible = true;
        	}
        	else
        	{
            	Cursor.lockState = CursorLockMode.Locked;
            	Cursor.visible = false;
        	}
    	}
	}
	
	public void LockInput()
	{
		playerInput = Vector3.zero;
		lockInput = true;
	}

	public void UnlockInput()
	{
		lockInput = false;
	}

	void Brake()
	{
		velocity = Vector3.MoveTowards(velocity, Vector3.zero, Time.deltaTime * 10);
	}

	void UpdateInputs()
	{
		HandleCursorLock();

		if(CameraOpen)
		{
			_camera.CameraControls();
		}

		if(Input.GetButtonDown("Open Camera"))
		{
			if (CameraOpen)
			{
				UIManager.Instance.CloseCamera();
				CameraOpen = false;
			}
			else
			{
				UIManager.Instance.OpenCamera();
				CameraOpen = true;
			}
		}

		if (lockInput) return;

		playerInput.x = Input.GetAxisRaw("Horizontal");
		playerInput.z = Input.GetAxisRaw("Shutter");
		if(Input.GetButton("Shutter Mouse")) playerInput.z = 1;
		playerInput.y = Input.GetAxisRaw("Vertical");


		desiredBoost |= Input.GetButtonDown("Jump");
	}

	void Update () {

		if(body.isKinematic) return;

		UpdateInputs();

		if (playerInputSpace) 
		{
			rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
			forwardAxis =
				ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
		}
		else 
		{
			rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
			forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
		}
		
		if (Swimming) 
		{
			forwardAxis = submarine.forward;
			rightAxis = submarine.right;
		}
	}

	void Boost()
	{
		if (boosting || !boostUnlocked) return;
		boosting = true;
		velocity += submarine.forward * boostAmount;
		velocity = Vector3.ClampMagnitude(velocity, 20f);
		OrbitCamera.Instance.CameraBoostLag( 1f , -0.5f, 20);
		_breath.RemoveBreath(5f);
	}

	void FixedUpdate () {

		if(body.isKinematic)
		{
			if(isHeldByClaw) {transform.localPosition= Vector3.zero;}
			return;
		} 

		Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
	
		UpdateState();

		if(CameraOpen) Brake();

		if(desiredBoost)
		{
			desiredBoost=false;
			Boost();
		}

		if (InWater) {
			velocity *= 1f - waterDrag * submergence * Time.deltaTime;
		}

		if(Swimming)
		{
			AdjustVelocityWater();
		}

		if (InWater) {
			velocity +=
				gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
		}
		else if (OnGround && velocity.sqrMagnitude < 0.01f) {
			velocity +=
				contactNormal *
				(Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
		}
		else {
			velocity += gravity * Time.deltaTime;
		}
		body.velocity = velocity;
		ClearState();
	}

	void ClearState () {
		lastContactNormal = contactNormal;
		lastSteepNormal = steepNormal;
		lastConnectionVelocity = connectionVelocity;
		groundContactCount = steepContactCount = 0;
		contactNormal = steepNormal = Vector3.zero;
		connectionVelocity = Vector3.zero;
		previousConnectedBody = connectedBody;
		connectedBody = null;
		submergence = 0f;
	}

	void UpdateState () {
		velocity = body.velocity;
		if (
			CheckSwimming() ||
			OnGround || CheckSteepContacts()
		) {
			if (groundContactCount > 1) {
				contactNormal.Normalize();
			}
		}
		else {
			contactNormal = upAxis;
		}
		
		if (connectedBody) {
			if (connectedBody.isKinematic || connectedBody.mass >= body.mass) {
				UpdateConnectionState();
			}
		}
	}

	void UpdateConnectionState () {
		if (connectedBody == previousConnectedBody) {
			Vector3 connectionMovement =
				connectedBody.transform.TransformPoint(connectionLocalPosition) -
				connectionWorldPosition;
			connectionVelocity = connectionMovement / Time.deltaTime;
		}
		connectionWorldPosition = body.position;
		connectionLocalPosition = connectedBody.transform.InverseTransformPoint(
			connectionWorldPosition
		);
	}

	bool CheckSwimming () {
		if (Swimming) {
			groundContactCount = 0;
			contactNormal = upAxis;
			return true;
		}
		return false;
	}

	bool CheckSteepContacts () {
		if (steepContactCount > 1) {
			steepNormal.Normalize();
			float upDot = Vector3.Dot(upAxis, steepNormal);
			if (upDot >= minGroundDotProduct) {
				steepContactCount = 0;
				groundContactCount = 1;
				contactNormal = steepNormal;
				return true;
			}
		}
		return false;
	}

	void AdjustVelocityWater()
	{
		if (lockInput) return;
		float swimFactor = Mathf.Min(1f, submergence / swimThreshold);
		float acceleration = maxSwimAcceleration;
		
		Vector3 zAxis = forwardAxis;
		Vector3 relativeVelocity = velocity - connectionVelocity;

		Vector3 playerInputLocal = zAxis * playerInput.z;
		playerInputLocal = Vector3.ClampMagnitude(playerInputLocal, 1);

		transform.Rotate(Vector3.up, playerInput.x * swimRotation * 10 * Time.deltaTime);
		transform.Rotate(Vector3.right, playerInput.y * swimRotation * 10 * Time.deltaTime);

		var rotation = transform.eulerAngles;
		float zLerp = Mathf.LerpAngle(rotation.z, 0, Time.deltaTime * 10f);
		
		transform.eulerAngles = new Vector3(rotation.x, rotation.y, zLerp);

		Vector3 targetSpeed = playerInputLocal * maxSwimSpeed;

		float vectorcomp = Vector3.Dot(velocity.normalized,targetSpeed.normalized);
		float vectorcompClamped = Mathf.Clamp(vectorcomp,0,1);

		float inputAdjustment = 1;
		if(playerInput.z == 0) {inputAdjustment = 0.05f;}
		else if(velocity.magnitude > targetSpeed.magnitude) inputAdjustment = 1-vectorcompClamped;

		velocity = Vector3.MoveTowards(velocity, targetSpeed, acceleration * Time.deltaTime * inputAdjustment); 

		if((velocity.magnitude > maxSwimSpeed + 3f && playerInput.magnitude>0f && vectorcomp > 0.5f))
		{
			targetSpeed = submarine.forward * velocity.magnitude;
			velocity = Vector3.MoveTowards(velocity,targetSpeed, acceleration * Time.deltaTime * 10);
		}
	}

	void OnTriggerEnter (Collider other) {
		if ((waterMask & (1 << other.gameObject.layer)) != 0) {
			EvaluateSubmergence(other);
		}
	}

	void OnTriggerStay (Collider other) {
		if ((waterMask & (1 << other.gameObject.layer)) != 0) {
			EvaluateSubmergence(other);
		}
	}

	void EvaluateSubmergence (Collider collider) {
		if (Physics.Raycast(
			body.position + upAxis * submergenceOffset,
			-upAxis, out RaycastHit hit, submergenceRange + 1f,
			waterMask, QueryTriggerInteraction.Collide
		)) {
			submergence = 1f - hit.distance / submergenceRange;
		}
		else {
			submergence = 1f;
		}
		if (Swimming) {
			connectedBody = collider.attachedRigidbody;
		}
	}

	Vector3 ProjectDirectionOnPlane (Vector3 direction, Vector3 normal) {
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}
}

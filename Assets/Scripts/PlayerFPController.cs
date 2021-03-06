using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif


	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif

public class PlayerFPController : MonoBehaviour
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	public float MoveSpeed = 4.0f;
	[Tooltip("Sprint speed of the character in m/s")]
	public float SprintSpeed = 6.0f;
	[Tooltip("Rotation speed of the character")]
	public float RotationSpeed = 1.0f;
	[Tooltip("Acceleration and deceleration")]
	public float SpeedChangeRate = 10.0f;
	[Tooltip("How much control does the player have in the air")]
	public float aerialDirectionControl = 0.5f;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float Gravity = -15.0f;

	public float CrouchPlayerScale = 0.75f;
	public float CrouchSpeedScale = 0.70f;
	public float CrouchRotationScale = 0.9f;
	public float CrouchTimeout = 0.1f;

	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout = 0.1f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout = 0.15f;

	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	public bool Grounded = true;
	[Tooltip("Useful for rough ground")]
	public float GroundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	public float GroundedRadius = 0.5f;
	[Tooltip("What layers the character uses as ground")]
	public LayerMask GroundLayers;

	[Header("Cinemachine")]
	[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
	public GameObject CinemachineCameraTarget;
	[Tooltip("How far in degrees can you move the camera up")]
	public float TopClamp = 90.0f;
	[Tooltip("How far in degrees can you move the camera down")]
	public float BottomClamp = -90.0f;

	// cinemachine
	private float _cinemachineTargetPitch;

	// player
	private float _speed;
	private float _rotationVelocity;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	// timeout deltatime
	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;
	private float _crouchTimeoutDelta;

	private PlayerInput _playerInput;
	public CharacterController characterController;
	public CapsuleCollider capsule;
	private PlayerInputHandler _pinput;

	private Camera _camera;

	private const float _threshold = 0.01f;
	protected bool isCrouching = false;

	public bool IsCrouching
	{
		get
		{
			return isCrouching;
		}
	}

	public bool IsSprinting
	{
		get
		{
			return _pinput.sprint;
		}
	}

	private float GetMoveSpeed
	{
		get
		{
			return isCrouching ? MoveSpeed * CrouchSpeedScale : MoveSpeed;
		}
	}

	private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

	protected float defaultCapsuleHeight;
	protected Vector3 defaultCameraPos;

	//private void Awake()
	//{
	//	// get a reference to our main camera
	//	if (_Camera == null) {
	//		_Camera = GameObject.FindGameObjectWithTag("MainCamera");
	//	}
	//}

	private void Start()
	{
		characterController = GetComponent<CharacterController>();
		capsule = GetComponentInChildren<CapsuleCollider>();
		_pinput = GetComponent<PlayerInputHandler>();
		//_pinput = _pinput as PlayerInputs;
		_playerInput = GetComponent<PlayerInput>();
		_camera = GameManager.PlayerCam;

		// reset our timeouts on start
		_jumpTimeoutDelta = JumpTimeout;
		_fallTimeoutDelta = FallTimeout;

		defaultCapsuleHeight = characterController.height;
		defaultCameraPos = _camera.transform.localPosition;
	}

	private void Update()
	{
		if (!GameManager.CanMove)
			return;

		JumpAndGravity();
		GroundedCheck();
		CrouchInput();
		Move();

		//Debug.LogFormat("Player Speed: {0}", _controller.velocity.magnitude);
	}

	private void LateUpdate()
	{
		if (!GameManager.CanMove)
			return;

		CameraRotation();
	}

	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
	}


	private void StopCrouch()
	{
		if (!isCrouching)
			return;

		if (_crouchTimeoutDelta > 0.0f)
			return;

		//float heightMidpoint = defaultCapsuleHeight - (defaultCapsuleHeight * CrouchPlayerScale);
		float heightMidpoint = (defaultCapsuleHeight - 0.15f);

		//Physics.IgnoreCollision()
		Collider[] cols = Physics.OverlapSphere(transform.position + (Vector3.up * heightMidpoint), 0.15f);

		if (cols.Length > 0) {
			bool self = true;
			foreach (Collider col in cols) {
				if (!col.CompareTag(GameManager.PlayerTag))
					self = false;
			}
			if (!self)
				return;
		}		

		_pinput.crouch = false;
		isCrouching = false;
		_crouchTimeoutDelta = CrouchTimeout;

		//transform.localScale = Vector3.one;
		capsule.height = defaultCapsuleHeight;
		characterController.height = defaultCapsuleHeight;
		_camera.transform.localPosition = defaultCameraPos;

	}

	private void StartCrouch()
	{
		if (isCrouching)
			return;

		if (_crouchTimeoutDelta > 0.0f)
			return;

		isCrouching = true;
		_crouchTimeoutDelta = CrouchTimeout;
			
		//Vector3 scale = transform.localScale;
		//scale.y = CrouchPlayerScale;
		//transform.localScale = scale;

		capsule.height = defaultCapsuleHeight * CrouchPlayerScale;
		characterController.height = capsule.height;
		_camera.transform.localPosition = new Vector3(defaultCameraPos.x, defaultCameraPos.y * CrouchPlayerScale, defaultCameraPos.z);

	}

	private void CrouchInput()
	{

		if (!Grounded)
			StopCrouch();


		if (_pinput.crouch) {

			if (!isCrouching)
				StartCrouch();
			else
				StopCrouch();

			_pinput.crouch = false;
		}


		if (_crouchTimeoutDelta >= 0.0f)
			_crouchTimeoutDelta -= Time.deltaTime;

	}

	private void CameraRotation()
	{
		// if there is an input
		if (_pinput.look.sqrMagnitude >= _threshold) {
			//Don't multiply mouse input by Time.deltaTime
			float deltaTimeMultiplier = (IsCurrentDeviceMouse ? 1.0f : Time.deltaTime) * Time.timeScale;

			_cinemachineTargetPitch += _pinput.look.y * RotationSpeed * deltaTimeMultiplier * CrouchRotationScale;
			_rotationVelocity = _pinput.look.x * RotationSpeed * deltaTimeMultiplier * CrouchRotationScale;

			// clamp our pitch rotation
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

			// Update Cinemachine camera target pitch
			CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

			// rotate the player left and right
			transform.Rotate(Vector3.up * _rotationVelocity);
		}
	}

	private void Move()
	{
		// set target speed based on move speed, sprint speed and if sprint is pressed

		//float targetSpeed = (_pinput.sprint) ? SprintSpeed : MoveSpeed;
		float targetSpeed = (_pinput.sprint && Grounded && !isCrouching) ? SprintSpeed : GetMoveSpeed; //Prevents sprinting in the air

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (_pinput.move == Vector2.zero) targetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		Vector3 currentHorizontalVelocity = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z);
		//float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
		float currentHorizontalSpeed = currentHorizontalVelocity.magnitude;

		float speedOffset = 0.1f;
		float inputMagnitude = _pinput.analogMovement ? _pinput.move.magnitude : 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset) {
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed


			//_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate); 
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * (Grounded ? inputMagnitude : 1.0f), Time.deltaTime * SpeedChangeRate); // Reduces input 

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		} else {
			_speed = targetSpeed;
		}

		// normalise input direction
		Vector3 inputDirection = new Vector3(_pinput.move.x, 0.0f, _pinput.move.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (_pinput.move != Vector2.zero) {
			// move
			inputDirection = transform.right * _pinput.move.x + transform.forward * _pinput.move.y;
		}

		Vector3 movementDirection = Grounded ? inputDirection : Vector3.Lerp(currentHorizontalVelocity, inputDirection, aerialDirectionControl);

		// move the player
		//_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		characterController.Move(movementDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
	}

	private void JumpAndGravity()
	{
		if (Grounded) {
			// reset the fall timeout timer
			_fallTimeoutDelta = FallTimeout;

			// stop our velocity dropping infinitely when grounded
			if (_verticalVelocity < 0.0f) {
				_verticalVelocity = -2f;
			}

			// Jump
			if (_pinput.jump && _jumpTimeoutDelta <= 0.0f) {
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				_pinput.jump = false;

				//While jumping, could instead add force in each frame (only if the Jump key was held uninterrupted)
				//e.g.:
				//// apply accumulated 'hold jump' force
				//m_MotorThrottle.y += m_MotorJumpForceAcc * Time.timeScale;
				//// dampen forces
				//m_MotorJumpForceAcc /= (1.0f + (MotorJumpForceHoldDamping * Time.timeScale));
				//m_MotorThrottle.y /= (1.0f + (MotorJumpForceDamping * Time.timeScale));

			}

			// jump timeout
			if (_jumpTimeoutDelta >= 0.0f) {
				_jumpTimeoutDelta -= Time.deltaTime;
			}
		} else {
			// reset the jump timeout timer
			_jumpTimeoutDelta = JumpTimeout;

			// fall timeout
			if (_fallTimeoutDelta >= 0.0f) {
				_fallTimeoutDelta -= Time.deltaTime;
			}

			// if we are not grounded, do not jump
			_pinput.jump = false;
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (_verticalVelocity < _terminalVelocity) {
			_verticalVelocity += Gravity * Time.deltaTime;
		}
	}

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}

	private void OnDrawGizmosSelected()
	{
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (Grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
	}
}
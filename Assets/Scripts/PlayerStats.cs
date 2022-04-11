using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerStats : MonoBehaviour
{

	public string PlayerTag = "Player";
	protected Transform playerCam;
	protected PlayerFPAnimator anim;

	protected PlayerInputs _input;
	
	protected PickableItem item;

	protected float interactReach = 2.5f;
	protected float interactTimeout = 0.1f;
	protected float interactTimeoutDelta = 0f;

	protected float _MaxHealth = 100f;
	public float MaxHealth
	{
		get
		{
			return _MaxHealth;
		}
		protected set
		{
			_MaxHealth = value;
		}
	}
	
	public float Health
	{
		get
		{
			return _health;
		}
		set
		{
			_health = Mathf.Clamp(value, 0f, MaxHealth);
		}
	}
	[SerializeField]
	protected float _health = 100f;

	protected bool _Sickle = false;
	public bool HasSickle
	{
		get
		{
			return _Sickle;
		}
		protected set
		{
			_Sickle = value;
		}
	}

	protected bool _Bell = false;
	public bool HasBell
	{
		get
		{
			return _Bell;
		}
		protected set
		{
			_Bell = value;
		}
	}

	protected Dictionary<Rigidbody, PickableItem> items = new Dictionary<Rigidbody, PickableItem>();



	void Awake()
	{
		Health = MaxHealth;
	}

    // Start is called before the first frame update
    void Start()
    {
		anim = GetComponentInChildren<PlayerFPAnimator>();
		playerCam = GameManager.PlayerCam.transform;
		_input = GetComponent<PlayerInputs>();
	}

	private void Update()
	{
		InteractInput();	
	}

	protected void InteractInput()
	{
		if (_input.secondary) {

			if (interactTimeoutDelta <= 0f) {
				CheckForInteractables();
				interactTimeoutDelta = interactTimeout;
			}

			_input.secondary = false;
		}

		if (interactTimeoutDelta >= 0.0f)
			interactTimeoutDelta -= Time.deltaTime;
	}

	private void FixedUpdate()
	{
		//CheckForInteractables();
	}

	protected void CheckForInteractables()
	{
		Ray ray = new Ray(playerCam.position, playerCam.forward);
		if (!Physics.Raycast(ray, out RaycastHit hitInfo, interactReach))
			return;

		if (!hitInfo.collider) {
			//Debug.LogFormat("No collider on {0}", hitInfo);
			return;
		}

		Rigidbody rb = hitInfo.collider.attachedRigidbody;

		if (!rb) {
			//Debug.LogFormat("No rigidbody on {0}", hitInfo.collider);
			return;
		}

		if (!items.TryGetValue(rb, out item)) {
			item = rb.GetComponent<PickableItem>();
			items.Add(rb, item);
		}

		if (!item) {
			return;
		}

		if (!item.TryInteract())
			return;

		anim.FoundItem();
	}

	public void PickupItem()
	{
		if (!item)
			return;
		item.OnPickup();
	}

	public void Heal(float healAmount)
	{
		Health += healAmount;
	}

	public void Hurt(float hurtAmount)
	{
		Health -= hurtAmount;

		if (Health <= 0f) {
			Die();
		}
	}

	public void GainSickle()
	{
		HasSickle = true;
	}

	public void LoseSickle()
	{
		HasSickle = false;
	}

	public void GainBell()
	{
		HasBell = true;
	}

	public void LoseBell()
	{
		HasBell = false;
	}

	public void Die()
	{
		// Play anim, etc
		// Reset, etc
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.Rendering.PostProcessing;

public class PlayerStats : MonoBehaviour
{

	public readonly string PlayerTag = "Player";
	protected Transform playerCam;
	protected PlayerFPAnimator anim;

	protected PlayerInputHandler _input;
	protected PickableItem item;

	//public Vignette vignette;
	//public PostProcessProfile vignetteProfile;
	protected PostProcessVolume volume;
	protected Vignette vignette;

	protected float interactReach = 2.5f;
	protected float interactTimeout = 0.1f;
	protected float interactTimeoutDelta = 0f;

	protected float _MaxHealth = 90f;
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
	protected float _health;

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

	protected AudioSource _audio;
	public Vector2 pitchRange = new Vector2(0.95f, 1.05f);
	public List<AudioClip> hurtSfx = new List<AudioClip>();

	//void Awake()
	//{
	//	//ResetHealth();
	//}

    // Start is called before the first frame update
    void Start()
    {
		anim = GetComponentInChildren<PlayerFPAnimator>();
		playerCam = GameManager.PlayerCam.transform;
		_input = GetComponent<PlayerInputHandler>();
		_audio = GetComponent<AudioSource>();

		SetupVignette();

		Refresh();
	}

	protected void ResetHealth()
	{
		Health = MaxHealth;
	}
	
	protected void SetupVignette()
	{

		volume = playerCam.GetComponent<PostProcessVolume>();

		if (!volume)
			return;

		if (!volume.profile.TryGetSettings<Vignette>(out vignette))
			return;

	}

	private void Refresh()
	{
		ResetHealth();
		UpdateVignette();
		LoseBell();
		LoseSickle();
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

	//private void FixedUpdate()
	//{
	//	//CheckForInteractables();
	//}

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

		item = null;

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

	// Method called from PlayerFPAnimator when hand is fully extended
	// Actually triggers the 'OnPickup' code, and tells the animator to check weapon contents
	public void PickupItem()
	{
		if (!item)
			return;
		item.OnPickup();
		//anim.UpdateItemsState();
		//Debug.LogFormat("PIcking up {0}, HaveSickle? {1}, HaveBell? {2}", item, HasSickle, HasBell);
		anim.UpdateItemsStateAndWield();
	}

	// Fancy way of writing an OR operator that can be extended infinitely
	public bool HasWeapons()
	{
		if (HasSickle)
			return true;

		if (HasBell)
			return true;

		return false;
	}

	protected float minVignetteIntensity = 0.1f;
	protected float maxVignetteIntensity = 0.5f; //0.6f;

	public void UpdateVignette()
	{
		if (!vignette)
			return;

		float healthRatio = 1 - (Health / MaxHealth);

		//float percentage = ap_Utility.EaseIn(healthRatio);
		//float percentage = ap_Utility.EaseOut(healthRatio); // 0.7f: 1 hit great, 2 hits too much // 0.6f: 1 hit faint, 2 hits too much
		//float percentage = ap_Utility.Smoothstep(healthRatio); // 0.7f: 1 hit faint, 2 hits too much
		//float percentage = ap_Utility.Smootherstep(healthRatio); //0.6f: 1 hit faint, 2 hits... maybe okay //1.0f: 1 hit too faint, 2 hits too much

		float percentage = healthRatio; 
		//0.7f: 1 hit faint, 2 hits okay 
		//0.6f: 1 hit faint, 2 hits okay 
		//0.2,0.6f: initial seems good, can't really tell, 1 hit: good? 2 hits: good
		//0.10.5f: seems good, better to err on the slightly subtle side I think

		vignette.intensity.Override(Mathf.Lerp(minVignetteIntensity, maxVignetteIntensity, percentage));

	}

	public void Heal(float healAmount)
	{
		Health += healAmount;
		UpdateVignette();
	}

	public void Hurt(float hurtAmount)
	{
		Health = Mathf.Max(0f, Health - hurtAmount);
		UpdateVignette();
		if (Health <= 0f) {
			ap_Helper.PlayRandomAudioClip(_audio, hurtSfx, pitchRange);
			Die();
		} else {
			ap_Helper.PlayRandomAudioClip(_audio, hurtSfx, pitchRange);
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
		//GameManager.Die()
		//ResetPosition()
		//Refresh()??? Or,. revert to last level setup?
	}
}

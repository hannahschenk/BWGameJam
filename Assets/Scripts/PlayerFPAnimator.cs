using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFPAnimator : MonoBehaviour
{
	protected PlayerStats stats;

	protected Animator _animator;
	public Animator animator
	{
		get
		{
			return _animator;
		}
		protected set
		{
			_animator = value;
		}
	}

	protected PlayerInputHandler input;

	protected Weapon currentWeapon;

	protected int animTriggerFoundItem;
	protected int animBoolHasItem;
	protected int animTriggerAttack;
	protected int animTriggerDefend;
	protected int animTriggerSwap;
	protected int animBoolCanAttack;
	protected int animBoolCanDefend;

	public Transform hand;
	//protected Dictionary<Transform, Weapon> weapons = new Dictionary<Transform, Weapon>();
	protected Weapon[] weapons;
	//protected int MaxWeapons = 0;


	protected int currentWeaponIndex = -1;

	protected bool swapping = false;

	protected float weaponTimeout = 0.4f;
	protected float nextWeaponWieldTime = 0f;
	protected float nextWeaponSwapTime = 0f;

	public bool HasItem
	{
		get
		{
			return animator.GetBool(animBoolHasItem);
		}
		set
		{
			animator.SetBool(animBoolHasItem, value);
		}
	}


	// Start is called before the first frame update
	void Start()
    {
		animator = GameManager.PlayerAnimator;
		stats = GameManager.PlayerStats;
		input = GameManager.PlayerInputHandler;
		CacheAnimReferences();
    }

	protected void CacheAnimReferences()
	{
		animTriggerFoundItem = Animator.StringToHash("foundItem");
		animBoolHasItem = Animator.StringToHash("hasItem");
		animTriggerAttack = Animator.StringToHash("attack");
		animTriggerDefend = Animator.StringToHash("defend");
		animTriggerSwap = Animator.StringToHash("swapItem");
		animBoolCanAttack = Animator.StringToHash("canAttack");
		animBoolCanDefend = Animator.StringToHash("canDefend");

		//animIntBells = Animator.StringToHash("bells");
		//animIntWeapons = Animator.StringToHash("weapons");

		weapons = hand.GetComponentsInChildren<Weapon>();

	}

	protected void Update()
	{
		if (!stats.HasWeapons())
			return;

		UpdateWeaponSwapInput();


		//TryWieldUpdate();
	}

	protected void UpdateWeaponSwapInput()
	{
		if (Time.time < nextWeaponSwapTime)
			return;

		if (swapping)
			return;

		//if (!stats.HasWeapons())
		//return;

		if (input.scroll == 0) {
			return;
		}

		//Debug.Log("Scroll Wheel input detected! Trying to swap...");

		if (!weapons[GetNextWeapon()].CanWield())
			return;

		animator.SetTrigger(animTriggerSwap);
		swapping = true;

		//nextWeaponSwapTime = Time.time + 1.0f;
		nextWeaponSwapTime = Time.time + weaponTimeout;
	}

	public void OnPrimaryAction()
	{
		if (swapping)
			return;

		if (currentWeapon == null)
			return;

		currentWeapon.OnPrimaryFire();
	}

	public void Attack()
	{
		animator.SetTrigger(animTriggerAttack);
	}

	public void Defend()
	{
		animator.SetTrigger(animTriggerDefend);
	}

	public void OnWeaponSwap()
	{
		//Debug.Log("Switching weapons!");
		swapping = false;

		if (currentWeapon != null) {
			//Debug.LogFormat("Attempting to unwield {0}", currentWeapon);
			currentWeapon.Unwield();
		}

		int nextWeapon = GetNextWeapon();

		//Debug.LogFormat("CurrentIndex: {0}, Trying to wield index {1}", nextWeapon);

		weapons[nextWeapon].Wield();
		currentWeapon = weapons[nextWeapon];
		currentWeaponIndex = nextWeapon;
	}

	protected int GetNextWeapon()
	{
		if (currentWeaponIndex == 0)
			return 1;
		else
			return 0;
	}


	

	// Plays animation to extend hand
	public void FoundItem()
	{
		animator.SetTrigger(animTriggerFoundItem);
	}

	// Event played when hand is fully extended, forwarded to PlayerStats.cs
	// such that whatever we're picking up can be added to the inventory
	public void PickupItem()
	{
		stats.PickupItem();
	}

	public void AnimEventOnBellRung()
	{
		stats.LoseBell();

		UpdateItemsState();
		//UpdateItemsStateAndWield();

		GameManager.Manager.TryStartBell();
		BellRing();
	}

	public void BellRing()
	{
		//Debug.Log("Ringing bell!");
		currentWeapon.SendMessage("BellRing", SendMessageOptions.DontRequireReceiver);
	}

	// Plays animation to carry an item
	public void CarryingItem(bool state = true)
	{
		animator.SetBool(animBoolHasItem, state);
	}

	// Called by PlayerStats after an item is successfully added to the inventory
	// Called when entering HoldingItem state

	public void UpdateItemsState()
	{
		animator.SetBool(animBoolHasItem, stats.HasWeapons());
		animator.SetBool(animBoolCanAttack, stats.HasSickle);
		animator.SetBool(animBoolCanDefend, stats.HasBell);
	}

	protected void WieldNextWeapon()
	{
		if (!stats.HasWeapons()) {
			//Debug.Log("No weapons, can't weild next weapon!");
			return;
		}

		if (stats.HasSickle) {
			currentWeapon = weapons[0];
			currentWeaponIndex = 0;
		} else if (stats.HasBell) {
			currentWeapon = weapons[1];
			currentWeaponIndex = 1;
		}

		currentWeapon.Wield();

	}

	public void UpdateItemsStateAndWield()
	{
		UpdateItemsState();


		if (currentWeapon == null) {
			//Debug.Log("Don't have a weapon, trying to wield next one");
			WieldNextWeapon();
		} else if (!currentWeapon.CanWield()) {
			//Debug.Log("Can't wield our current weapon, trying to wield next one");

			currentWeapon.Unwield();

			WieldNextWeapon();

		} else {
			//Debug.Log("Trying to wield our current weapon!");
			currentWeapon.Wield();
		}
	}
}

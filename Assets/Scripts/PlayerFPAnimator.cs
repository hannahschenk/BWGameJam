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
	//protected int _CurrentWeapon = 0;
	//protected int CurrentWeapon
	//{
	//	get
	//	{
	//		return _CurrentWeapon;
	//	}
	//	set
	//	{
	//		_CurrentWeapon = Mathf.Clamp(value, 0, weapons.Length);
	//	}
	//}
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

		//weapons.AddRange(hand.GetComponentInChildren<Weapon>());

		weapons = hand.GetComponentsInChildren<Weapon>();
		//MaxWeapons = weapons.Length;

		//for (int i = 0; i < weapons.Length; i++) {
		//	weapons[i].id = i;
		//}
		//Debug.LogFormat("{0} weapons!", weapons.Length);

		//Weapon[] ws = hand.GetComponentsInChildren<Weapon>();
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

		//int nextWeapon = currentWeaponIndex + 1;
		//Debug.LogFormat("CurrentIndex: {0}, attempting to wield index {1}", currentWeaponIndex, nextWeapon);

		//if (nextWeapon < 0)
		//	nextWeapon = (weapons.Length - 1);
		//if (nextWeapon >= weapons.Length)
		//	nextWeapon = 0;

		//Debug.LogFormat("Clamped wield index to {0}", nextWeapon);

		//return nextWeapon;
	}


	//protected void UpdateWeaponSwapInput()
	//{
	//	//if (Time.time < nextWeaponSwapTime)
	//	//	return;
	//	if (Time.time < nextWeaponWieldTime)
	//		return;

	//	//if (!stats.HasWeapons())
	//	//return;

	//	if (input.scroll == 0)
	//		return;

	//	int nextWeapon = Mathf.Max(currentWeaponIndex, 0);

	//	if (input.scroll > 0)
	//		nextWeapon--;
	//	else if (input.scroll < 0)
	//		nextWeapon++;

	//	if (nextWeapon < 0)
	//		nextWeapon = (weapons.Length - 1);
	//	if (nextWeapon >= weapons.Length)
	//		nextWeapon = 0;

	//	if (currentWeaponIndex == nextWeapon) // same weapon, do nothing
	//		return;
	//	Debug.LogFormat("Trying to unwield weapon {0}", currentWeaponIndex);

	//	if (currentWeaponIndex >= 0) {
	//		weapons[currentWeaponIndex].Unwield();
	//	}
	//	currentWeapon = null;

	//	currentWeaponIndex = nextWeapon;

	//	//nextWeaponSwapTime = Time.time + weaponTimeout;
	//	nextWeaponWieldTime = Time.deltaTime + weaponTimeout;

	//	// When you gain a weapon, how does it know which ID to check?


	//}

	protected void TryWieldUpdate()
	{
		if (Time.time < nextWeaponWieldTime)
			return;

		if (currentWeaponIndex >= 0 && weapons[currentWeaponIndex] != currentWeapon) {

			Debug.LogFormat("Trying to wield weapon {0}", currentWeaponIndex);
			weapons[currentWeaponIndex].Wield();
			currentWeapon = weapons[currentWeaponIndex];
			//weapons[CurrentWeapon].TryWield();
			nextWeaponWieldTime = Time.deltaTime + weaponTimeout;

		}

		if (currentWeaponIndex < 0 && HasItem) {
			CarryingItem(false);
		}

		// Wield the current weapon
		//if (CurrentWeapon < 0) { // Nothing wielded
		//	CurrentWeapon = weapons[0].id;
		//	weapons[0].Wield();
		//}
	}


	//protected void Update()
	//{
	//	UpdateWeaponSwapInput();

	//	//Debug.LogFormat("CurrentWeapon: {0}", CurrentWeapon);

	//	TryWieldWeapon();
	//}

	//protected void UpdateWeaponSwapInput()
	//{
	//	if (!stats.HasWeapons())
	//		return;

	//	Debug.Log("Has weapons!");

	//	if (Time.time < nextWeaponWieldTime)
	//		return;

	//	//e.g., mouse scroll, weapon int

	//	SwapWeapon(input.scroll);

	//	//if (input.scroll < 0) {
	//	//	//Next weapon
	//	//	Debug.LogFormat("Next weapon!");
	//	//	//CurrentWeapon = CurrentWeapon + 1;

	//	//	if (weapons[CurrentWeapon++].)
	//	//	CurrentWeapon++;
	//	//	TryWieldWeapon();

	//	//	nextWeaponWieldTime = Time.time + weaponTimeout;

	//	//} else if (input.scroll > 0) {
	//	//	//Previous weapon
	//	//	Debug.LogFormat("Previous weapon!");
	//	//	//CurrentWeapon = CurrentWeapon - 1;
	//	//	CurrentWeapon--;
	//	//	TryWieldWeapon();
	//	//	nextWeaponWieldTime = Time.time + weaponTimeout;
	//	//}

	//	//if scroll
	//	// newWeapon = currentWeapon % weapons.Count
	//	// TryWield(newWeapon)
	//}

	//protected void SwapWeapon(float direction)
	//{
	//	if (direction == 0f)
	//		return;

	//	//int weaponIndex = 0;

	//	int nextWeapon = CurrentWeapon;

	//	if (direction > 0)
	//		nextWeapon--;
	//	else if (direction < 0)
	//		nextWeapon++;

	//	if (nextWeapon < 0)
	//		nextWeapon = (weapons.Length - 1);
	//	if (nextWeapon >= weapons.Length)
	//		nextWeapon = 0;

	//	//if (direction > 0)
	//	//	weaponIndex--;
	//	//else if (direction < 0)
	//	//	weaponIndex++;

	//	//int nextWeapon = CurrentWeapon + weaponIndex;
	//	Debug.LogFormat("Currently on Weapon {0}, trying to swap to {1}!", CurrentWeapon, nextWeapon);

	//	if (CurrentWeapon == nextWeapon && weapons[CurrentWeapon].Wielded)
	//		return;

	//	if (weapons[nextWeapon].CanWield()) {
	//		Debug.Log("Can wield next weapon");

	//		weapons[CurrentWeapon].Unwield();
	//		weapons[nextWeapon].Wield();

	//		nextWeaponWieldTime = Time.time + weaponTimeout;
	//	} else {
	//		Debug.Log("Cannot wield next weapon");
	//	}


	//}

	//protected void TryWieldWeapon()
	//{
	//	//weapons[CurrentWeapon].TryWield();
	//	Debug.LogFormat("Trying to Wield Weapon {0}", CurrentWeapon);
	//	anim.SetBool(animBoolHasItem, weapons[CurrentWeapon].TryWield());
	//}

	//protected void UnWieldWeapon()
	//{
	//	weapons[CurrentWeapon].Unwield();
	//}

	//protected void TryNextWeapon()
	//{

	//}

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
		//Debug.Log("AnimEventOnBellRung!");
		//stats.LoseBell();
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
	public void UpdateItemsState()
	{
		//if (anim.GetBool(animBoolHasItem)) {
		//	return;
		//}

		// Updates the animator variables (so the player's animations can change accordingly)
		animator.SetBool(animBoolHasItem, (stats.HasBell || stats.HasSickle));
		animator.SetBool(animBoolCanAttack, stats.HasSickle);
		animator.SetBool(animBoolCanDefend, stats.HasBell);


		if (currentWeapon == null || !currentWeapon.CanWield()) {
			if (stats.HasSickle) {
				currentWeapon = weapons[0];
				currentWeaponIndex = 0;
			} else if (stats.HasBell) {
				currentWeapon = weapons[1];
				currentWeaponIndex = 1;
			}
			//Debug.LogFormat("UpdatingItemState: HasBell: {0}, HasSickle: {1}, attempting to wield {2} index of {3}", stats.HasBell, stats.HasSickle, currentWeapon, currentWeaponIndex);
			currentWeapon.Wield();
			//currentWeapon.UpdateHeldState();
		}
		//bool hasItem = false;

		//anim.SetBool(animBoolHasItem, weapons[CurrentWeapon].CanWield());


		//Updates the weapon states
		//for (int i = 0; i < weapons.Length; i++) {
		//	weapons[i].UpdateHeldState();
		//}
	}

	//public void UpdateSickle(bool state)
	//{
	//	UpdateItemsState();
	//}

	//public void UpdateBell(bool state)
	//{
	//	UpdateItemsState();
	//}
}

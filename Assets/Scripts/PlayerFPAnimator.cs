using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFPAnimator : MonoBehaviour
{
	protected PlayerStats stats;
	protected Animator anim;

	protected int animTriggerFoundItem;
	protected int animBoolHasItem;
	protected int animTriggerAttack;
	protected int animTriggerDefend;
	//protected int animIntBells;
	//protected int animIntWeapons;

	public Transform hand;
	//protected Dictionary<Transform, Weapon> weapons = new Dictionary<Transform, Weapon>();
	protected Weapon[] weapons;


	protected int currentWeapon = 0;


	// Start is called before the first frame update
	void Start()
    {
		anim = GetComponent<Animator>();
		stats = GameManager.PlayerStats;
		CacheAnimReferences();
    }

	protected void CacheAnimReferences()
	{

		animTriggerFoundItem = Animator.StringToHash("foundItem");
		animBoolHasItem = Animator.StringToHash("hasItem");
		animTriggerAttack = Animator.StringToHash("Attack");
		animTriggerDefend = Animator.StringToHash("Defend");
		//animIntBells = Animator.StringToHash("bells");
		//animIntWeapons = Animator.StringToHash("weapons");

		//weapons.AddRange(hand.GetComponentInChildren<Weapon>());

		weapons = hand.GetComponentsInChildren<Weapon>();

		//Weapon[] ws = hand.GetComponentsInChildren<Weapon>();
		//foreach (Weapon w in ws) {
		//	weapons.Add(w.transform, w);
		//}

	}

	//protected void Update()
	//{
	//	UpdateWeaponInput();
	//}

	protected void UpdateWeaponInput()
	{
		//e.g., mouse scroll, weapon int

		//if scroll
		// newWeapon = currentWeapon % weapons.Count
		// TryWield(newWeapon)
	}

	// Plays animation to extend hand
	public void FoundItem()
	{
		anim.SetTrigger(animTriggerFoundItem); ;
	}

	// Event played when hand is fully extended, forwarded to PlayerStats.cs
	// such that whatever we're picking up can be added to the inventory
	public void PickupItem()
	{
		stats.PickupItem();
	}

	// Called by PlayerStats after an item is successfully added to the inventory
	public void UpdateItemsState()
	{
		// Updates the animator variables (so the player's animations can change accordingly)
		//anim.SetBool(animBoolHasItem, (stats.HasBell || stats.HasSickle));

		//bool hasItem = false;

		anim.SetBool(animBoolHasItem, weapons[currentWeapon].CanWield());





		// Updates the weapon states
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

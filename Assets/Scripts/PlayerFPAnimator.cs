using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFPAnimator : MonoBehaviour
{
	protected PlayerStats stats;
	protected Animator anim;

	protected int animTriggerFoundItem;
	protected int animBoolHasItem;
	protected int animIntBells;
	protected int animIntWeapons;

	public Transform hand;
	//protected Dictionary<Transform, Weapon> weapons = new Dictionary<Transform, Weapon>();
	protected Weapon[] weapons;


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
		animIntBells = Animator.StringToHash("bells");
		animIntWeapons = Animator.StringToHash("weapons");

		//weapons.AddRange(hand.GetComponentInChildren<Weapon>());

		weapons = hand.GetComponentsInChildren<Weapon>();

		//Weapon[] ws = hand.GetComponentsInChildren<Weapon>();
		//foreach (Weapon w in ws) {
		//	weapons.Add(w.transform, w);
		//}

	}

	// Plays animation to extend hand
	public void FoundItem()
	{
		anim.SetTrigger(animTriggerFoundItem); ;
	}

	// Event played when hand is fully extended, forwarded to PlayerStats.cs
	public void PickupItem()
	{
		stats.PickupItem();
	}

	// Called by PlayerStats after an item is successfully added to the inventory
	public void UpdateItemsState()
	{
		// Updates the animator variables (so the player's animations can change accordingly)
		anim.SetBool(animBoolHasItem, (stats.HasBell || stats.HasSickle));

		// Updates the weapon states
		for (int i = 0; i < weapons.Length; i++) {
			weapons[i].UpdateHeldState();
		}

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

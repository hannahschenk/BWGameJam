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
	}

	// Update is called once per frame
	//void Update()
 //   {
        
 //   }

	public void FoundItem()
	{
		anim.SetTrigger(animTriggerFoundItem); ;
	}

	public void PickupItem()
	{
		stats.PickupItem();
	}

	public void UpdateItemsState()
	{
		anim.SetBool(animBoolHasItem, (stats.HasBell || stats.HasSickle) );
	}

	public void UpdateSickle(bool state)
	{
		anim.SetInteger(animIntWeapons, state ? 1 : 0);
		UpdateItemsState();
	}

	public void UpdateBell(bool state)
	{
		anim.SetInteger(animIntBells, state ? 1 : 0);
		UpdateItemsState();
	}
}

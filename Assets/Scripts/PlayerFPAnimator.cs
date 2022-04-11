using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFPAnimator : MonoBehaviour
{
	protected PlayerStats stats;
	protected Animator anim;

	protected int animTriggerFoundItem;
	protected int animBoolHasItem;
	protected int animFloatBells;
	protected int animFloatWeapons;


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
		animFloatBells = Animator.StringToHash("bells");
		animFloatWeapons = Animator.StringToHash("weapons");
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
}

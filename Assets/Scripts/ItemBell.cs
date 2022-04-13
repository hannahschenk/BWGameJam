using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBell : PickableItem
{
	public override bool TryInteract()
	{
		return !stats.HasBell;
	}

	public override void OnPickup()
	{
		base.OnPickup();

		stats.GainBell();

		Die();
	}
}

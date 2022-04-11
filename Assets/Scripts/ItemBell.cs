using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBell : PickableItem
{
	public override void TryInteract()
	{
		if (!stats.TryGetBell())
			return;

		stats.GainBell();

		Die();
	}
}

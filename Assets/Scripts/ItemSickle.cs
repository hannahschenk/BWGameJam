using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSickle : PickableItem
{
	public override void TryInteract()
	{
		if (!stats.TryGetSickle())
			return;

		stats.GainSickle();

		Die();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHealthKit : PickableItem
{

	public float healAmount = 50f;

	public override void TryInteract()
	{
		if (!stats.TryGetKit())
			return;

		stats.GetKit();

		stats.Heal(healAmount);
		Die();
	}
}

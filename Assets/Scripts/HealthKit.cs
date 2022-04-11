using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthKit : PickableItem
{

	public float healAmount = 50f;

	protected override void OnPickup()
	{
		if (!stats.TryGetKit())
			return;

		stats.Heal(healAmount);
		Die();
	}


}

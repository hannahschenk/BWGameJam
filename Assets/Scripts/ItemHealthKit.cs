using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHealthKit : PickableItem
{

	public float healAmount = 50f;

	public override bool TryInteract()
	{
		//if (stats.Health < stats.MaxHealth)
		//return;

		return stats.Health < stats.MaxHealth;
	}

	public override void OnPickup()
	{
		stats.Heal(healAmount);
		Die();
	}
}

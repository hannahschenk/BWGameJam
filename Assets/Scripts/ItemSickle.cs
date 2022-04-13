using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSickle : PickableItem
{
	public override bool TryInteract()
	{

		return !stats.HasSickle;

	}

	public override void OnPickup()
	{
		base.OnPickup();

		stats.GainSickle();

		Die();

	}
}

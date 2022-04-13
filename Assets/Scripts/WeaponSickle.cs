using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSickle : Weapon
{

	public override bool CanWield()
	{
		return stats.HasSickle;
	}

	public override void OnPrimaryFire()
	{
		if (CanWield())
			FPAnimHandler.Attack();
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSickle : Weapon
{

	public override bool CanWield()
	{
		return stats.HasSickle;
	}

}

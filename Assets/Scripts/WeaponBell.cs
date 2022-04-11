using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBell : Weapon
{
	public override bool CanWield()
	{
		return stats.HasBell;
	}
}

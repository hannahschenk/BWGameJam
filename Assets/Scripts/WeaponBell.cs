using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBell : Weapon
{
	public override bool CanWield()
	{
		return stats.HasBell;
	}

	public override void OnPrimaryFire()
	{
		if (CanWield())
			FPAnimHandler.Defend();
	}

	public void BellRing()
	{
		PlayRandomAudioClip();
	}

	public override void AttackAbilityBegin()
	{
		//FPAnimHandler.UpdateItemsState();
		//UpdateItemsStateAndWield();

		GameManager.Manager.TryStartBell();
		BellRing();
	}

	public override void AttackAbilityFinish()
	{

		stats.LoseBell();
		FPAnimHandler.UpdateItemsState();
		//FPAnimHandler.UpdateItemsStateAndWield();

	}
}

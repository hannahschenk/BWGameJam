using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSickle : Weapon
{

	protected bool isAttacking = false;

	protected override void Start()
	{
		base.Start();

		//DisablePlayerCollisions();
	}

	//protected void DisablePlayerCollisions()
	//{
	//	//Collider[] colliders = GetComponentsInChildren<Collider>();
	//	MeshCollider[] colliders = GetComponentsInChildren<MeshCollider>();

	//	Debug.Log("Trying to disable collisions...");
	//	foreach (MeshCollider col in colliders) {
	//		Debug.Log("Disabling collisions between col {0} and player capsule");
	//		Physics.IgnoreCollision(GameManager.PlayerController.capsule, col);
	//	}
	//}

	public override bool CanWield()
	{
		return stats.HasSickle;
	}

	public override void OnPrimaryFire()
	{
		if (!CanWield())
			return;

		FPAnimHandler.Attack();
		isAttacking = true;
		Debug.LogFormat("Attacking!");
		//Debug.Break();
	}

	public virtual void OnAttackEnd()
	{
		isAttacking = false;
		Debug.Log("No longer attacking!");
	}

	public virtual void OnCollisionStay(Collision collision)
	//public virtual void OnCollisioinEnter(Collision collision)
	{
		if (!isAttacking) {
			Debug.Log("Sickle cannot respond to this collision, not attacking!");
			return;
		}

		Collider col = collision.collider;

		if (!col) {
			Debug.Log("No collider on this collision!");
			return;
		}

		if (!col.CompareTag("Enemy")) {
			Debug.LogFormat("Collided into {0}, not an Enemy!", col);
			return;
		}

		Enemy e = col.GetComponentInParent<Enemy>();

		if (!e)
			return;

		Debug.Log("Killing enemy!");
		e.Kill();

	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	protected GameObject child;
	protected PlayerStats stats;

    // Start is called before the first frame update
    protected virtual void Start()
    {
		child = transform.GetChild(0).gameObject;
		stats = GameManager.PlayerStats;
		UpdateHeldState();
    }

	public virtual void UpdateHeldState()
	{
		child.SetActive(CanWield());
	}

	// e.g., what parameters constrain that we have this item?
	public virtual bool CanWield()
	{
		return false;
	}

	// What does this weapon do when called?
	public virtual void OnPrimaryFire()
	{

	}

}

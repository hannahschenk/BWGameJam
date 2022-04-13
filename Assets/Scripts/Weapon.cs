using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	protected GameObject child;
	protected PlayerStats stats;

	protected PlayerFPAnimator FPAnimHandler;
	protected Animator playerAnimator;

	public int id = 0;

	protected bool _Wielded;
	public bool Wielded
	{
		get
		{
			return _Wielded;
		}
		protected set
		{
			_Wielded = value;
		}
	}

    // Start is called before the first frame update
    protected virtual void Start()
    {
		child = transform.GetChild(0).gameObject;
		stats = GameManager.PlayerStats;
		FPAnimHandler = GameManager.PlayerFPAnimHandler;
		playerAnimator = GameManager.PlayerAnimator;
		UpdateHeldState();
    }

	public virtual bool TryWield()
	{
		Wielded = CanWield();

		//Debug.LogFormat("CanWield {0}? {1}", gameObject, canWield);

		if (child.activeInHierarchy != Wielded) {
			Wield();
			//UpdateHeldState();
		}

		return Wielded;
	}

	public virtual void Unwield()
	{
		FPAnimHandler.CarryingItem(false);
		child.SetActive(false);
		Wielded = false;
	}

	public virtual void Wield()
	{
		FPAnimHandler.CarryingItem();
		child.SetActive(true);
		Wielded = true;
	}

	public virtual void UpdateHeldState()
	{
		child.SetActive(CanWield());
		//Wield();
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

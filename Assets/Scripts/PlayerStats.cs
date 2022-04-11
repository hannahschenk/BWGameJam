using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

	public string PlayerTag = "Player";
	protected Transform playerCam;

	protected float interactReach = 2.5f;

	protected float MaxHealth = 100f;
	
	public float Health
	{
		get
		{
			return _health;
		}
		set
		{
			_health = Mathf.Clamp(value, 0f, MaxHealth);
		}
	}
	protected float _health = 100f;

	protected bool _Sickle = false;
	public bool HasSickle
	{
		get
		{
			return _Sickle;
		}
		protected set
		{
			_Sickle = value;
		}
	}

	protected bool _Bell = false;
	public bool HasBell
	{
		get
		{
			return _Bell;
		}
		protected set
		{
			_Bell = value;
		}
	}

	//protected Dictionary<>

	//protected bool _Kit = false;
	//public bool HasKit
	//{
	//	get
	//	{
	//		return
	//	}
	//}
	
	//public bool HasSickle = false;
	//public bool HasBell = false;



	void Awake()
	{
		Health = MaxHealth;
	}

    // Start is called before the first frame update
    void Start()
    {
		playerCam = GameManager.PlayerCam.transform;
    }

	// Update is called once per frame
	//void Update()
	//{

	//}

	private void FixedUpdate()
	{
		CheckForInteractables();
	}

	protected void CheckForInteractables()
	{
		Ray ray = new Ray(playerCam.position, playerCam.forward);
		if (!Physics.Raycast(ray, out RaycastHit hitInfo, interactReach))
			return;



	}

	public void Heal(float healAmount)
	{
		Health += healAmount;
	}

	public void Hurt(float hurtAmount)
	{
		Health -= hurtAmount;

		if (Health <= 0f) {
			Die();
		}
	}

	public void GainSickle()
	{
		HasSickle = true;
	}

	public void LoseSickle()
	{
		HasSickle = false;
	}

	public void GainBell()
	{
		HasBell = true;
	}

	public void LoseBell()
	{
		HasBell = false;
	}

	public bool TryGetSickle()
	{
		return !HasSickle;
	}

	public bool TryGetKit()
	{
		Debug.Log("Trying to pick up health kit");
		return (Health < MaxHealth);
	}

	public bool TryGetBell()
	{
		return !HasBell;
	}

	public void Die()
	{
		// Play anim, etc
		// Reset, etc
	}
}

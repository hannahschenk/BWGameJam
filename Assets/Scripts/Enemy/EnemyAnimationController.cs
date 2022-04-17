using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
	protected Animator animator;

	public float walkingThreshold = 3f;
	public float runningThreshold = 6f;
	protected Vector3 velocity = Vector3.zero;

	protected int animBoolIsWalking;
	protected int animBoolIsRunning;
	protected int animBoolIsIdle;
	protected int animBoolDetectsPlayer;
	protected int animBoolTookHit;
	protected int animBoolCanAttackPlayer;
	protected int animBoolIsDead;
	//protected int animTriggerBellRung;
	protected int animBoolBellRung;

	protected new Rigidbody rigidbody;

	public bool IsWalking
	{
		get
		{
			return animator.GetBool(animBoolIsWalking);
		}
		protected set
		{
			animator.SetBool(animBoolIsWalking, value);
		}
	}

	public bool IsRunning
	{
		get
		{
			return animator.GetBool(animBoolIsRunning);
		}
		protected set
		{
			animator.SetBool(animBoolIsRunning, value);
		}
	}

	public bool IsIdle
	{
		get
		{
			return animator.GetBool(animBoolIsIdle);
		}
		protected set
		{
			animator.SetBool(animBoolIsIdle, value);
		}
	}

	public bool DetectsPlayer
	{
		get
		{
			return animator.GetBool(animBoolDetectsPlayer);
		}
		protected set
		{
			animator.SetBool(animBoolDetectsPlayer, value);
		}
	}

	public bool TookHit
	{
		get
		{
			return animator.GetBool(animBoolTookHit);
		}
		protected set
		{
			animator.SetBool(animBoolTookHit, value);
		}
	}

	public bool CanAttackPlayer
	{
		get
		{
			return animator.GetBool(animBoolCanAttackPlayer);
		}
		protected set
		{
			animator.SetBool(animBoolCanAttackPlayer, value);
		}
	}

	public bool IsDead
	{
		get
		{
			return animator.GetBool(animBoolIsDead);
		}
		protected set
		{
			animator.SetBool(animBoolIsDead, value);
		}
	}

	public bool IsBellActive
	{
		get
		{
			return animator.GetBool(animBoolBellRung);
		}
		protected set
		{
			animator.SetBool(animBoolBellRung, value);
		}
	}

	//public void BellRung()
	//{
	//	animator.SetTrigger(animTriggerBellRung);
	//}

	private void Awake()
	{
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody>();

		CacheAnimReferences();
	}

	// Start is called before the first frame update
	void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
		if (TookHit || IsDead)
			return;

		CheckBellState();
		CheckMovement();
    }

	protected void CheckBellState()
	{
		IsBellActive = GameManager.Manager.isBellActive;
	}

	protected void CheckMovement()
	{
		velocity = rigidbody.velocity;
		float sqrVel = velocity.sqrMagnitude;


		//IsIdle = sqrVel <= 0f;
		//IsWalking = sqrVel <= (walkingThreshold * walkingThreshold);
		//IsRunning = sqrVel <= (runningThreshold * runningThreshold);

		if (sqrVel <= 0f) {
			IsIdle = true;
			IsWalking = IsRunning = false;
		} else if (sqrVel <= walkingThreshold * walkingThreshold) {
			IsWalking = true;
			IsIdle = IsRunning = false;
		} else {
			IsRunning = true;
			IsIdle = IsWalking = false;
		}


	}

	protected void CacheAnimReferences()
	{
		animBoolIsWalking = Animator.StringToHash("isWalking");
		animBoolIsRunning = Animator.StringToHash("isRunning");
		animBoolIsIdle = Animator.StringToHash("isIdle");
		animBoolDetectsPlayer = Animator.StringToHash("detectsPlayer");
		animBoolTookHit = Animator.StringToHash("tookHit");
		animBoolCanAttackPlayer = Animator.StringToHash("canAttackPlayer");
		animBoolIsDead = Animator.StringToHash("isDead");
		//animTriggerBellRung = Animator.StringToHash("bellRung");
		animBoolBellRung = Animator.StringToHash("bellActive");
	}

	public void Kill()
	{
		TookHit = true;
		//IsDead = true;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
	protected Animator animator;

	protected int animBoolIsWalking;
	protected int animBoolIsRunning;
	protected int animBoolIsIdle;
	protected int animBoolDetectsPlayer;
	protected int animBoolTookHit;
	protected int animBoolCanAttackPlayer;
	protected int animBoolIsDead;
	//protected int animTriggerBellRung;
	protected int animBoolBellRung;

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

		CacheAnimReferences();
	}

	// Start is called before the first frame update
	void Start()
    {
		IsIdle = true;
    }

    // Update is called once per frame
    void Update()
    {
		UpdateAnimstate();
    }

	protected void UpdateAnimstate()
	{
		IsBellActive = GameManager.Manager.isBellActive;
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
}

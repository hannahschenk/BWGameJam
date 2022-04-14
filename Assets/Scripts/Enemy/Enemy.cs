using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

	public Vector2 pitchRange = new Vector2(0.9f, 1.1f);
	public List<AudioClip> stabSfx = new List<AudioClip>();

	protected EnemyAnimationController enemyController;
	protected Animator animator;
	protected CapsuleCollider capsule;
	protected AudioSource audioSource;

	private void Awake()
	{
		capsule = GetComponentInChildren<CapsuleCollider>();
		enemyController = GetComponentInChildren<EnemyAnimationController>();
		animator = GetComponentInChildren<Animator>();
		audioSource = GetComponentInChildren<AudioSource>();

	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Kill()
	{
		if (enemyController.IsDead)
			return;

		if (enemyController.TookHit)
			return;

		PlayHurtSfx();
		enemyController.Kill();
		capsule.enabled = false;
	}

	protected void PlayHurtSfx()
	{

		Vector3 hitPos = capsule.center;

		ap_Helper.GetRandomAudioClip(stabSfx, pitchRange, out AudioClip clip, out float pitch);
		ap_Helper.PlayClipAt(audioSource, hitPos, clip);

	}
}

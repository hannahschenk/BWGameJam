using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{

	public Vector2 pitchRange = new Vector2(0.9f, 1.1f);
	public List<AudioClip> sfx = new List<AudioClip>();

	protected PlayerStats stats;
	protected AudioSource _audio;

    // Start is called before the first frame update
    protected virtual void Start()
    {
		stats = GameManager.PlayerStats;
		_audio = GetComponent<AudioSource>();
    }

    //// Update is called once per frame
    //protected virtual void Update()
    //{
        
    //}

	public virtual bool TryInteract()
	{
		return false;
	}

	public virtual void OnPickup()
	{
		//Debug.Log("Playing pickup audio");
		//ap_Helper.PlayRandomAudioClip(_audio, sfx, pitchRange);

		if (!ap_Helper.GetRandomAudioClip(sfx, pitchRange, out AudioClip clip, out float pitch))
			return;

		ap_Helper.PlayClipAt(_audio, transform.position, clip);
	}

	//protected virtual void OnCollisionEnter(Collision col)
	//{

	//	Debug.LogFormat("Col.collider: {0}, Tag: {1}", col.collider, col.collider.tag);

	//	if (!col.collider.CompareTag(stats.PlayerTag))
	//		return;

	//	TryInteract();
	//}

	protected virtual void Die()
	{
		//Debug.Log("Skipping item destroy for testing");
		GameObject.Destroy(gameObject);
	}
}

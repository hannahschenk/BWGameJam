using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{

	protected PlayerStats stats;

    // Start is called before the first frame update
    protected virtual void Start()
    {
		stats = GameManager.PlayerStats;
    }

    //// Update is called once per frame
    //protected virtual void Update()
    //{
        
    //}

	protected virtual void TryInteract()
	{
		OnPickup();
	}

	protected virtual void OnCollisionEnter(Collision col)
	{
		if (!col.collider.CompareTag(stats.PlayerTag))
			return;

		OnPickup();
	}

	protected virtual void OnPickup()
	{

	}

	protected virtual void Die()
	{
		GameObject.Destroy(this);
	}
}

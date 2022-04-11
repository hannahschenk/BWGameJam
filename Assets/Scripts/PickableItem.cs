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

	public virtual void TryInteract()
	{

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
		GameObject.Destroy(gameObject);
	}
}

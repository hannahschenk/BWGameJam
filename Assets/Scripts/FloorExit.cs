using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorExit : MonoBehaviour
{
	public int currentFloor = 1;

	//protected bool canExit = true;
	//protected int exit = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	//void OnTriggerEnter(Collider col)
	void OnTriggerEnter(Collider col)
	{
		if (!col.CompareTag(GameManager.PlayerTag))
			return;

		//if (exit > 0)
		//		return;
		//exit++;
		//if (!canExit)
		//return;

		//canExit = false;
		Debug.LogFormat("{0} hit Exit Trigger, on object instance ID {1}", col, gameObject.GetInstanceID());
		if (!GameManager.Manager.CanStartChangeFloors()) {
			Debug.LogFormat("Instance ID {0} cannot change floors", gameObject.GetInstanceID());
			return;
		}
		Debug.LogFormat("Instance ID {0} is trigger floor change", gameObject.GetInstanceID());
		Debug.Log("Changing floors: Incrementing Current Floor, Initiating Level Generation, Warping Player...");

		GameManager.Player.SetPositionAndRotation(Vector3.zero + (GameManager.Apartment.GetFloorHeight(GameManager.CurrentFloor) * Vector3.up), Quaternion.FromToRotation(GameManager.Player.forward, Vector3.forward));

		GameManager.CurrentFloor = currentFloor + 2;
		GameManager.Apartment.NewLevel();

	}

	//void OnTriggerStay(Collider col)
	//{
	//	Debug.Log("OTS!");

	//	if (!col.CompareTag(GameManager.PlayerTag))
	//		return;

	//}
}

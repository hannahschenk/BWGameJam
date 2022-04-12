using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorExit : MonoBehaviour
{
	public int currentFloor = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnTriggerEnter(Collider col)
	{
		if (!col.CompareTag(GameManager.PlayerTag))
			return;

		if (!GameManager.Manager.CanStartChangeFloors())
			return;

		GameManager.CurrentFloor = currentFloor + 2;
		GameManager.Apartment.NewLevel();
		GameManager.Player.SetPositionAndRotation(Vector3.zero + (GameManager.Apartment.GetFloorHeight(GameManager.CurrentFloor) * Vector3.up), Quaternion.FromToRotation(GameManager.Player.forward, Vector3.forward));

	}

	//void OnTriggerStay(Collider col)
	//{
	//	Debug.Log("OTS!");

	//	if (!col.CompareTag(GameManager.PlayerTag))
	//		return;

	//}
}

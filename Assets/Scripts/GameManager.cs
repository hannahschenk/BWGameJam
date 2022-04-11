using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class for important references in-game; only inquire in START or later.
/// </summary>
public class GameManager : MonoBehaviour
{

	public static GameManager Manager;
	public static PlayerStats PlayerStats;
	public static Camera PlayerCam;

	public bool isBellActive = false;
	protected float bellEndTime = 0f;

	private void Awake()
	{
		Manager = this;
		PlayerStats = FindObjectOfType<PlayerStats>() as PlayerStats;

		PlayerCam = PlayerStats.GetComponentInChildren<Camera>(); //old way that has to deal with multiple cameras, don't want to raycast from the scene camera if we have one lol
	}

	private void Update()
	{
		if (isBellActive) {
			if (Time.time >= bellEndTime)
				isBellActive = false;
		}
	}

	public bool TryStartBell()
	{
		if (isBellActive)
			return false;

		return true;
	}

	public void StartBell(float duration)
	{
		isBellActive = true;
		bellEndTime = Time.time + duration;
	}

	// Start is called before the first frame update
	//void Start()
	//   {
	//	//Debug.LogFormat("Player's Health is {0}", PlayerStats.Health);
	//   }

	// Update is called once per frame
	//void Update()
	//{

	//}
}

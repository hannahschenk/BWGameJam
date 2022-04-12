using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class for important references in-game; only inquire in START or later.
/// </summary>
public class GameManager : MonoBehaviour
{

	public static ApartmentBuilder Apartment;
	public static GameManager Manager;
	public static Transform Player;
	public static PlayerStats PlayerStats;
	public static Camera PlayerCam;
	public static PlayerInputHandler PlayerInputHandler;
	public static PlayerFPAnimator PlayerFPAnim;

	public static string PlayerTag = "Player";

	public bool isBellActive = false;
	protected float bellEndTime = 0f;

	public static int CurrentFloor = 1;

	//public string EventLowIntensity = "Low_Intensity";
	//public string EventHighIntensity = "High_Intensity";
	public string PlayAmbiance = "Play_Ambience";
	public string EventLowIntensity = "Play_Song_EmptyHalls";
	public string EventHighIntensity = "Play_Song_Ghost_Attack";
	//public string EventStopAll = "Stop_All";



	private void Awake()
	{
		Manager = this;
		Apartment = FindObjectOfType<ApartmentBuilder>() as ApartmentBuilder;
		PlayerStats = FindObjectOfType<PlayerStats>() as PlayerStats;
		Player = PlayerStats.transform;
		PlayerInputHandler = PlayerStats.GetComponent<PlayerInputHandler>();
		PlayerFPAnim = PlayerStats.GetComponentInChildren<PlayerFPAnimator>();

		PlayerCam = PlayerStats.GetComponentInChildren<Camera>(); //old way that has to deal with multiple cameras, don't want to raycast from the scene camera if we have one lol
	}

	private void Start()
	{
		BGMLowIntensity();
	}

	private void Update()
	{
		if (isBellActive) {
			if (Time.time >= bellEndTime) {
				EndBell();
			}
				
		}
	}

	protected bool changingFloors = false;
	public bool CanStartChangeFloors()
	{
		if (changingFloors)
			return false;

		Time.timeScale = 0f;
		changingFloors = true;
		return changingFloors;
	}

	public void ChangedFloors()
	{
		changingFloors = false;
		Time.timeScale = 1.0f;
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

	public void EndBell()
	{
		isBellActive = false;
	}

	public void BGMLowIntensity()
	{
		//AkSoundEngine.PostEvent(EventLowIntensity, gameObject);
		AkSoundEngine.PostEvent(PlayAmbiance, gameObject);
	}

	public void BGMHighIntensity()
	{
		AkSoundEngine.PostEvent(EventHighIntensity, gameObject);
	}

	public void BGMStopAll()
	{
		//AkSoundEngine.PostEvent(EventStopAll, gameObject);
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

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

	private void Awake()
	{
		Manager = this;
		PlayerStats = FindObjectOfType<PlayerStats>() as PlayerStats;

		PlayerCam = PlayerStats.GetComponentInChildren<Camera>(); //old way that has to deal with multiple cameras, don't want to raycast from the scene camera if we have one lol
	}

	// Start is called before the first frame update
	void Start()
    {
		Debug.LogFormat("Player's Health is {0}", PlayerStats.Health);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

/// <summary>
/// Helper class for important references in-game; only inquire in START or later.
/// </summary>
public class GameManager : MonoBehaviour
{

	public static bool CanMove = true;

	public static ApartmentBuilder Apartment;
	public static GameManager Manager;
	public static Transform Player;
	public static PlayerStats PlayerStats;
	public static Camera PlayerCam;
	public static PlayerInputHandler PlayerInputHandler;
	public static PlayerFPAnimator PlayerFPAnimHandler;
	public static Animator PlayerAnimator;
	//public static AudioSource Audio;

	public static string PlayerTag = "Player";

	public AudioClip bgmLowIntensity;
	public AudioClip bgmHighIntensity;

	public AudioClip[] ambience;
	protected int ambIndex = 0;

	public bool isBellActive = false;
	protected float bellEndTime = 0f;

	public static int CurrentFloor = 1;

	public AudioSource asMusic;
	protected float musicMaxVolume = 1.0f;

	public AudioSource asAmbience;
	protected float ambienceVolume = 0.25f;

	public Image BlackScreen;
	protected Color colorEmpty = new Color(0f, 0f, 0f, 0f);
	//protected float screenFadeTime = 8f;
	protected float fadeGoalTime = 0f;

	protected float blackScreenTime = 0f;

	protected float musicFadeTime = 10f;
	protected float audioFadeOutGoalTime = -1f;
	protected float audioFadeInGoalTime = -1f;
	protected AudioClip queuedMusic = null;

	public AnimationCurve screenFadeCurve;
	public float screenFadeTime = 6f;
	public float startBlackScreenHoldTime = 4f;

	//public string EventLowIntensity = "Low_Intensity";
	//public string EventHighIntensity = "High_Intensity";
	//public string PlayAmbiance = "Play_Ambience";
	//public string EventLowIntensity = "Play_Song_EmptyHalls";
	//public string EventHighIntensity = "Play_Song_Ghost_Attack";
	//public string EventStopAll = "Stop_All";



	private void Awake()
	{
		Manager = this;
		Apartment = FindObjectOfType<ApartmentBuilder>() as ApartmentBuilder;
		PlayerStats = FindObjectOfType<PlayerStats>() as PlayerStats;
		Player = PlayerStats.transform;
		PlayerInputHandler = PlayerStats.GetComponent<PlayerInputHandler>();
		PlayerFPAnimHandler = PlayerStats.GetComponentInChildren<PlayerFPAnimator>();
		PlayerAnimator = PlayerFPAnimHandler.GetComponent<Animator>();

		PlayerCam = PlayerStats.GetComponentInChildren<Camera>(); //old way that has to deal with multiple cameras, don't want to raycast from the scene camera if we have one lol

		ConfigureAudio();
		ConfigureFade();
	}

	protected void ConfigureFade()
	{
		BlackScreen.color = Color.black;
		BlackScreen.gameObject.SetActive(true);
		//blackScreenTime = Time.time + 10f;
		//Debug.LogFormat("CurrentTime: {0}, StartFadeInAt: {1}", Time.time, blackScreenTime);
	}

	protected void ConfigureAudio()
	{
		asMusic = gameObject.AddComponent<AudioSource>();
		asAmbience = gameObject.AddComponent<AudioSource>();

		asMusic.volume = 0f;
		asMusic.playOnAwake = true;
		asMusic.loop = true;

		asAmbience.playOnAwake = true;
		asAmbience.loop = false;
		asAmbience.volume = ambienceVolume;
	}

	private void Start()
	{
		CanMove = false;

		PlayBGMLowIntensity();
		audioFadeInGoalTime = Time.time + (musicFadeTime);

		PlayAmbience();

		Invoke("FadeIn", startBlackScreenHoldTime);
		Invoke("OnFadeIn", startBlackScreenHoldTime + (screenFadeTime / 3.0f));
		//FadeIn();
		//StartBlackScreen(30f, true);
	}

	protected float ambienceUpdateTime = 1.0f;
	protected float nextAmbienceCheckTime = 0f;

	private void Update()
	{
		//Debug.LogFormat("Can Player Move? {0}", CanMove);

		if (isBellActive) {
			if (Time.time >= bellEndTime) {
				EndBell();
			}				
		}

		FadeOutAudio();
		FadeInAudio();
		//HoldBlackScreen();
		TryFade();

		if (Time.time >= nextAmbienceCheckTime) {
			CheckAmbience();
			nextAmbienceCheckTime = Time.time + ambienceUpdateTime;
		}

	}

	protected void CheckAmbience()
	{
		if (asAmbience.isPlaying)
			return;

		Debug.Log("Ambience not playing, switching track!");
		ambIndex = (ambIndex + 1) % ambience.Length;
		asAmbience.clip = ambience[ambIndex];
		asAmbience.Play();
		nextAmbienceCheckTime = Time.time + (ambienceUpdateTime * 5.0f);

	}

	public void FadeIn()
	{
		Debug.LogFormat("Starting FadeIn at {0}", Time.time);
		fadeGoalTime = Time.time + screenFadeTime;
		BlackScreen.color = Color.black;
		BlackScreen.gameObject.SetActive(true);
		fadeDirection = -1f;
	}

	public void FadeOut()
	{
		Debug.LogFormat("Starting FadeOut at {0}", Time.time);
		fadeGoalTime = Time.time + screenFadeTime;
		BlackScreen.color = colorEmpty;
		BlackScreen.gameObject.SetActive(true);
		fadeDirection = 1f;
	}

	//public void StartBlackScreen(float time, bool fade)
	//{
	//	blackScreenTime = Time.time + time;
	//	Debug.LogFormat("CurrentTime: {0}, HoldingBlackScreenUntil: {1}", Time.time, blackScreenTime);

	//	BlackScreen.color = Color.black;
	//	BlackScreen.gameObject.SetActive(time == 0);
	//	StartCoroutine(HoldBlackScreen(fade));
	//}

	//public void EndBlackScreen()
	//{

	//}

	//IEnumerator HoldBlackScreen(bool fade)
	//{
	//	while (blackScreenTime != 0f) {

	//		if (blackScreenTime < 0f) {
	//			yield return null;
	//		}

	//		if (Time.time < blackScreenTime) {
	//			Debug.Log("Holding Black Screen!");
	//			yield return null;
	//		}

	//		Debug.LogFormat("BST over: Time: {0}, BST: {1}", Time.time, blackScreenTime);

	//		blackScreenTime = 0f;

	//		if (fade)
	//			FadeIn();

	//	}
	//	//StopCoroutine()
	//	//StopCoroutine(this);

	//}

	//public void HoldBlackScreen()
	//{
	//	if (blackScreenTime < 0f)
	//		return;

	//	if (Time.time < blackScreenTime)
	//		return;

	//	blackScreenTime = 0f;

	//}

	protected float GetFadeRatio(float endTIme, float fadeTime)
	{
		return 1 - ((endTIme - Time.time) / fadeTime);
	}

	protected float fadeDirection = 0;
	protected float alphaMin = 0f;
	protected float alphaMax = 1f;

	public void TryFade()
	{
		if (fadeDirection == 0f)
			return;

		if (!BlackScreen.gameObject.activeInHierarchy)
			return;

		//if (Time.time < blackScreenTime)
		//	return;

		//if (Time.time > fadeGoalTime)
		//	return;

		//float fadeRatio = (Time.time / fadeGoalTime);
		//float fadeRatio = GetFadeRatio(fadeGoalTime, fadeTime);

		float fadeRatio = screenFadeCurve.Evaluate(GetFadeRatio(fadeGoalTime, screenFadeTime));

		float a = 0f;
		float start = 0f;
		float end = 0f;

		if (fadeDirection > 0) {
			start = alphaMin;
			end = alphaMax;
		} else if (fadeDirection < 0) {
			start = alphaMax;
			end = alphaMin;
		}

		a = Mathf.Lerp(start, end, fadeRatio);

		if (fadeRatio >= 0.98f) {
			a = end;
		}

		//Debug.LogFormat("Ratio: {0}, a: {1}", fadeRatio, a );

		Color newColor = new Color(0f, 0f, 0f, a);
		BlackScreen.color = newColor;

		if (a == end) {
			Debug.LogFormat("Ended Fade at {0}", Time.time);
			BlackScreen.gameObject.SetActive(false);
			fadeDirection = 0f;
		}
	}

	protected void OnFadeIn()
	{
		Debug.Log("FadeIn finished, enabling movement");
		changingFloors = false;
		CanMove = true;
	}

	protected static bool changingFloors = false;
	public bool CanStartChangeFloors()
	{
		if (changingFloors)
			return false;

		changingFloors = true;
		Debug.Log("Changing floors - Starting Fade Out");

		CanMove = false;
		//Time.timeScale = 0f;
		FadeOut();
		Invoke("FadeIn", fadeGoalTime);
		Invoke("OnFadeIn", fadeGoalTime + (fadeGoalTime / 3f));
		return changingFloors;
	}

	public void ChangedFloors()
	{
		Debug.Log("Finished changing floors!");

		//changingFloors = false;
		//Time.timeScale = 1.0f;
		//CanMove = true;
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

	public void FadeOutAudio()
	{

		//Time.ti

		if (Time.time > audioFadeOutGoalTime)
			return;

		//float ratio = (Time.time / audioFadeOutGoalTime);
		float ratio = GetFadeRatio(audioFadeOutGoalTime, musicFadeTime);
		float volume = (ratio * musicMaxVolume) - musicMaxVolume;

		if (ratio >= 0.98f)
			volume = 0f;

		asMusic.volume = volume;

		if (queuedMusic != null) {
			asMusic.clip = queuedMusic;
			queuedMusic = null;
			audioFadeInGoalTime = Time.time + musicFadeTime;
		}
	}

	public void FadeInAudio()
	{
		if (Time.time > audioFadeInGoalTime)
			return;
		//float ratio = (Time.time / audioFadeInGoalTime);
		float ratio = GetFadeRatio(audioFadeInGoalTime, musicFadeTime);
		float volume = ratio * musicMaxVolume;

		if (ratio >= 0.98f)
			volume = musicMaxVolume;

		asMusic.volume = volume;
	}

	//public void QueuedMusic()
	//{

	//}

	public void PlayBGMLowIntensity()
	{
		if (asMusic.clip == bgmHighIntensity) {
			audioFadeOutGoalTime = Time.time + musicFadeTime;
			queuedMusic = bgmLowIntensity;
			return;
		}

		asMusic.clip = bgmLowIntensity;
		//fadeInGoalTime = Time.time + musicFadeTime;
		asMusic.Play();


		//AkSoundEngine.PostEvent(EventLowIntensity, gameObject);
		//AkSoundEngine.PostEvent(PlayAmbiance, gameObject);
	}

	public void PlayAmbience()
	{
		ambIndex = Random.Range(0, ambience.Length);
		asAmbience.clip = ambience[ambIndex];
		asAmbience.Play();
	}

	public void PlayBGMHighIntensity()
	{
		//AkSoundEngine.PostEvent(EventHighIntensity, gameObject);

		if (asMusic.clip == bgmLowIntensity) {
			audioFadeOutGoalTime = Time.time + (musicFadeTime/2);
			queuedMusic = bgmHighIntensity;
			return;
		}

		asMusic.clip = bgmHighIntensity;
		asMusic.Play();
	}

	public void PlayBGMStop()
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

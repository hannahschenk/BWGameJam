using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Helper class for important references in-game; only inquire in START or later.
/// </summary>
public class GameManager : MonoBehaviour
{

	public static bool _CanMove = true;
	public static bool CanMove
	{
		get
		{
			return _CanMove;
		}
		protected set
		{
			_CanMove = value;
		}
	}

	public static ApartmentBuilder Apartment;
	public static GameManager Manager;

	public static Transform Player;
	public static PlayerStats PlayerStats;
	public static PlayerFPController PlayerController;

	public static Camera PlayerCam;
	public static PlayerInputHandler PlayerInputHandler;
	public static PlayerFPAnimator PlayerFPAnimHandler;
	public static Animator PlayerAnimator;
	public static string PlayerTag = "Player";
	public static PostProcessVolume Volume;

	public AudioClip bgmLowIntensity;
	public AudioClip bgmHighIntensity;

	public AudioClip[] ambience;
	protected int ambIndex = 0;

	public float bellDuration = 10f;
	public bool isBellActive = false;
	protected float bellEndTime = 0f;

	public static int CurrentFloor = 1;

	public AudioSource asMusic;
	protected float musicMaxVolume = 0.90f;
	protected float musicMaxLoadScreen = 1.0f;


	public AudioSource asAmbience;
	protected float ambienceVolume = 0.20f;

	public Image BlackScreen;

	protected float musicFadeTime = 8f;
	protected float audioFadeOutGoalTime = -1f;
	protected float audioFadeInGoalTime = -1f;
	protected AudioClip queuedMusic = null;

	//public AnimationCurve screenFadeCurve;
	public float screenFadeInTime = 6f;
	public float screenFadeOutTime = 1f;
	public float startBlackScreenHoldTime = 4f;

	public bool skipIntro = false;

	//public string EventLowIntensity = "Low_Intensity";
	//public string EventHighIntensity = "High_Intensity";
	//public string PlayAmbiance = "Play_Ambience";
	//public string EventLowIntensity = "Play_Song_EmptyHalls";
	//public string EventHighIntensity = "Play_Song_Ghost_Attack";
	//public string EventStopAll = "Stop_All";

	protected float alphaMin = 0f;
	protected float alphaMax = 1f;

	protected static bool reachedExit = false;
	protected Coroutine warpPlayer = null;


	private void Awake()
	{
		Manager = this;
		Apartment = FindObjectOfType<ApartmentBuilder>() as ApartmentBuilder;

		CachePlayerRefs();
		ConfigureAudio();
		ConfigureFade();
	}

	protected void CachePlayerRefs()
	{
		PlayerStats = FindObjectOfType<PlayerStats>() as PlayerStats;
		if (PlayerStats == null)
			return;

		Player = PlayerStats.transform;
		PlayerController = Player.GetComponent<PlayerFPController>();

		PlayerInputHandler = Player.GetComponent<PlayerInputHandler>();
		PlayerFPAnimHandler = Player.GetComponentInChildren<PlayerFPAnimator>();
		PlayerAnimator = PlayerFPAnimHandler.GetComponent<Animator>();

		PlayerCam = Player.GetComponentInChildren<Camera>(); //old way that has to deal with multiple cameras, don't want to raycast from the scene camera if we have one lol
		Volume = PlayerCam.GetComponent<PostProcessVolume>();
	}

	protected void ConfigureFade()
	{
		if (skipIntro)
			return;
		BlackScreen.color = Color.black;
		BlackScreen.gameObject.SetActive(true);
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
		PlayBGMLowIntensity();
		audioFadeInGoalTime = Time.time + (musicFadeTime);

		PlayAmbience();

		if (skipIntro)
			return;
		DisableMovementAndFadeIn();
	}

	protected void DisableMovementAndFadeIn()
	{
		CanMove = false;
		Invoke("FadeIn", startBlackScreenHoldTime);
		Invoke("EnableMovement", startBlackScreenHoldTime); //+ (screenFadeTime / 3.0f));
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
		//TryFade();

		if (Time.time >= nextAmbienceCheckTime) {
			CheckAmbience();
			nextAmbienceCheckTime = Time.time + ambienceUpdateTime;
		}

	}

	protected void CheckAmbience()
	{
		if (asAmbience.isPlaying)
			return;

		//Debug.Log("Ambience not playing, switching track!");
		ambIndex = (ambIndex + 1) % ambience.Length;
		asAmbience.clip = ambience[ambIndex];
		asAmbience.Play();
		nextAmbienceCheckTime = Time.time + (ambienceUpdateTime * 5.0f);

	}

	public void FadeIn()
	{
		//Debug.LogFormat("Starting FadeIn at {0}, should blend over {1} seconds and end at {2}", Time.time, screenFadeInTime, Time.time + screenFadeInTime);
		BlackScreen.gameObject.SetActive(true);
		BlackScreen.canvasRenderer.SetAlpha(alphaMax);
		IEnumerator newFade = FadeCanvas(BlackScreen.canvasRenderer, alphaMin, screenFadeInTime, false);
		StartCoroutine(newFade);
	}

	public void FadeOut()
	{
		//Debug.LogFormat("Starting FadeOut at {0}, should blend over {1} seconds and end at {2}", Time.time, screenFadeInTime, Time.time + screenFadeInTime);
		//Debug.LogFormat("Starting FadeOut at {0}", Time.time);
		
		BlackScreen.gameObject.SetActive(true);
		BlackScreen.canvasRenderer.SetAlpha(alphaMin);
		
		IEnumerator newFade = FadeCanvas(BlackScreen.canvasRenderer, alphaMax, screenFadeOutTime, false, ap_Utility.LerpTypes.EaseOut);
		StartCoroutine(newFade);
	}

	protected float GetFadeRatio(float endTIme, float fadeTime)
	{
		return 1 - ((endTIme - Time.time) / fadeTime);
	}

	//protected void OnFadeIn()
	//{
	//	//Debug.LogFormat("Enabling movement", Time.time);
	//	CanMove = true;
	//}

	public void EnableMovement()
	{
		CanMove = true;
	}

	public void DisableMovement()
	{
		CanMove = false;
	}

	public void ReachedExit()
	{
		if (reachedExit)
			return;

		reachedExit = true;

		if (warpPlayer != null)
			return;

		//Debug.LogFormat("Manager.ReachedExit firing at {0}!", Time.time);
		//CanMove = false;
		Invoke("DisableMovement", (screenFadeOutTime));

		FadeOut();

		warpPlayer = StartCoroutine(WarpPlayerAndRebuild(screenFadeOutTime + 2));
	}

	protected IEnumerator WarpPlayerAndRebuild(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);

		PlayerController.enabled = false;
		Vector3 newPos = Vector3.zero + (Apartment.GetFloorHeight(CurrentFloor) * Vector3.up);
		//Quaternion rot = Quaternion.FromToRotation(Player.forward, Vector3.forward);
		Quaternion rot = Quaternion.LookRotation(Vector3.forward, Vector3.up);

		Player.SetPositionAndRotation(newPos, rot);
		yield return null;

		Apartment.NewLevel();
		yield return null;

		reachedExit = false;
		PlayerController.enabled = true;
		FadeIn();
		CanMove = true;
		StopCoroutine(warpPlayer);
		warpPlayer = null;
	}

	public bool TryStartBell()
	{
		if (isBellActive)
			return false;

		StartBell(bellDuration);
		return true;
	}

	public void StartBell(float duration)
	{
		//Debug.LogFormat("Starting bell at {0}", Time.time);
		isBellActive = true;
		bellEndTime = Time.time + duration;
	}

	public void EndBell()
	{
		//Debug.LogFormat("Ending bell at {0}", Time.time);
		isBellActive = false;
	}

	public void FadeOutAudio()
	{

		if (Time.time > audioFadeOutGoalTime)
			return;

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

		float ratio = GetFadeRatio(audioFadeInGoalTime, musicFadeTime);
		float volume = ratio * musicMaxVolume;

		if (ratio >= 0.98f)
			volume = musicMaxVolume;

		asMusic.volume = volume;
	}

	public void PlayBGMLowIntensity()
	{
		if (asMusic.clip == bgmHighIntensity) {
			audioFadeOutGoalTime = Time.time + musicFadeTime;
			queuedMusic = bgmLowIntensity;
			return;
		}

		asMusic.clip = bgmLowIntensity;
		asMusic.Play();
	}

	public void PlayAmbience()
	{
		ambIndex = Random.Range(0, ambience.Length);
		asAmbience.clip = ambience[ambIndex];
		asAmbience.Play();
	}

	public void PlayBGMHighIntensity()
	{
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

	//TODO: THIS SHOULDN'T BE HERE, REEEE
	public IEnumerator FadeCanvas(CanvasRenderer canvas, float alpha, float duration, bool ignoreTimeScale, ap_Utility.LerpTypes lerpType = ap_Utility.LerpTypes.EaseIn)
	{

		float startAlpha = canvas.GetAlpha();
		float targetAlpha = alpha;

		float elapsedTime = 0f;

		while (elapsedTime < duration) {

			elapsedTime += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

			float percentage = Mathf.Clamp01(ap_Utility.GetLerpType(elapsedTime / duration, lerpType));

			float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, percentage);
			canvas.SetAlpha(newAlpha);
			yield return null;
		}

		canvas.SetAlpha(targetAlpha);
		//Debug.LogFormat("Finishing Fade at {0}", Time.time);
	}

}

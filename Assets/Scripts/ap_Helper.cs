using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public sealed class ap_Helper
{
	public static readonly ap_Helper instance = new ap_Helper();

	public static bool GetRandomAudioClip(List<AudioClip> clips, Vector2 pitchRange, out AudioClip clip, out float pitch)
	{
		clip = null;
		pitch = 0f;

		if (clips.Count == 0)
			return false;

		clip = clips[Random.Range(0, clips.Count)];
		pitch = Random.Range(pitchRange.x, pitchRange.y);
		return true;
	}

	public static void PlayRandomAudioClip(AudioSource audio, List<AudioClip> clips, Vector2 pitchRange)
	{
		if (GetRandomAudioClip(clips, pitchRange, out AudioClip clip, out float pitch)) {
			//Debug.Log("Playing audio clip!");
			audio.pitch = pitch;
			audio.PlayOneShot(clip);
		}
	}

	public static void PlayClipAt(AudioSource audioSource, Vector3 position, AudioClip soundToPlay)
	{
		GameObject g = new GameObject("SoundAtPosition");
		g.transform.position = position;
		AudioSource newAudio = g.AddComponent<AudioSource>();

		newAudio.volume = audioSource.volume;
		newAudio.pitch = audioSource.pitch;
		newAudio.spatialBlend = audioSource.spatialBlend;
		newAudio.minDistance = audioSource.minDistance;
		newAudio.maxDistance = audioSource.maxDistance;

		newAudio.clip = soundToPlay;
		newAudio.Play();
		GameObject.Destroy(g, soundToPlay.length);
		//vp_Utility.Destroy(g, soundToPlay.length);
	}

}

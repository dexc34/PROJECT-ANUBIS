using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Config", menuName = "Weapons/Audio Config", order = 1)]
public class GunAudio : ScriptableObject
{
    [Range(0, 1f)]
    public float volume = 1f;
    public AudioClip[] fireClips;
    public AudioClip cockClip;
    public AudioClip reloadClip; 

    public void PlayShootClip(AudioSource audio)
    {
        audio.pitch = Random.Range(0.9f, 1.1f);
        audio.PlayOneShot(fireClips[Random.Range(0, fireClips.Length)], volume);
    }

    public void PlayCockClip(AudioSource audio)
    {
        audio.pitch = 1;
        audio.PlayOneShot(cockClip, volume);
    }

    public void PlayReloadClip(AudioSource audio)
    {
        audio.pitch = 1;
        if (reloadClip != null)
            audio.PlayOneShot(reloadClip, volume);
    }
}

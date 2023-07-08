using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    //Script variables
    private float slideStartVolume;
    private float windVolume;

    //Audio sources
    private AudioSource[] audioSources;
    private AudioSource windSFX;
    private AudioSource jumpSFX;
    private AudioSource slideSFX;
    private AudioSource dashSFX;
    private AudioSource groundPoundImpactSFX;

    //Required components
    private Movement movementScript;
    private CharacterController characterController;

    private void Awake() 
    {
        GetAllComponents();
    }

    private void Update() 
    {
        if(characterController.isGrounded)
        {
            //Play slide sfx if sliding on ground
            if(movementScript.isSliding)
            {
                if(!slideSFX.isPlaying)
                {
                    PlayOnRandomClipSecond(slideSFX);
                }
            }

            //Fade out slide sfx if isn't sliding
            else
            {
                if(slideSFX.volume == slideStartVolume && slideSFX.isPlaying)
                {
                    StartCoroutine(AudioFade(slideSFX, .2f));
                }
            }

            //If isn't sliding or dashing, don't play wind sfx while grounded
            if(!movementScript.isSliding && !movementScript.isDashing)
            {
                StartCoroutine(AudioFade(windSFX, 0.1f));                
            }

            //Play wind sfx if sliding or dashing on the ground
            else
            {
                if(!windSFX.isPlaying)
                {
                    PlayOnRandomClipSecond(windSFX);
                }
                windVolume = characterController.velocity.magnitude/50;
            }
        }
        else
        {
            //Always play wind sfx when not grounded
            if(!windSFX.isPlaying)
            {
                PlayOnRandomClipSecond(windSFX);
            }
            windVolume = characterController.velocity.magnitude/50;

            //Stop slide sfx if ins't grounded
            if(slideSFX.volume == slideStartVolume && slideSFX.isPlaying)
            {
                StartCoroutine(AudioFade(slideSFX, .2f));
            }
        }

        windVolume = Mathf.Clamp(windVolume, 0, 1.5f);
        windSFX.volume = windVolume;
    }

    private void PlayOnRandomClipSecond(AudioSource clipToPlay)
    {
        clipToPlay.time = Random.Range(0, clipToPlay.clip.length);
        clipToPlay.Play();
    }

    private IEnumerator AudioFade(AudioSource soundToFade, float fadeTime)
    {
        float startingVolume = soundToFade.volume;

        while(soundToFade.volume > 0)
        {
            soundToFade.volume -= startingVolume * Time.deltaTime/fadeTime;
            yield return null;
        }

        soundToFade.Stop();
        soundToFade.volume = startingVolume;
    }

    public void PlayJumpSFX()
    {
        jumpSFX.Play();
    }

    public void PlayDashSFX()
    {
        dashSFX.Play();
    }

    public void PlayGroundPoundImpact()
    {
        groundPoundImpactSFX.Play();
    }

    //Gets all necessary components and audio sources
    private void GetAllComponents()
    {
        characterController = transform.parent.GetComponent<CharacterController>();
        movementScript = transform.parent.GetComponent<Movement>();

        audioSources = GetComponents<AudioSource>();
        windSFX = CheckAudioSourceArray("Wind");
        slideSFX = CheckAudioSourceArray("Slide");
        slideStartVolume = slideSFX.volume;
        dashSFX = CheckAudioSourceArray("Dash");
        jumpSFX = CheckAudioSourceArray("Jump");
        groundPoundImpactSFX = CheckAudioSourceArray("Ground Pound Impact");
    }

    //Look for specific audio sources and assign them to the approriate variable
    private AudioSource CheckAudioSourceArray(string clipNameToLookFor)
    {
        AudioSource clipFound = null;

        for(int i = 0; i < audioSources.Length; i++)
        {
            if(audioSources[i].clip.name == clipNameToLookFor)
            {
                clipFound = audioSources[i];
            }
        }
        return(clipFound);
    }
}

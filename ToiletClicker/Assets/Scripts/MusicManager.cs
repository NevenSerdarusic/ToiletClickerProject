using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    //[SerializeField] private FirewallManager firewallManager;

    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusicClip;

    [Header("Audio Source Reference")]
    [SerializeField] private AudioSource backgroundMusicAudioSource;

    [Header("Background Music Settings")]
    [SerializeField] private float backgroundMusicVolume = 0.2f;
    [SerializeField] private float backgroundMusicFadeInduration = 5f;

    //private AudioClip currentClip;


    private void Start()
    {
        if (backgroundMusicAudioSource == null)
        {
            Debug.LogError("AudioSource for background music is not assigned!");
            return;
        }

        BackgroundMusicSettings();
        PlayBackgroundMusic();
    }

    private void PlayBackgroundMusic()
    {
        backgroundMusicAudioSource.clip = backgroundMusicClip;
        backgroundMusicAudioSource.Play();
    }

    private void BackgroundMusicSettings()
    {
        backgroundMusicAudioSource.loop = true;
        backgroundMusicAudioSource.playOnAwake = true;

        //Check and set mute status from PlayerPrefs
        bool isMuted = PlayerPrefsHandler.IsMusicMuted();
        backgroundMusicAudioSource.mute = isMuted;

        //if the music is not muted then gradually make a Fade in
        if (!isMuted)
        {
            StartCoroutine(FadeInMusic(backgroundMusicVolume, backgroundMusicFadeInduration));
        }
    }

    //Method for mute/unmute background music
    public void ToggleBackgroundMusicMute()
    {
        bool isMuted = PlayerPrefsHandler.IsMusicMuted();
        bool newMuteStatus = !isMuted;         

        PlayerPrefsHandler.SetMusicMuted(newMuteStatus);
        backgroundMusicAudioSource.mute = newMuteStatus;
        backgroundMusicAudioSource.volume = backgroundMusicVolume;
    }

    //Method for FadeIn Background Music on Start
    private IEnumerator FadeInMusic(float targetVolume, float duration)
    {
        backgroundMusicAudioSource.volume = 0f;
        backgroundMusicAudioSource.Play();

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            backgroundMusicAudioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            yield return null;
        }

        backgroundMusicAudioSource.volume = targetVolume;
    }
}

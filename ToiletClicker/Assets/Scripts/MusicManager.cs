using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private WeightManager weightManager;

    [Header("Audio Clips by Weight Range")]
    [SerializeField] private AudioClip musicClip_01; // 80 - 120
    [SerializeField] private AudioClip musicClip_02; // 121 - 160
    [SerializeField] private AudioClip musicClip_03; // 161 - 200
    [SerializeField] private AudioClip musicClip_04; // 201+

    [Header("Audio Source Reference")]
    [SerializeField] private AudioSource backgroundMusicAudioSource;

    [Header("Background Music Settings")]
    [SerializeField] private float backgroundMusicVolume = 0.2f;
    [SerializeField] private float backgroundMusicFadeInduration = 5f;

    private AudioClip currentClip;


    private void Start()
    {
        if (weightManager == null || backgroundMusicAudioSource == null)
        {
            Debug.LogError("WeightManager or AudioSource is not assigned!");
            return;
        }

        BackgroundMusicSettings();

        //Initial background music
        UpdateMusicByWeight(weightManager.GetCurrentWeight());
    }

    private void OnEnable()
    {
        weightManager.OnWeightChanged += UpdateMusicByWeight;
    }

    private void OnDisable()
    {
        weightManager.OnWeightChanged -= UpdateMusicByWeight;
    }

    private void UpdateMusicByWeight(float weight)
    {
        AudioClip newClip = GetClipForWeight(weight);

        if (newClip != null && newClip != currentClip)
        {
            currentClip = newClip;
            backgroundMusicAudioSource.clip = currentClip;
            backgroundMusicAudioSource.Play();
        }
    }

    private AudioClip GetClipForWeight(float weight)
    {
        if (weight >= gameConfig.range1Min && weight <= gameConfig.range1Max)
            return musicClip_01;
        else if (weight > gameConfig.range2Min && weight <= gameConfig.range2Max)
            return musicClip_02;
        else if (weight > gameConfig.range3Min && weight <= gameConfig.range3Max)
            return musicClip_03;
        else if (weight > gameConfig.range4Min)
            return musicClip_04;
        else
            return null;
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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    private class SoundData
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    public static SoundManager Instance { get; private set; }

    [Header("UI Button Click Sound")]
    [SerializeField] private AudioClip clickSound;

    [Header("General Sounds")]
    [SerializeField] private List<SoundData> generalSounds;

    [Header("Fart Sounds")]
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private float typingSoundVolume = 0.5f;

    [Header("Volume Slider")]
    [SerializeField] private Slider volumeSlider;

    [Header("UI Buttons")]
    [SerializeField] private List<Button> gameButtonList;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource generalAudioSource;
    [SerializeField] private AudioSource typingAudioSource;
    [SerializeField] private AudioSource buttonAudioSource;

    private readonly Dictionary<string, AudioSource> activeSounds = new();
    private Dictionary<string, SoundData> soundLookup = new();

    private static bool buttonsInitialized = false;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (generalAudioSource == null || typingAudioSource == null || buttonAudioSource == null)
            Debug.LogError("[SoundManager] AudioSources not properly assigned!");

        soundLookup = generalSounds.ToDictionary(s => s.name, s => s);
        SoundsSettings();
    }

    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(HandleTypingVolumeChanged);

            // Set slider on saved value in PlayerPrefs
            float savedVolume = PlayerPrefsHandler.GetTypingVolume();
            volumeSlider.value = savedVolume;

            // Set AudioSource volume on same value as savedVolume
            typingSoundVolume = savedVolume;
            typingAudioSource.volume = typingSoundVolume;
        }

        if (!buttonsInitialized)
        {
            SetupAllUIButtonSounds();
            buttonsInitialized = true;
        }
    }

    private void SetupAllUIButtonSounds()
    {
        gameButtonList = Resources.FindObjectsOfTypeAll<Button>()
        .Where(b => b.gameObject.hideFlags == HideFlags.None && !string.IsNullOrEmpty(b.gameObject.scene.name))
        .ToList();


        foreach (var button in gameButtonList)
        {
            button.onClick.AddListener(() => PlayClick());
        }
    }

    private void PlayClick()
    {
        if (clickSound != null)
            buttonAudioSource.PlayOneShot(clickSound);
    }

    public void Play(string clipName)
    {
        if (soundLookup.TryGetValue(clipName, out var soundData))
        {
            generalAudioSource.volume = soundData.volume;
            generalAudioSource.PlayOneShot(soundData.clip);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Clip with name '{clipName}' not found.");
        }
    }

    public void PlayControlled(string clipName)
    {
        if (!soundLookup.TryGetValue(clipName, out var soundData))
        {
            Debug.LogWarning($"[SoundManager] Clip '{clipName}' not found.");
            return;
        }

        if (activeSounds.TryGetValue(clipName, out var source))
        {
            source.Stop();
            source.Play();
            return;
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = soundData.clip;
        newSource.volume = soundData.volume;
        newSource.loop = false;
        newSource.playOnAwake = false;
        newSource.Play();

        activeSounds.Add(clipName, newSource);
    }

    public void Stop(string clipName)
    {
        if (activeSounds.TryGetValue(clipName, out var source))
        {
            source.Stop();
            Destroy(source);
            activeSounds.Remove(clipName);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] No active sound found with name '{clipName}' to stop.");
        }
    }

    public void PlayTypingSound()
    {
        if (typingSound == null) return;

        typingAudioSource.volume = typingSoundVolume;
        typingAudioSource.PlayOneShot(typingSound);
    }

    //Looping Sounds (Bank Alarm)
    // Add this method to play a looping sound
    public void PlayLoopingSound(string clipName)
    {
        if (!soundLookup.TryGetValue(clipName, out var soundData))
        {
            Debug.LogWarning($"[SoundManager] Looping clip '{clipName}' not found.");
            return;
        }

        if (activeSounds.ContainsKey(clipName))
        {
            Debug.Log($"[SoundManager] Looping sound '{clipName}' is already playing.");
            return;
        }

        AudioSource loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.clip = soundData.clip;
        loopSource.volume = soundData.volume;
        loopSource.loop = true;
        loopSource.playOnAwake = false;
        loopSource.Play();

        activeSounds.Add(clipName, loopSource);
    }

    // Add this method to stop a looping sound
    public void StopLoopingSound(string clipName)
    {
        if (activeSounds.TryGetValue(clipName, out var source))
        {
            source.Stop();
            Destroy(source);
            activeSounds.Remove(clipName);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Looping sound '{clipName}' is not currently playing.");
        }
    }


    private void SoundsSettings()
    {
        ApplyAudioSettings(generalAudioSource, 1f);
        ApplyAudioSettings(buttonAudioSource, 1f);
        ApplyAudioSettings(typingAudioSource, typingSoundVolume);
    }

    private void ApplyAudioSettings(AudioSource source, float volume)
    {
        if (source == null) return;

        source.volume = volume;
        source.loop = false;
        source.playOnAwake = false;
    }

    //Method for managing volume slider of fartAudioSource
    private void HandleTypingVolumeChanged(float newValue)
    {
        typingSoundVolume = newValue;

        if (typingAudioSource != null)
        {
            typingAudioSource.volume = typingSoundVolume;
            PlayTypingSound(); //Play typing sound when sliding on slider
        }

        PlayerPrefsHandler.SetTypingVolume(newValue);
    }
}

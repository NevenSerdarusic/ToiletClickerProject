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
    [SerializeField] private List<AudioClip> fartSounds;
    [SerializeField] private float fartSoundVolume = 0.5f;

    [Header("UI Buttons")]
    [SerializeField] private List<Button> gameButtonList;

    [Header("Main Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float soundVolume = 1f;

    private Dictionary<string, AudioSource> activeSounds = new();
    private Dictionary<string, SoundData> soundLookup = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetupAllUIButtonSounds();
        SoundsSettings();
        soundLookup = generalSounds.ToDictionary(s => s.name, s => s);
    }

    private void SetupAllUIButtonSounds()
    {
        gameButtonList = FindObjectsOfType<Button>(true).ToList();
        foreach (var button in gameButtonList)
        {
            button.onClick.AddListener(() => PlayClick());
        }
    }

    private void PlayClick()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void Play(string clipName)
    {
        if (soundLookup.TryGetValue(clipName, out var soundData))
        {
            audioSource.volume = soundData.volume;
            audioSource.PlayOneShot(soundData.clip);
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

    public void PlayRandom()
    {
        if (fartSounds.Count == 0) return;

        AudioClip randomClip = fartSounds[Random.Range(0, fartSounds.Count)];
        audioSource.volume = fartSoundVolume;
        audioSource.PlayOneShot(randomClip);
    }

    private void SoundsSettings()
    {
        audioSource.volume = soundVolume;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }
}

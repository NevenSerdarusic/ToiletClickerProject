using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("Ui Button Click Sound")]
    [SerializeField] private AudioClip clickSound;

    [Header("General Sound List")]
    [SerializeField] private List<AudioClip> generalSounds;

    [Header("Fart Sound List")]
    [SerializeField] private List<AudioClip> fartSounds;

    [Header("UI Game Buttons")]
    [SerializeField] private List<Button> gameButtonList;

    [Header("Audio Source Reference")]
    [SerializeField] private AudioSource audioSource;

    [Header("Audio Source Settings")]
    [SerializeField] private float soundVolume = 1f;

    private void Start()
    {
        SetupAllUIButtonSounds();
        SoundsSettings();
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

    //Play general Sound
    public void Play(string clipName)
    {
        var clip = generalSounds.FirstOrDefault(c => c != null && c.name == clipName);
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Clip with name '{clipName}' not found.");
        }
    }

    //Randomly play fart sound
    public void PlayRandom()
    {
        if (fartSounds.Count == 0) return;

        AudioClip randomClip = fartSounds[Random.Range(0, fartSounds.Count)];
        audioSource.PlayOneShot(randomClip);
    }

    private void SoundsSettings()
    {
        audioSource.volume = soundVolume;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
    }
}

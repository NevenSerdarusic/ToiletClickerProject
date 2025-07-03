using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeightManager weightManager;

    [Header("Audio Clips by Weight Range")]
    [SerializeField] private AudioClip musicClip_01; // 80 - 120
    [SerializeField] private AudioClip musicClip_02; // 121 - 160
    [SerializeField] private AudioClip musicClip_03; // 161 - 200
    [SerializeField] private AudioClip musicClip_04; // 201+

    [Header("Weight Ranges")]
    [SerializeField] private float range1Min = 80f;
    [SerializeField] private float range1Max = 120f;

    [SerializeField] private float range2Min = 121f;
    [SerializeField] private float range2Max = 160f;

    [SerializeField] private float range3Min = 161f;
    [SerializeField] private float range3Max = 200f;

    [SerializeField] private float range4Min = 201f;

    [Header("Audio Source Reference")]
    [SerializeField] private AudioSource audioSource;

    [Header("Audio Source Settings")]
    [SerializeField] private float backgroundMusicVolume = 0.7f;

    private AudioClip currentClip;



    private void Start()
    {
        if (weightManager == null || audioSource == null)
        {
            Debug.LogError("WeightManager or AudioSource is not assigned!");
            return;
        }

        //Initial background music
        UpdateMusicByWeight(weightManager.GetCurrentWeight());

        MainMusicSettings();
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
            audioSource.clip = currentClip;
            audioSource.Play();
        }
    }

    private AudioClip GetClipForWeight(float weight)
    {
        if (weight >= range1Min && weight <= range1Max)
            return musicClip_01;
        else if (weight > range2Min && weight <= range2Max)
            return musicClip_02;
        else if (weight > range3Min && weight <= range3Max)
            return musicClip_03;
        else if (weight > range4Min)
            return musicClip_04;
        else
            return null;
    }

    private void MainMusicSettings()
    {
        audioSource.volume = backgroundMusicVolume;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
    }
}

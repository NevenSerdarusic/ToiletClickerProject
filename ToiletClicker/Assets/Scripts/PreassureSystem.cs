using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreassureSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig config;
    
    [Header("Slider Settings")]
    [SerializeField] private Slider pressureSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject warningText;

    private float currentPressure = 0f;
    private bool isOverloaded = false;

    private const string CriticalPressure = "CRITICAL PRESSURE! You need to stop pushing";
    private const string PressureNormalized = "Pressure back to normal. Proceed when ready.";

    private void Start()
    {
        if (warningText != null)
            warningText.SetActive(false);

        pressureSlider.value = 0;
    }


    private void Update()
    {
        if (currentPressure > 0)
        {
            currentPressure -= config.pressureDecreasePerSecond * Time.deltaTime;
            currentPressure = Mathf.Clamp(currentPressure, 0f, 100f);
            UpdateSlider();
        }

        if (currentPressure >= config.criticalThreshold && !isOverloaded)
        {
            isOverloaded = true;
            OnCriticalPressureReached();
        }
        else if (currentPressure < config.criticalThreshold && isOverloaded)
        {
            isOverloaded = false;
            OnPressureBackToSafe();
        }
    }

    public void OnClick()
    {
        if (isOverloaded)
        {
            return;
        }

        currentPressure += config.pressurePerClick;
        currentPressure = Mathf.Clamp(currentPressure, 0f, 100f);
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        if (pressureSlider != null)
            pressureSlider.value = currentPressure / 100f;

        if (fillImage != null)
            fillImage.color = Color.Lerp(Color.green, Color.red, currentPressure / 100f);
    }

    private void OnCriticalPressureReached()
    {
        //Animacija

        if (warningText != null)
            warningText.GetComponent<TMP_Text>().text = CriticalPressure;
            warningText.SetActive(true);
    }

    private void OnPressureBackToSafe()
    {

        if (warningText != null)
            warningText.GetComponent<TMP_Text>().text = PressureNormalized;
            warningText.SetActive(false);
    }

    public bool IsOverloaded() => isOverloaded;

}

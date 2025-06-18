using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreassureSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private WeightManager weightManager;
    [SerializeField] private GameManager gameManager;
    
    [Header("Slider Settings")]
    [SerializeField] private Slider pressureSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject warningText;

    private float currentPressure = 0f;
    private float overloadTimer = 0f;
    private bool isOverloaded = false;

    //Prebaciti text u posebnu klasu
    private const string CriticalPressure = "CRITICAL PRESSURE! You need to stop pushing";

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
            currentPressure -= gameConfig.pressureDecreasePerSecond * Time.deltaTime;
            currentPressure = Mathf.Clamp(currentPressure, 0f, 100f);
            UpdateSlider();
        }

        if (currentPressure >= gameConfig.criticalThreshold)
        {
            if (!isOverloaded)
            {
                isOverloaded = true;
                OnCriticalPressureReached();
                //Debug.Log("[PreassureSystem] ENTERED overload zone!");
            }

            //Start measuring time in overload
            overloadTimer += Time.deltaTime;
            //Debug.Log($"[PreassureSystem] Overload timer: {overloadTimer:F2} / {gameConfig.overloadDurationBeforeGameOver}");

            if (overloadTimer >= gameConfig.overloadDurationBeforeGameOver)
            {
                //Debug.Log("[PreassureSystem] Overload timer exceeded! Triggering GameOver: ToiletOveruse");
                gameManager.TriggerGameOver(GameOverReason.PressureOverload);
            }
        }
        else if (isOverloaded)
        {
            isOverloaded = false;
            OnPressureBackToSafe();
            overloadTimer = 0f;
            //Debug.Log("[PreassureSystem] EXITED overload zone! Timer reset.");
        }
    }

    public void OnClick()
    {
        if (isOverloaded)
        {
            return;
        }

        currentPressure += gameConfig.pressurePerClick;
        currentPressure = Mathf.Clamp(currentPressure, 0f, 100f);
        UpdateSlider();

        if (weightManager != null)
        {
            weightManager.SubtractWeight(gameConfig.weightLossPerClick);
        }
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
            warningText.SetActive(false);
    }

    public bool IsOverloaded() => isOverloaded;

}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ClickBoostManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig config;
    [SerializeField] private GameManager gameManager;


    [Header("Boost Button")]
    [SerializeField] private GameObject boostButton;
    private Button boostUIButton;

    [Header("Boost UI")]
    [SerializeField] private Image boostTimerImage;


    private bool isBoostActive = false;
    private float defaultClickValue;

    private void Start()
    {
        if (boostButton != null)
        {
            boostButton.SetActive(false);
            boostUIButton = boostButton.GetComponent<Button>();

            if (boostUIButton != null)
            {
                boostUIButton.onClick.AddListener(ActivateBoost);
            }
            else
            {
                Debug.LogError("BoostButton nema Button komponentu!");
            }
        }

        if (boostTimerImage != null)
        {
            boostTimerImage.fillAmount = 1f;
            boostTimerImage.gameObject.SetActive(false);
        }
            
        // Pokreni logiku za periodicno pojavljivanje boosta
        StartCoroutine(ShowBoostButtonRoutine());
    }

    private IEnumerator ShowBoostButtonRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(config.boostAppearMinTime, config.boostAppearMaxTime)); // svakih X sekundi

            if (!isBoostActive)
                boostButton.SetActive(true);
        }
    }

    public void ActivateBoost()
    {
        if (isBoostActive || gameManager == null) return;

        isBoostActive = true;
        boostButton.SetActive(false);

        gameManager.SetClickMultiplier(config.boostMultiplier);

        if (boostTimerImage != null)
        {
            StartCoroutine(UpdateBoostTimerUI(config.boostDuration));
            boostTimerImage.gameObject.SetActive(true);
        }
            

        StartCoroutine(ResetBoostAfterDuration());
    }

    private IEnumerator UpdateBoostTimerUI(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fillAmount = 1f - (elapsed / duration);
            if (boostTimerImage != null)
                boostTimerImage.fillAmount = fillAmount;

            yield return null;
        }

        if (boostTimerImage != null)
        {
            boostTimerImage.fillAmount = 0f;
            boostTimerImage.gameObject.SetActive(false);
        }
            

    }

    private IEnumerator ResetBoostAfterDuration()
    {
        yield return new WaitForSeconds(config.boostDuration);

        gameManager.ResetClickMultiplier();
        isBoostActive = false;
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveButtonSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private FirewallManager firewallManager;
    [SerializeField] private UIManager uiManager;

    [Header("Settings")]
    [SerializeField] private RectTransform clickTargetArea;
    [SerializeField] private float minSpawnInterval = 5f;
    [SerializeField] private float maxSpawnInterval = 15f;
    [SerializeField] private float buttonLifetime = 4f;
    [SerializeField] private TMP_Text buttonText;

    [Header("Interactive Button")]
    [SerializeField] private Button interactiveButton;
    [SerializeField] private float buttonSpawningAnimationDuration = 0.3f;

    private RectTransform buttonRect;

    private Coroutine spawnRoutine;

    private bool isPositive;
    private int amount;

    private void OnEnable()
    {
        interactiveButton.onClick.AddListener(OnInteractiveButtonClicked);
    }

    private void OnDisable()
    {
        interactiveButton.onClick.RemoveListener(OnInteractiveButtonClicked);
    }

    private void Start()
    {
        interactiveButton.gameObject.SetActive(false);
        spawnRoutine = StartCoroutine(SpawnEffectButtonRoutine());
        buttonRect = interactiveButton.GetComponent<RectTransform>();
    }

    private IEnumerator SpawnEffectButtonRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            ShowEffectButton();
        }
    }

    private void ShowEffectButton()
    {
        interactiveButton.gameObject.SetActive(true);

        //Get button rect transform and set random position of spawning
        RectTransform rect = interactiveButton.GetComponent<RectTransform>();
        rect.anchoredPosition = GetRandomPositionInside(clickTargetArea);

        //Set randpm amount
        amount = Random.Range(gameConfig.minFirewallIncreaseValue, gameConfig.maxFirewallIncreaseValue);
        //Radnomly choose between positive and negative effect
        isPositive = Random.value > 0.5f;

        //Set color
        buttonText.text = isPositive ? $"-{amount}" : $"+{amount}";
        buttonText.color = isPositive ? uiManager.WarningTextColor : uiManager.DangerTextColor;

        //Button animation
        LeanTween.scale(buttonRect, Vector3.one, buttonSpawningAnimationDuration).setEaseOutBack();

        StartCoroutine(HideButtonAfterSeconds(buttonLifetime));
    }

    private void OnInteractiveButtonClicked()
    {
        if (isPositive)
            firewallManager.SubtractFirewallProtection(amount);
        else
            firewallManager.AddFirewallProtection(amount);

        interactiveButton.gameObject.SetActive(false);
    }

    private IEnumerator HideButtonAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        AnimateButtonHide();
    }

    private void AnimateButtonHide()
    {
        // Pop-up out animacija
        LeanTween.scale(buttonRect, Vector3.zero, buttonSpawningAnimationDuration).setEaseInBack().setOnComplete(() =>
        {
            interactiveButton.gameObject.SetActive(false);
        });
    }

    private Vector2 GetRandomPositionInside(RectTransform area)
    {
        float width = area.rect.width;
        float height = area.rect.height;

        float x = Random.Range(-width / 2f, width / 2f);
        float y = Random.Range(-height / 2f, height / 2f);

        return new Vector2(x, y);
    }
}

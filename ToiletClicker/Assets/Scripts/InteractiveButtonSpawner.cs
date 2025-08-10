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

        //Set position
        RectTransform rect = interactiveButton.GetComponent<RectTransform>();
        rect.anchoredPosition = GetRandomPositionInside(clickTargetArea);
        rect.localScale = Vector3.zero;

        //Randomly choose between positive and negative
        isPositive = Random.value > 0.5f;

        if (isPositive)
        {
            amount = Random.Range(gameConfig.Current.minPositiveFirewallValue, gameConfig.Current.maxPositiveFirewallValue);
            buttonText.text = $"-{amount}%";
            buttonText.color = uiManager.SuccessTextColor;
        }
        else
        {
            amount = Random.Range(gameConfig.Current.minNegativeFirewallValue, gameConfig.Current.maxNegativeFirewallValue);
            buttonText.text = $"+{amount}%";
            buttonText.color = uiManager.ErrorTextColor;
        }

        //Pop-in animation
        LeanTween.scale(rect, Vector3.one, buttonSpawningAnimationDuration).setEaseOutBack();

        //Auto-hide
        StartCoroutine(HideButtonAfterSeconds(buttonLifetime));
    }

    private void OnInteractiveButtonClicked()
    {
        if (isPositive)
        {
            firewallManager.SubtractFirewallProtection(amount);
            SoundManager.Instance.Play("FirewallDecrease");
        } 
        else
        {
            firewallManager.AddFirewallProtection(amount);
            SoundManager.Instance.Play("FirewallIncrease");
        }
            
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

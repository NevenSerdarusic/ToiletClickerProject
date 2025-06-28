using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickTarget : MonoBehaviour, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameConfig gameConfig;

    [Header("Character Animations")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private string clickAnimationTrigger = "Click";

    public event Action OnClicked;

    private Coroutine autoClickRoutine; //coroutine responsible for autoclick upgrade

    private bool isBlocked = false; //bool responsible for blocking tapping on clicktarget in case of game over

    public void BlockClicks(bool block)
    {
        isBlocked = block;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isBlocked || gameManager.IsGameOver) return;

        OnClicked?.Invoke();

        //Implementacija animacija lika
        //if (characterAnimator != null)
        //{
        //    characterAnimator.SetTrigger(clickAnimationTrigger);
        //}
    }

    public void SetClickMultiplier(float multiplier)
    {
        if (gameManager != null)
        {
            gameManager.SetClickMultiplier(multiplier);
        }
        else
        {
            Debug.LogWarning("GameManager reference not set!");
        }
    }

    //AutoClick Upgrade Settings
    public void EnableAutoClick(bool enable)
    {
        if (enable)
        {
            if (autoClickRoutine == null)
                autoClickRoutine = StartCoroutine(AutoClickRoutine());
        }
        else
        {
            if (autoClickRoutine != null)
            {
                StopCoroutine(autoClickRoutine);
                autoClickRoutine = null;
            }
        }
    }

    private IEnumerator AutoClickRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(gameConfig.autoClickInterval);
            OnClicked?.Invoke();

            // Animacija
            // if (characterAnimator != null)
            //     characterAnimator.SetTrigger(clickAnimationTrigger);
        }
    }
}

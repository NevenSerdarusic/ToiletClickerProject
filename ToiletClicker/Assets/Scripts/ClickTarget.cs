using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickTarget : MonoBehaviour, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameConfig gameConfig;

    [Header("Events")]
    [SerializeField] private GameEventChannel clickEvent;

    [Header("Character Animations")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private string clickAnimationTrigger = "Click";

    [Header("Flying Clickable Bonus")]
    [SerializeField] private FlyingClickableBonus prefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private int poolSize = 7;
    [SerializeField] private Transform parentCanvas;

    public event Action OnClicked;

    private Coroutine autoClickRoutine; //coroutine responsible for autoclick upgrade

    private bool isBlocked = false; //bool responsible for blocking tapping on clicktarget in case of game over

    private Queue<FlyingClickableBonus> pool = new Queue<FlyingClickableBonus>(); //Pool of all clickable flying bonuses


    private void Awake()
    {
        //Populate CLICKABLE POOL and turn OFF all childs od pool
        for (int i = 0; i < poolSize; i++)
        {
            var instance = Instantiate(prefab, parentCanvas);
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }

    public void BlockClicks(bool block)
    {
        isBlocked = block;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isBlocked || gameManager.IsGameOver && clickEvent) return;

        if(clickEvent != null)
        {
            clickEvent.RaiseEvent();
        }

        OnClicked?.Invoke();

        Vector2 screenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out screenPos);
        //Set end position: top left corner
        Vector2 targetPos = new Vector2(-canvas.pixelRect.width / 2f + 50f, canvas.pixelRect.height / 2f - 50f);


        ActivateFlyingClickableEvent(screenPos, targetPos);

        //Implementacija animacija lika
        //if (characterAnimator != null)
        //{
        //    characterAnimator.SetTrigger(clickAnimationTrigger);
        //}

        SoundManager.Instance.PlayRandom();
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

    //Animate logic for Flying Clickable Bonuses
    public void ActivateFlyingClickableEvent(Vector2 screenPosition, Vector2 endPosition)
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("Pool exhausted!");
            return;
        }

        var image = pool.Dequeue();
        image.Animate(screenPosition, endPosition, ReturnClickableBonusToPool);
    }

    private void ReturnClickableBonusToPool(FlyingClickableBonus image)
    {
        pool.Enqueue(image);
    }
}

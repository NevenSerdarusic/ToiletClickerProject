using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlyingClickableBonus : MonoBehaviour
{
    [SerializeField] private Vector2 endSize = new Vector2(100f, 100f);
    [SerializeField] private Vector2 startSize = new Vector2(40f, 40f);
    [SerializeField] private float animationDuration = 1.0f;

    private RectTransform rectTransform;
    private Coroutine currentAnim;

    private Vector2 targetPosition;

    private System.Action<FlyingClickableBonus> onCompleteCallback;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Animate(Vector2 startPosition, Vector2 endPosition, System.Action<FlyingClickableBonus> onComplete)
    {
        gameObject.SetActive(true);
        rectTransform.anchoredPosition = startPosition;
        rectTransform.sizeDelta = startSize;
        targetPosition = endPosition;
        onCompleteCallback = onComplete;

        if (currentAnim != null) StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        float time = 0;
        Vector2 initialPosition = rectTransform.anchoredPosition;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float t = time / animationDuration;

            rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, t);
            rectTransform.sizeDelta = Vector2.Lerp(startSize, endSize, t);

            yield return null;
        }

        gameObject.SetActive(false);
        onCompleteCallback?.Invoke(this);
    }
}


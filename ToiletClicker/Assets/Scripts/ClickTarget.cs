using UnityEngine;
using UnityEngine.EventSystems;

public class ClickTarget : MonoBehaviour, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [Header("Character Animations")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private string clickAnimationTrigger = "Click";

    public void OnPointerDown(PointerEventData eventData)
    {
        gameManager.RegisterClick();

        //Implementacija animacija lika
        //if (characterAnimator != null)
        //{
        //    characterAnimator.SetTrigger(clickAnimationTrigger);
        //}
    }
}

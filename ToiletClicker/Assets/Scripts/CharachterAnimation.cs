using NUnit.Framework;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CharachterAnimation : MonoBehaviour
{
    [SerializeField] private GameEventChannel clickEvent;
    [SerializeField] private Sprite[] frames;
    private int frameCounter = 0;
    private Image charachterImage;

	private void OnEnable()
	{
        clickEvent.OnEventRaised += Click;
	}

	private void OnDisable()
	{
        clickEvent.OnEventRaised -= Click;
	}
	void Start()
    {
        charachterImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Click()
    { 
        frameCounter++;
        if (frameCounter > frames.Length - 1)
        {
            frameCounter = 0;
        }
        charachterImage.sprite = frames[frameCounter];
    }
}

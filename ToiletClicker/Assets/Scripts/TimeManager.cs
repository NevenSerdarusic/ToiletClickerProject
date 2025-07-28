using UnityEngine;
using UnityEngine.UIElements;

public class TimeManager : MonoBehaviour
{

    [SerializeField] private TimeCounterEvent timerCounterEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("OneSecondPassed", 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OneSecondPassed()
    {
        timerCounterEvent.RaiseEvent();
        Debug.Log("One second Passed, raised Event");
    }
}

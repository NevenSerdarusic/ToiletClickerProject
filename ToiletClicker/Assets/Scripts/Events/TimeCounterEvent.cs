using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="TimeCounterEvent", menuName = "Scriptable Objects/TimeCounterEvent")] 
public class TimeCounterEvent : ScriptableObject
{
    public UnityAction OnEventRaised;
    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}

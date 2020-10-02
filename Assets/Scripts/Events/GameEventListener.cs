using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent @event;
    public UnityEvent response;

    private void OnEnable()
    {
        @event.RegisterListener(this);
    }

    private void OnDisable()
    {
        @event.UnregisterListener(this);
    }


    /// <summary>
    /// Appelée depuis event.Invoke() ou peut être appelée séparément
    /// </summary>
    public void OnEventRaised()
    {
        response.Invoke();
    }

}
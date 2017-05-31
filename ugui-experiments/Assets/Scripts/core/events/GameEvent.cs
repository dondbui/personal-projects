using UnityEngine.Events;

namespace core.events
{
    /// <summary>
    /// A wrapper for the UnityEvent that takes a parameter of type object
    /// </summary>
    public class GameEvent : UnityEvent<object>
    {
    }
}
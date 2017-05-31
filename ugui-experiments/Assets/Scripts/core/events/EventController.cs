using System.Collections.Generic;
using UnityEngine.Events;

namespace core.events
{
    public class EventController
    {
        private static EventController instance;

        private Dictionary<EventTypeEnum, GameEvent> eventMap;

        private EventController()
        {
            eventMap = new Dictionary<EventTypeEnum, GameEvent>();
        }

        public static EventController GetInstance()
        {
            if (instance == null)
            {
                instance = new EventController();
            }


            return instance;
        }

        /// <summary>
        /// Registers a method to listen to the invokes of a GameEvent
        /// </summary>
        public void RegisterForEvent(EventTypeEnum type, UnityAction<object> callback)
        {
            GameEvent uEvent = null;
            if (!eventMap.ContainsKey(type))
            {
                eventMap[type] = new GameEvent();
            }

            uEvent = eventMap[type];
            uEvent.AddListener(callback);
        }

        /// <summary>
        /// Stops a method from listening for the invokes from a GameEvent
        /// </summary>
        public void UnregisterForEvent(EventTypeEnum type, UnityAction<object> callback)
        {
            GameEvent uEvent = null;
            if (!eventMap.ContainsKey(type))
            {
                return;
            }

            uEvent = eventMap[type];
            uEvent.RemoveListener(callback);
        }

        /// <summary>
        /// Invokes the GameEvent and calls all the listeners attached to it.
        /// </summary>
        public void FireEvent(EventTypeEnum type, object obj)
        {
            GameEvent uEvent = null;

            // Nothing ever registered to listen for this even then just eat it
            // and save us the trouble of having to do the invoke.
            if (!eventMap.ContainsKey(type))
            {
                return;
            }

            uEvent = eventMap[type];

            uEvent.Invoke(obj);
        }

        public void ClearAllListeners()
        {
            // Remove all listeners on the events
            foreach (KeyValuePair<EventTypeEnum, GameEvent> entry in eventMap)
            {
                entry.Value.RemoveAllListeners();
            }
        }
    }
}
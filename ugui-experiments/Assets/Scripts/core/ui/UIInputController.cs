using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace core.ui
{
    /// <summary>
    /// This acts as the central Observer for all of the button
    /// clicks for the UI
    /// </summary>
    public class UIInputController
    {
        public delegate void OnClick(Button button);
        
        private static UIInputController instance;

        private Dictionary<Button, List<OnClick>> callbackMap;

        private UIInputController()
        {
            callbackMap = new Dictionary<Button, List<OnClick>>();
        }

        public static UIInputController GetInstance()
        {
            if (instance == null)
            {
                instance = new UIInputController();
            }

            return instance;
        }

        public void RegisterOnClick(Button button, OnClick callback)
        {
            if (!callbackMap.ContainsKey(button))
            {
                callbackMap[button] = new List<OnClick>();
            }

            List<OnClick> callbackList = callbackMap[button];

            // Already been registered don't care to re-register and have double calls
            if (callbackList.Contains(callback))
            {
                Debug.LogError("Button Listener Already registered!!");
                return;
            }

            callbackList.Add(callback);
        }

        public void UnregisterOnClick(Button button, OnClick callback)
        {
            // Can't unregister something that hasn't been registered
            if (!callbackMap.ContainsKey(button))
            {
                Debug.LogError("Button Key: " + button.name + " never registered!!");
                return;
            }

            List<OnClick> callbackList = callbackMap[button];

            if (!callbackList.Contains(callback))
            {
                Debug.LogError("Button: " + button.name + " never registered!!");
                return;
            }

            callbackList.Remove(callback);
        }

        public void ButtonClicked(Button button)
        {
            Debug.Log("ButtonClicked: " + button.name);

            // Doesn't exist then bounce out
            if (!callbackMap.ContainsKey(button))
            {
                return;
            }

            // Iterate through all the listeners and call them
            List<OnClick> callbackList = callbackMap[button];
            for (int i = 0, count = callbackList.Count; i < count; i++)
            {
                callbackList[i](button);
            }
        }
        
    }
}
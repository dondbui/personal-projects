using core.ui.screens;
using System.Collections.Generic;
using UnityEngine;

namespace core.ui
{
    public class ScreenQueueManager
    {
        private static ScreenQueueManager instance;

        /// <summary>
        /// The linkedlist of screens to be displayed. 
        /// We don't use a queue because we want to be able to inject screens
        /// at varying points in the list.
        /// </summary>
        private LinkedList<GameObject> screenStack;

        public GameObject CurrentScreen { get; private set; }

        private ScreenQueueManager()
        {
            screenStack = new LinkedList<GameObject>();
        }

        public static ScreenQueueManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ScreenQueueManager();
            }

            return instance;
        }

        public bool CanCloseScreen()
        {
            if (CurrentScreen == null)
            {
                return false;
            }

            BaseScreenComponent baseScreen = CurrentScreen.GetComponent<BaseScreenComponent>();

            if (baseScreen == null)
            {
                return false;
            }

            return baseScreen.isCloseable;
        }

        public void CloseCurrentScreen()
        {
            if (!CanCloseScreen())
            {
                Debug.LogError("No CurrentScreen to close!!");
                return;
            }

            GameObject.Destroy(CurrentScreen);
        }

        public void QueueScreen(GameObject screen)
        {
            screen.SetActive(false);
            screenStack.AddLast(screen);
        }

        public void QueueScreenAsNext(GameObject screen)
        {
            screen.SetActive(false);
            screenStack.AddFirst(screen);
        }

        public void ClearQueueAndDestroyAllScreens()
        {
            foreach (GameObject screen in screenStack)
            {
                GameObject.Destroy(screen);
            }

            screenStack.Clear();
        }

        public void ShowNextScreen()
        {
            CurrentScreen = null;

            if (screenStack.Count == 0)
            {
                Debug.Log("All outta screens bub");
                return;
            }

            GameObject screen = screenStack.First.Value;
            screenStack.RemoveFirst();

            screen.SetActive(true);

            CurrentScreen = screen;

            Debug.Log("Now showing Screen: " + screen.name);
        }

        public void ShowScreenNow(GameObject screen)
        {
            screen.SetActive(true);
            CurrentScreen = screen;
        }
    }
}

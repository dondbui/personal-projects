using System.Collections.Generic;
using UnityEngine;

namespace core.ui
{
    public class ScreenQueueManager
    {
        private static ScreenQueueManager instance;

        /// <summary>
        /// The linkedlsit of screens to be displayed. 
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
            if (screenStack.Count == 0)
            {
                Debug.Log("All outta screens bub");
                return;
            }

            GameObject screen = screenStack.First.Value;
            screenStack.RemoveFirst();

            screen.SetActive(true);

            Debug.Log("Now showing Screen: " + screen.name);
        }
    }
}
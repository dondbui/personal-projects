using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace core.ui.screens
{
    public class BaseScreenComponent : MonoBehaviour
    {
        /// <summary>
        /// Is this screeen closable by the ScreenQueueManager? Usually flagged true
        /// and only set to false for special screens such as the TitleScreen
        /// </summary>
        public bool isCloseable = true;

        public virtual void OnDestroy()
        {
            ScreenQueueManager.GetInstance().ShowNextScreen();
        }
    }
}

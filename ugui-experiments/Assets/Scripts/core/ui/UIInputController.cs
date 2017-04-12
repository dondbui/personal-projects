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
        private static UIInputController instance;

        private UIInputController()
        {

        }

        public static UIInputController GetInstance()
        {
            if (instance == null)
            {
                instance = new UIInputController();
            }

            return instance;
        }

        public void ButtonClicked(Button button)
        {
            Debug.Log("ButtonClicked: " + button.name);


        }
        
    }
}
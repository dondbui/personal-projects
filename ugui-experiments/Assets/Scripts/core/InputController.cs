using UnityEngine;

namespace core
{
    /// <summary>
    /// Constantly listens to input from the user's keyboard
    /// </summary>
    public class InputController
    {
        private static InputController instance;

        /// <summary>
        /// Is input currently allowed on the user's part
        /// </summary>
        public bool isActive = true;

        private InputController()
        {
            
        }

        public static InputController GetInstance()
        {
            if (instance == null)
            {
                instance = new InputController();
            }

            return instance;
        }

        public void Update()
        {
            if (!isActive)
            {
                return;
            }

            // Handle the pressing of the escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscape();
            }
        }

        private void HandleEscape()
        {
            Debug.Log("Escape Key Pressed");
        }
    }
}
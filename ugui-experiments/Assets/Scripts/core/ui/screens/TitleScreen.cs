using UnityEngine;
using UnityEngine.UI;

namespace core.ui.screens
{
    public class TitleScreen : MonoBehaviour
    {
        private const string BTN_NEWGAME = "NewGame";
        private const string BTN_CONTINUE = "Continue";

        private Button newGameBtn;
        private Button continueBtn;

        public void Start()
        {
            newGameBtn = GetButtonByName(transform, BTN_NEWGAME);
            continueBtn = GetButtonByName(transform, BTN_CONTINUE);

            UIInputController uic = UIInputController.GetInstance();

            uic.RegisterOnClick(newGameBtn, OnNewGameClicked);
            uic.RegisterOnClick(continueBtn, OnContinueClicked);
        }

        private void OnNewGameClicked(Button button)
        {
            Debug.Log("Handle New Game Button");
        }

        private void OnContinueClicked(Button button)
        {
            Debug.Log("Handle Continue Button");
        }

        private Button GetButtonByName(Transform transform, string name)
        {
            // Try to the find the child transform by name
            Transform btnTransform = transform.Find(name);

            // If we couldn't find it then it'll be null
            if (btnTransform == null)
            {
                return null;
            }

            // Now we can safely get the gameobject off of the found tranform
            GameObject btnObj = btnTransform.gameObject;

            // Return the button component
            return btnObj.GetComponent<Button>();
        }
    }
}
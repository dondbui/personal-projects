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

        private Animator anim;

        private bool isActive = true;

        public void Start()
        {
            anim = GetComponent<Animator>();

            newGameBtn = UIUtils.GetButtonByName(this.gameObject, BTN_NEWGAME);
            continueBtn = UIUtils.GetButtonByName(this.gameObject, BTN_CONTINUE);

            UIInputController uic = UIInputController.GetInstance();

            uic.RegisterOnClick(newGameBtn, OnNewGameClicked);
            uic.RegisterOnClick(continueBtn, OnContinueClicked);


            AnimationClip clip;

            // get the animation clip and add the AnimationEvent
            clip = anim.runtimeAnimatorController.animationClips[0];

            if (clip.name == "ContinueSelectedAnim")
            {
                // new event created
                AnimationEvent evt;
                evt = new AnimationEvent();
                evt.stringParameter = "Continue";
                evt.time = 0.9f;
                evt.functionName = "OnFadeoutComplete";
                clip.AddEvent(evt);
            }
        }

        public void OnDestroy()
        {
            // Unregister button listeners
            UIInputController uic = UIInputController.GetInstance();

            uic.UnregisterOnClick(newGameBtn, OnNewGameClicked);
            uic.UnregisterOnClick(continueBtn, OnContinueClicked);
        }

        public void Update()
        {
            if (!isActive && Input.GetKey(KeyCode.Escape))
            {
                anim.SetTrigger("returnToTitle");
            }
        }

        private void OnNewGameClicked(Button button)
        {
            Debug.Log("Handle New Game Button");
        }

        private void OnContinueClicked(Button button)
        {
            Debug.Log("Handle Continue Button");
            anim.SetTrigger("continue");
        }

        public void OnFadeoutComplete()
        {
            isActive = false;

            Debug.Log("Handle Anim Complete");

            // Show the load file screen
            GameObject resourceObj = Resources.Load<GameObject>("Prefabs/LoadFileScreen");

            GameObject screen = GameObject.Instantiate(resourceObj);
            screen.name = "LoadFileScreen";

            // Add it to the title game object
            GameObject MainMenu = GameObject.Find("MainMenu");

            screen.transform.SetParent(MainMenu.transform, false);
        }

    }
}
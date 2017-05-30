using UnityEngine;
using UnityEngine.UI;

namespace core.ui.screens
{
    public class TitleScreen : BaseScreenComponent
    {
        private const string BTN_NEWGAME = "NewGame";
        private const string BTN_CONTINUE = "Continue";

        private const string TRG_CONTINUE = "continue";
        private const string TRG_RETURN_TO_TITLE = "returnToTitle";

        private Button newGameBtn;
        private Button continueBtn;

        private Animator anim;

        private bool isActive = true;

        public void Start()
        {
            // This guy is not closeable from the Screen Queue since it is the title screen
            base.isCloseable = false;

            ScreenQueueManager.GetInstance().ShowScreenNow(this.gameObject);

            anim = GetComponent<Animator>();

            newGameBtn = UIUtils.GetButtonByName(this.gameObject, BTN_NEWGAME);
            continueBtn = UIUtils.GetButtonByName(this.gameObject, BTN_CONTINUE);

            UIInputController uic = UIInputController.GetInstance();

            uic.RegisterOnClick(newGameBtn, OnNewGameClicked);
            uic.RegisterOnClick(continueBtn, OnContinueClicked);

            AnimationClip[] animClips = anim.runtimeAnimatorController.animationClips;

            // Check all the animation clips and add AnimationEvents to the appropriate ones
            for (int i = 0, count = animClips.Length; i < count; i++)
            {
                AnimationClip clip = anim.runtimeAnimatorController.animationClips[i];
                AnimationEvent evt;
                
                // Add in the event to call OnFadeOutComplete after the animation is done.
                if (clip.name == "ContinueSelectedAnim")
                {
                    evt = new AnimationEvent();
                    evt.stringParameter = "Continue";
                    evt.time = clip.length;
                    evt.functionName = "OnContinueSelectedAnimComplete";
                    clip.AddEvent(evt);
                }

                // Add in the animation event to call OnFadeInComplete after the animation is done.
                if (clip.name == "ReturnToTitle")
                {
                    evt = new AnimationEvent();
                    evt.stringParameter = "BackToTitle";
                    evt.time = clip.length;
                    evt.functionName = "OnFadeInComplete";
                    clip.AddEvent(evt);
                }
            }
        }

        public void OnDestroy()
        {
            // Unregister button listeners
            UIInputController uic = UIInputController.GetInstance();

            uic.UnregisterOnClick(newGameBtn, OnNewGameClicked);
            uic.UnregisterOnClick(continueBtn, OnContinueClicked);
        }

        private void OnEnable()
        {
            // Upon re-enabling reset the animation triggers
            if (anim != null)
            {
                anim.ResetTrigger(TRG_CONTINUE);
                anim.ResetTrigger(TRG_RETURN_TO_TITLE);
                anim.SetTrigger(TRG_RETURN_TO_TITLE);
            }
        }

        private void OnNewGameClicked(Button button)
        {
            if (!isActive)
            {
                return;
            }

            Debug.Log("Handle New Game Button");
        }

        private void OnContinueClicked(Button button)
        {
            if (!isActive)
            {
                return;
            }

            SetButtonStatus(false);
            Debug.Log("Handle Continue Button");
            anim.ResetTrigger(TRG_RETURN_TO_TITLE);
            anim.ResetTrigger(TRG_CONTINUE);
            anim.SetTrigger(TRG_CONTINUE);
        }

        private void SetButtonStatus(bool state)
        {
            continueBtn.interactable = state;
            newGameBtn.interactable = state;
        }

        /// <summary>
        /// Handles the creation of the LoadFileScreen after the title screen has transitioned
        /// </summary>
        public void OnContinueSelectedAnimComplete()
        {
            isActive = false;
            Debug.Log("Handle Anim Complete");

            // Add it to the title game object
            GameObject MainMenu = GameObject.Find("MainMenu");

            GameObject loadFileScreen = UIFactory.CreateScreen(UIFactory.SCR_LOAD_FILE, MainMenu);

            ScreenQueueManager sqm = ScreenQueueManager.GetInstance();

            sqm.ShowScreenNow(loadFileScreen);
            sqm.QueueScreenAsNext(this.gameObject);
        }

        /// <summary>
        /// After fading the screen back in we need to flag it back as active.
        /// </summary>
        public void OnFadeInComplete()
        {
            isActive = true;
            SetButtonStatus(true);
        }
    }
}
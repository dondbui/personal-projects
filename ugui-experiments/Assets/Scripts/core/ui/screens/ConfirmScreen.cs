using UnityEngine;
using UnityEngine.UI;

namespace core.ui.screens
{
    public class ConfirmScreen : BaseScreenComponent
    {
        public delegate void ChoiceCallback();

        private const string TXT_TITLE = "Title";
        private const string TXT_BODY = "Body";
        private const string BTN_YES = "BtnYes";
        private const string BTN_NO = "BtnNo";

        private Text txtTitle;
        private Text txtBody;
        private Button btnYes;
        private Button btnNo;

        private string title;
        private string body;
        private ChoiceCallback yesCallback;
        private ChoiceCallback noCallback;

        // Use this for initialization
        public void Start()
        {
            txtTitle = UIUtils.GetComponentFromGameObjectName<Text>(gameObject, TXT_TITLE);
            txtBody = UIUtils.GetComponentFromGameObjectName<Text>(gameObject, TXT_BODY);
            btnYes = UIUtils.GetComponentFromGameObjectName<Button>(gameObject, BTN_YES);
            btnNo = UIUtils.GetComponentFromGameObjectName<Button>(gameObject, BTN_NO);

            txtTitle.text = title;
            txtBody.text = body;

            btnYes.onClick.AddListener(OnYes);
            btnNo.onClick.AddListener(OnNo);
        }

        public void SetData(string title, string body, ChoiceCallback yesCallback, ChoiceCallback noCallback)
        {
            this.title = title;
            this.body = body;
            this.yesCallback = yesCallback;
            this.noCallback = noCallback;
        }

        private void OnYes()
        {
            if (yesCallback != null)
            {
                yesCallback();
            }

            CloseScreen();
        }

        private void OnNo()
        {
            if (noCallback != null)
            {
                noCallback();
            }

            CloseScreen();
        }

        public override void OnDestroy()
        {
            btnYes.onClick.RemoveAllListeners();
            btnNo.onClick.RemoveAllListeners();

            base.OnDestroy();
        }

        private void CloseScreen()
        {
            Destroy(this.gameObject);
        }
    }
}
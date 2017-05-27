using UnityEngine;
using UnityEngine.UI;

namespace core.ui
{
    /// <summary>
    /// This component is there to forward button behavior over to
    /// the UIInputController.
    /// </summary>
    public class ButtonComponent : MonoBehaviour
    {
        public Button button;


        public virtual void Start()
        {
            button = this.gameObject.GetComponent<Button>();
            button.onClick.AddListener(OnClicked);
        }

        protected virtual void OnClicked()
        {
            UIInputController.GetInstance().ButtonClicked(button);
        }
    }
}
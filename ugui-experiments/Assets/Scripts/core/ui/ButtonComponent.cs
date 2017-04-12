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
        private Button button;


        void Start()
        {
            button = this.gameObject.GetComponent<Button>();
        }

        public void OnClicked()
        {
            UIInputController.GetInstance().ButtonClicked(button);
        }
    }
}
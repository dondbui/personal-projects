
using UnityEngine;
using UnityEngine.UI;

namespace core.ui
{
    public class UIUtils
    {
        public static Button GetButtonByName(GameObject parentObj, string name)
        {
            // Try to the find the child transform by name
            Transform btnTransform = parentObj.transform.Find(name);

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
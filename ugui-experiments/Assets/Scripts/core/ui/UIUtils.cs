
using UnityEngine;
using UnityEngine.UI;

namespace core.ui
{
    public class UIUtils
    {
        public static T GetComponentFromGameObjectName<T>(GameObject parentObj, string name) where T: Component
        {
            // Try to the find the child transform by name
            Transform transf = parentObj.transform.Find(name);

            // If we couldn't find it then it'll be null
            if (transf == null)
            {
                return null;
            }

            // Now we can safely get the gameobject off of the found tranform
            GameObject obj = transf.gameObject;

            // Return the component
            return obj.GetComponent<T>();
        }

        public static Button GetButtonByName(GameObject parentObj, string name)
        {
            return GetComponentFromGameObjectName<Button>(parentObj, name);
        }
    }
}
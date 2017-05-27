
using UnityEngine;

namespace core.ui
{
    /// <summary>
    /// A factory class meant to contain static methods to create new instances
    /// of UI Elements. Screens, modules, etc.
    /// </summary>
    public class UIFactory
    {
        /// <summary>
        /// The prefab for the load file screen where the user can load their
        /// last saved game. 
        /// </summary>
        public const string SCR_LOAD_FILE = "Prefabs/LoadFileScreen";
        public const string SCR_CONFIRM = "Prefabs/Screens/ConfirmScreen";

        public static GameObject CreateScreen(string screenName, GameObject parent)
        {
            // Show the load file screen
            GameObject resourceObj = Resources.Load<GameObject>(screenName);

            GameObject screen = GameObject.Instantiate(resourceObj);

            string[] pathPieces = screenName.Split('/');


            screen.name = pathPieces[pathPieces.Length - 1];

            if (parent != null)
            {
                screen.transform.SetParent(parent.transform, false);
            }

            return screen;
        }
    }
}

using UnityEngine;

namespace core.ui
{
    /// <summary>
    /// A factory class meant to contain static methods to create new instances
    /// of UI Elements. Screens, modules, etc.
    /// </summary>
    public class UIFactory
    {
        public const string PREFAB_SCREEN_PATH = "Prefabs/Screens/";

        /// <summary>
        /// The prefab for the load file screen where the user can load their
        /// last saved game. 
        /// </summary>
        public const string SCR_LOAD_FILE = "LoadFileScreen";
        public const string SCR_CONFIRM = "ConfirmScreen";

        public static T CreateScreenAndAddComponent<T>(string screenName, GameObject parent) where T: MonoBehaviour
        {
            GameObject screen = CreateScreen(screenName, parent);

            T component = null;
            if (screen != null)
            {
                component = screen.AddComponent<T>();
            }

            return component;
        }

        public static GameObject CreateScreen(string screenName, GameObject parent)
        {
            // Show the load file screen
            GameObject resourceObj = Resources.Load<GameObject>(PREFAB_SCREEN_PATH + screenName);

            GameObject screen = GameObject.Instantiate(resourceObj);

            screen.name = screenName;

            if (parent != null)
            {
                screen.transform.SetParent(parent.transform, false);
            }

            return screen;
        }
    }
}
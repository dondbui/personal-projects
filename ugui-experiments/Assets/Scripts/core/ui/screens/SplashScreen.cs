using UnityEngine;
using UnityEngine.SceneManagement;

namespace core.ui.screens
{
    public class SplashScreen : MonoBehaviour
    {

        // Use this for initialization
        public void Start()
        {
            Debug.Log("Game Start");

            Invoke("GoToMainScene", 4);
        }

        private void GoToMainScene()
        {
            Debug.Log("Opening Main Scene");
            SceneManager.LoadScene("MainScene");
        }
    }


}

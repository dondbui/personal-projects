using UnityEngine;

namespace core.ui.screens
{
    public class LoadFileScreen : MonoBehaviour
    {

        // Use this for initialization
        public void Start()
        {

        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Debug.Log("Destroy LoadFileScreen");

                Destroy(this.gameObject);
            }
        }
    }
}
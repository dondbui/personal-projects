using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace core
{
    public class GameLoop : MonoBehaviour
    {
        private InputController inputController;

        // Use this for initialization
        public void Start()
        {
            inputController = InputController.GetInstance();
        }

        // Update is called once per frame
        public void Update()
        {
            if (inputController != null)
            {
                inputController.Update();
            }
        }
    }
}
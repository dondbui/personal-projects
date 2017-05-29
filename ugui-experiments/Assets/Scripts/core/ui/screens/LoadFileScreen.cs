using core.ui.modules;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace core.ui.screens
{
    /// <summary>
    /// Handles the functionality of the load file selection screen.
    /// </summary>
    public class LoadFileScreen : MonoBehaviour
    {
        private GridLayoutGroup gridLayoutGroup;

        private GameObject cachedSaveFileAsset;


        // Use this for initialization
        public void Start()
        {
            gridLayoutGroup = UIUtils.GetComponentFromGameObjectName<GridLayoutGroup>(gameObject, "SaveFileGridPanel");

            // add all the save slots
            for (int i = 0; i < 4; i++)
            {
                
                if (i == 0)
                {
                    DateTime creationDate = new DateTime(2017, 3, 21, 0, 0, 0, DateTimeKind.Utc);
                    DateTime lastSaveDate = new DateTime(2017, 4, 1, 3, 55, 0, DateTimeKind.Utc);
                    GetNewSaveFileButton(creationDate, lastSaveDate, i + 1, "HoneyBear");
                }
                else
                {
                    GetNewSaveFileButton(DateTime.MinValue, DateTime.MinValue, 9, "");
                }
            }
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Close();
            }
        }

        public void Close()
        {
            Debug.Log("Destroy LoadFileScreen");

            Destroy(this.gameObject);
        }

        private GameObject GetNewSaveFileButton(
            DateTime creationDate, DateTime lastSaveDate, int slot, string playerName)
        {
            if (cachedSaveFileAsset == null)
            {
                // Show the load file screen
                cachedSaveFileAsset = Resources.Load<GameObject>("Prefabs/SaveButton");
            }

            GameObject saveFileButton = GameObject.Instantiate(cachedSaveFileAsset);
            saveFileButton.name = "saveFile";

            // Add it to the title game object
            GameObject MainMenu = GameObject.Find("SaveFileGridPanel");

            saveFileButton.transform.SetParent(MainMenu.transform, false);
            SaveFileButtonModule module = saveFileButton.AddComponent<SaveFileButtonModule>();
            module.SetData(creationDate, lastSaveDate, slot, playerName);

            return saveFileButton;
        }
    }
}
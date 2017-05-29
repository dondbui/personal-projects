
using core.ui.screens;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace core.ui.modules
{
    /// <summary>
    /// The UI module to handle the save file clickable button element
    /// </summary>
    public class SaveFileButtonModule : ButtonComponent
    {
        private const string TXT_LAST_SAVE_DATE = "LastSaveDate";
        private const string TXT_PLAY_DURATION = "PlayDur";
        private const string TXT_PLAYER_NAME = "PlayerName";
        private const string TXT_SAVE_NUM = "SaveFileNumText";

        private Text lastSaveText;
        private Text playDurText;
        private Text playerNameText;
        private Text saveFileNumText;

        private DateTime creationDate;
        private DateTime lastSaveDate;

        private int slotNum;
        private string playerName;

        private GameObject parentScreen;

        /// <summary>
        /// Sets the data prior to the monobehavior hitting start
        /// </summary>
        public void SetData(DateTime creationDate, DateTime lastSaveDate, int slotNum, string playerName)
        {
            this.creationDate = creationDate;
            this.lastSaveDate = lastSaveDate;
            this.slotNum = slotNum;
            this.playerName = playerName;
        }

        public override void Start()
        {
            base.Start();

            parentScreen = GameObject.Find(UIFactory.SCR_LOAD_FILE);

            lastSaveText = UIUtils.GetComponentFromGameObjectName<Text>(gameObject, TXT_LAST_SAVE_DATE);
            playDurText = UIUtils.GetComponentFromGameObjectName<Text>(gameObject, TXT_PLAY_DURATION);
            playerNameText = UIUtils.GetComponentFromGameObjectName<Text>(gameObject, TXT_PLAYER_NAME);
            saveFileNumText = UIUtils.GetComponentFromGameObjectName<Text>(gameObject, TXT_SAVE_NUM);

            if (creationDate == DateTime.MinValue)
            {
                SetEmptySlot();
                return;
            }

            button.interactable = true;
            saveFileNumText.text = "Save File " + slotNum;
            playerNameText.text = "Captain " + playerName;

            lastSaveText.text = lastSaveDate.ToShortDateString() + " " + lastSaveDate.ToShortTimeString();

            TimeSpan duration = lastSaveDate.Subtract(creationDate);
            playDurText.text = duration.ToString();
        }

        protected override void OnClicked()
        {
            Debug.Log("Save File Button Clicked");

            base.OnClicked();
            GameObject MainMenu = GameObject.Find("MainMenu");
            ConfirmScreen screenComp = 
                UIFactory.CreateScreenAndAddComponent<ConfirmScreen>(UIFactory.SCR_CONFIRM, MainMenu);

            ScreenQueueManager sqm = ScreenQueueManager.GetInstance();
            sqm.ShowScreenNow(screenComp.gameObject);
            sqm.QueueScreenAsNext(parentScreen);

            screenComp.SetData("Load this file?", "Are you sure you want to load this file?", OnYes, OnNo);
        }

        private void OnYes()
        {
            LoadFileScreen screenComp = parentScreen.GetComponent<LoadFileScreen>();
            ScreenQueueManager.GetInstance().ClearQueueAndDestroyAllScreens();
            screenComp.Close();
        }

        private void OnNo()
        {
        }

        private void SetEmptySlot()
        {
            button.interactable = false;

            lastSaveText.text = "";
            saveFileNumText.text = "EMPTY";
            playerNameText.text = "";
            playDurText.text = "";
        }

    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
    public class MenuController : MonoBehaviour
    {
        public GameObject HomeTab;
        public GameObject PlayTab;
        public GameObject SettingsTab;

        public Button HomeButton;
        public Button PlayButton;
        public Button SettingsButton;

        #region Private Fields and Methods

        private GameObject currentTab;
        private Button currentButton;

        private void ToggleTab(string tab)
        {
            Vector3 upVector = new Vector3(0, Screen.height);
            Vector3 downVector = new Vector3(0, -Screen.height);
            
            currentTab.transform.position += upVector;
            currentButton.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.UpperCase;
            
            switch (tab)
            {
                case "home":
                    HomeTab.transform.position += downVector;
                    currentTab = HomeTab;
                    currentButton = HomeButton;
                    break;
                case "play":
                    PlayTab.transform.position += downVector;
                    currentTab = PlayTab;
                    currentButton = PlayButton;
                    break;
                case "settings":
                    SettingsTab.transform.position += downVector;
                    currentTab = SettingsTab;
                    currentButton = SettingsButton;
                    break;
            }
            
            currentButton.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.UpperCase;
            currentButton.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
        }

        #endregion
        

        #region MonoBehavior Callbacks

        private void Start()
        {
            currentTab = HomeTab;
            currentButton = HomeButton;
            PlayTab.transform.position += new Vector3(0, Screen.height);
            SettingsTab.transform.position += new Vector3(0, Screen.height);
        }

        private void Update()
        {
        }

        private void OnApplicationQuit()
        {
            if (PlayerPrefs.GetInt(PlayerPrefKeys.SaveCredentials) != 1) // Logout at application quit if the user have chosen not to save credentials 
            {
                Firebase.AuthHandler.LogOut();
            }
        }

        #endregion
        

        #region Button Handlers

        public void HomeHandler()
        {
            ToggleTab("home");
        }
        
        public void PlayHandler()
        {
            ToggleTab("play");
        }

        public void SettingsHandler()
        {
            ToggleTab("settings");
        }

        public void QuitHandler()
        {
            Application.Quit();
        }

        #endregion
    }
}

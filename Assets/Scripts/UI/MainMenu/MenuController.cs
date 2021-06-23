using System;
using System.Collections;
using System.Collections.Generic;
using PlayerManagement;
using Scripts;
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

        public GameObject Selector;

        public Button HomeButton;
        public Button PlayButton;
        public Button SettingsButton;

        #region Private Fields and Methods

        private GameObject currentTab;
        private Button currentButton;
        private string currentTabStr = "home";

        private void ToggleTab(string tab)
        {
            if (tab == currentTabStr)
            {
                return;
            }
            
            Vector3 upVector = new Vector3(Screen.width, 0);
            Vector3 downVector = new Vector3(-Screen.width, 0);
            
            SmoothToggle(currentTab.transform, upVector);

            currentButton.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.UpperCase;
            
            switch (tab)
            {
                case "home":
                    HomeTab.transform.position += 2 * downVector;
                    SmoothToggle(HomeTab.transform, upVector);
                    currentTab = HomeTab;
                    currentButton = HomeButton;
                    currentTabStr = "home";
                    break;
                case "play":
                    PlayTab.transform.position += 2 * downVector;
                    SmoothToggle(PlayTab.transform, upVector);
                    currentTab = PlayTab;
                    currentButton = PlayButton;
                    currentTabStr = "play";
                    break;
                case "settings":
                    SettingsTab.transform.position += 2 * downVector;
                    SmoothToggle(SettingsTab.transform, upVector);
                    currentTab = SettingsTab;
                    currentButton = SettingsButton;
                    currentTabStr = "settings";
                    break;
            }
            
            currentButton.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.UpperCase;
            currentButton.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
            Selector.transform.position = currentButton.transform.position;
        }

        private void SmoothToggle(Transform transform, Vector3 vector3, Action callback = null)
        {
            var initpos = transform.position;
            
            StartCoroutine(Utils.SmoothTransition(
                f => transform.position = Vector3.Lerp(transform.position,
                    initpos + vector3, f),
                0.5f, callback));
        }

        #endregion
        

        #region MonoBehavior Callbacks

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            currentTab = HomeTab;
            currentButton = HomeButton;
            PlayTab.transform.position += new Vector3(Screen.width, 0);
            SettingsTab.transform.position += new Vector3(Screen.width, 0);
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

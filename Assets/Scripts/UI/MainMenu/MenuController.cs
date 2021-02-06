using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
    public class MenuController : MonoBehaviour
    {
        public GameObject HomeTab;
        public GameObject SoloTab;
        public GameObject MultiTab;
        public GameObject SettingsTab;
        
        private void OnApplicationQuit()
        {
            if (PlayerPrefs.GetInt(PlayerPrefKeys.SaveCredentials) != 1) // Logout at application quit if the user have chosen not to save credentials 
            {
                Firebase.AuthHandler.LogOut();
            }
        }

        private void ToggleTab(string tab)
        {
            HomeTab.SetActive(false);
            SoloTab.SetActive(false);
            MultiTab.SetActive(false);
            SettingsTab.SetActive(false);

            switch (tab)
            {
                case "home":
                    HomeTab.SetActive(true);
                    break;
                case "solo":
                    SoloTab.SetActive(true);
                    break;
                case "multi":
                    MultiTab.SetActive(true);
                    break;
                case "settings":
                    SettingsTab.SetActive(true);
                    break;
            }
        }

        private void Start()
        {
            ToggleTab("home");
        }

        public void HomeHandler()
        {
            ToggleTab("home");
        }

        public void SoloHandler()
        {
            ToggleTab("solo");
        }

        public void MultiplayerHandler()
        {
            ToggleTab("multi");
        }

        public void SettingsHandler()
        {
            ToggleTab("settings");
        }

        public void QuitHandler()
        {
            Application.Quit();
        }
    }
    
}

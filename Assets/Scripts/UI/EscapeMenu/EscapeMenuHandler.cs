using System;
using PlayerManagement;
using UI.MainMenu.Tabs.SettingsTab;
using UnityEngine;

namespace UI.EscapeMenu
{
    public class EscapeMenuHandler : MonoBehaviour
    {
        public GameObject EscapeMenuContainer;
        public GameObject SettingsMenu;
        
        public void ResumeButtonHandler()
        {
            EscapeMenuContainer.SetActive(false);
            SettingsMenu.SetActive(false);
        }

        public void SettingsButtonHandler()
        {
            SettingsMenu.SetActive(!SettingsMenu.activeInHierarchy);
        }

        private void Start()
        {
            ResumeButtonHandler();
            SettingsMenu.GetComponent<SettingsFormHandler>().EscapeMenu = gameObject;
        }
    }
}

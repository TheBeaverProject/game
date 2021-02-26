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
            
            
            //Locks the cursor in the center of the screen
            Cursor.lockState = CursorLockMode.Locked;
            //Reenable the mouse look
            GetComponent<Canvas>().worldCamera.GetComponent<MouseLook>().enabled = true;
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (EscapeMenuContainer.activeInHierarchy)
                {
                    ResumeButtonHandler();
                }
                else
                {
                    EscapeMenuContainer.SetActive(true);
                    //Unlocks the cursor for interaction with the menus
                    Cursor.lockState = CursorLockMode.None;
                    //Disable the mouse look script to fix the camera
                    GetComponent<Canvas>().worldCamera.GetComponent<MouseLook>().enabled = false;
                }
            }
        }
    }
}

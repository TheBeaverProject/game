using System;
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
        }

        public void SettingsButtonHandler()
        {
            SettingsMenu.SetActive(!SettingsMenu.activeInHierarchy);
        }

        private void Start()
        {
            ResumeButtonHandler();
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
                }
            }
        }
    }
}

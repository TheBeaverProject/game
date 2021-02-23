using System;
using UnityEngine;

namespace UI.EscapeMenu
{
    public class EscapeMenuHandler : MonoBehaviour
    {
        public GameObject EscapeMenuContainer;

        public void ResumeButtonHandler()
        {
            EscapeMenuContainer.SetActive(false);
            //Locks the cursor in the center of the screen
            Cursor.lockState = CursorLockMode.Locked;
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

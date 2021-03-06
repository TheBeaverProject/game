﻿using System;
using PlayerManagement;
using UI.BuyMenu;
using UI.EscapeMenu;
using UnityEngine;

namespace UI
{
    public class PlayerMenusHandler : MonoBehaviour
    {
        public GameObject EscapeMenu;
        public GameObject BuyMenu;
        public GameObject HUD;
        public Camera playerCamera;

        private EscapeMenuHandler EscapeMenuController;
        private BuyMenuController BuyMenuController;
        private PlayerManager _playerManager;
        private MouseLook _mouseLook;

        private void Start()
        {
            EscapeMenuController = EscapeMenu.GetComponent<EscapeMenuHandler>();
            BuyMenuController = BuyMenu.GetComponent<BuyMenuController>();
            _mouseLook = playerCamera.GetComponent<MouseLook>();
            _playerManager = GetComponent<PlayerManager>();
            
            BuyMenuController.Container.SetActive(false);
            EscapeMenuController.EscapeMenuContainer.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B) && !EscapeMenuController.EscapeMenuContainer.activeInHierarchy)
            {
                BuyMenuController.Container.SetActive(true);
                UnLockCursor();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (BuyMenuController.Container.activeInHierarchy)
                {
                    BuyMenuController.PreviewContainer.GetComponent<WeaponPreview>().ClosePreview();
                    BuyMenuController.Container.SetActive(false);
                    LockCursor();
                    return;
                }
                
                if (EscapeMenuController.EscapeMenuContainer.activeInHierarchy)
                {
                    EscapeMenuController.ResumeButtonHandler();
                    LockCursor();
                }
                else
                {
                    EscapeMenuController.EscapeMenuContainer.SetActive(true);
                    UnLockCursor();
                }
            }
        }

        private void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            //Reenable the mouse look
            _mouseLook.followCursor = true;
            _playerManager.EnableShooting();
            HUD.SetActive(true);
        }

        private void UnLockCursor()
        {
            //Unlocks the cursor for interaction with the menus
            Cursor.lockState = CursorLockMode.None;
            //Disable the mouse look script to fix the camera
            _mouseLook.followCursor = false;
            _playerManager.DisableShooting();
            HUD.SetActive(false);
        }
    }
}
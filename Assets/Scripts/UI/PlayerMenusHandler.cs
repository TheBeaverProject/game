using System;
using Photon.Pun;
using PlayerManagement;
using UI.BuyMenu;
using UI.EscapeMenu;
using UnityEngine;

namespace UI
{
    public class PlayerMenusHandler : MonoBehaviourPun
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
            if (!PhotonNetwork.OfflineMode && PhotonNetwork.IsConnected && !photonView.IsMine)
            {
                return;
            }
            
            EscapeMenuController = EscapeMenu.GetComponent<EscapeMenuHandler>();
            BuyMenuController = BuyMenu.GetComponent<BuyMenuController>();
            _mouseLook = playerCamera.GetComponent<MouseLook>();
            _playerManager = GetComponent<PlayerManager>();
            
            BuyMenuController.Container.SetActive(false);
            EscapeMenuController.EscapeMenuContainer.SetActive(false);
        }

        private void Update()
        {
            if (!PhotonNetwork.OfflineMode && PhotonNetwork.IsConnected && !photonView.IsMine)
            {
                return;
            }
            
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
                }
                else
                {
                    EscapeMenuController.EscapeMenuContainer.SetActive(true);
                    UnLockCursor();
                }
            }
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            //Reenable the mouse look
            _mouseLook.followCursor = true;
            _playerManager.EnableShooting();
            HUD.SetActive(true);
        }

        public void UnLockCursor()
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
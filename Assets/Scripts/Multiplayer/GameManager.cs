using System;
using Photon.Pun;
using Photon.Realtime;
using PlayerManagement;
using UI;
using UI.BuyMenu;
using UI.HUD;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Serialized Fields

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        
        [Tooltip("Camera used for the view of the client")]
        public Camera cameraPrefab;
        
        [Tooltip("Camera used for the view of the client's weapon")]
        public Camera weaponCameraPrefab;
        
        [Tooltip("HUD of the client")]
        public GameObject hudPrefab;
        
        [Tooltip("Escape Menu of the client")]
        public GameObject ESCPrefab;
        
        [Tooltip("Buy Menu")]
        public GameObject BuyMenuPrefab;

        [Tooltip("GameObject used to display scopes when aiming")]
        public GameObject ScopePrefab;

        public Vector3 playerStartPos;

        #endregion

        #region Photon Callbacks

        /// <summary>
        /// Loads the Main Menu scene when the player leaves the Room
        /// </summary>
        public override void OnLeftRoom()
        {
            Debug.Log("PhotonNetwork: Player Left the room");
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"PhotonNetwork: Player {newPlayer.NickName} Entered the room.");

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log($"PhotonNetwork: Client is MasterClient");
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"PhotonNetwork: Player {otherPlayer.NickName} Left the room.");
            
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log($"PhotonNetwork: Client is MasterClient");
            }
        }

        #endregion

        #region MonoBehaviours callbacks

        private void Start()
        {
            playerStartPos = new Vector3(0f, 5f, 0f);
            
            if (playerPrefab == null)
            {
                Debug.LogError("Missing PlayerPrefab Reference.");   
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.Log($"Instantiating LocalPlayer from {SceneManagerHelper.ActiveSceneName}");
                    InstantiateLocalPlayer();
                }
                else
                {
                    Debug.LogFormat($"Ignoring scene load for {SceneManagerHelper.ActiveSceneName}");
                }
            }
        }

        private void Update()
        {
            // TEMP: Respawn player if he is dead
            if (PlayerManager.LocalPlayerInstance == null) return;
            
            var playerManager = PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>();
            if (playerManager.Health > 0) return;

            RespawnPlayer();
        }

        #endregion

        #region Private Methods

        private void RespawnPlayer()
        {
            PhotonNetwork.Destroy(PlayerManager.LocalPlayerInstance);
            
            InstantiateLocalPlayer();
        }

        private GameObject clientPlayer;
        private Camera clientCamera;
        private GameObject clientHUD;
        private GameObject clientESCMenu;
        private GameObject clientBuyMenu;
        private GameObject clientScopeHUD;
        private Camera clientWeaponCamera;

        private void InstantiateLocalPlayer()
        {
            // Instantiate the Object of the localPlayer
            // Using PhotonNetwork to make it present on the network
            clientPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, playerStartPos, Quaternion.identity, 0);
            clientPlayer.name = PhotonNetwork.NickName;
            
            var initPos = clientPlayer.transform.position;

            // Add a camera and a HUD only on the player representing the client to have a single camera/hud per game scene and avoid confusion
            clientCamera = Instantiate(cameraPrefab, initPos, Quaternion.identity, clientPlayer.transform);
            clientWeaponCamera = Instantiate(weaponCameraPrefab, clientCamera.transform.position, Quaternion.identity, clientCamera.transform);
            clientHUD = Instantiate(hudPrefab, initPos, Quaternion.identity, clientPlayer.transform);
            clientESCMenu = Instantiate(ESCPrefab, initPos, Quaternion.identity, clientPlayer.transform);
            clientBuyMenu = Instantiate(BuyMenuPrefab, initPos, Quaternion.identity, clientPlayer.transform);
            clientScopeHUD = Instantiate(ScopePrefab, initPos, Quaternion.identity, clientPlayer.transform);
            
            // Assing the PlayerManager on the Buy Menu
            clientBuyMenu.GetComponent<BuyMenuController>().playerManager = clientPlayer.GetComponent<PlayerManager>();
            
            // Assing the camera to the player object
            clientPlayer.GetComponent<PlayerManager>().playerCamera = clientCamera;
            clientPlayer.GetComponent<PlayerManager>().weaponCamera = clientWeaponCamera;
            
            // Assign the HUD to the playerManager
            clientPlayer.GetComponent<PlayerManager>().HUD = clientHUD.GetComponent<Controller>();

            // Place the camera correctly and set the FOV according to the Player's settings
            clientCamera.GetComponent<MouseLook>().playerBody = clientPlayer.transform;
            clientCamera.transform.position += new Vector3(0, 0.8f);
            clientCamera.fieldOfView = PlayerPrefs.HasKey(PlayerPrefKeys.FOV) ? PlayerPrefs.GetInt(PlayerPrefKeys.FOV) : 70;

            // Associate the HUD to the Render Camera
            InitCameraOnUIElement(clientHUD, clientCamera);
            clientESCMenu.GetComponent<Canvas>().worldCamera = clientCamera;
            
            // Assign the menus to control to the player's controller
            var playerMenusHandler = clientPlayer.GetComponent<PlayerMenusHandler>();
            playerMenusHandler.BuyMenu = clientBuyMenu;
            playerMenusHandler.EscapeMenu = clientESCMenu;
            playerMenusHandler.playerCamera = clientCamera;
            playerMenusHandler.HUD = clientHUD;
        }

        private static void InitCameraOnUIElement(GameObject uiEl, Camera ccam)
        {
            uiEl.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay; // TODO: Switch back to ScreenSpaceCamera if needed to reenable glitch effects
            uiEl.GetComponent<Canvas>().worldCamera = ccam;
            uiEl.GetComponent<Canvas>().planeDistance = 1f;
        }

        #endregion

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
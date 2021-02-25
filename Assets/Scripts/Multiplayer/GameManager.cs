using System;
using Photon.Pun;
using Photon.Realtime;
using PlayerManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        
        [Tooltip("Camera used for the view of the client")]
        public Camera cameraPrefab;
        
        [Tooltip("HUD of the client")]
        public GameObject hudPrefab;
        
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

        private void Start()
        {
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

        private void InstantiateLocalPlayer()
        {
            // Instantiate the Object of the localPlayer
            // Using PhotonNetwork to make it present on the network
            var clientPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,5f,0f), Quaternion.identity, 0);
            var initPos = clientPlayer.transform.position;
            
            // Add a camera and a HUD only on the player representing the client to have a single camera/hud per game scene and avoid confusion
            var clientCamera = Instantiate(cameraPrefab, initPos, Quaternion.identity, clientPlayer.transform);
            var clientHUD = Instantiate(hudPrefab, initPos, Quaternion.identity,
                clientPlayer.transform);

            clientCamera.GetComponent<MouseLook>().playerBody = clientPlayer.transform;
            clientCamera.transform.position += new Vector3(0, 0.8f);

            clientHUD.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            clientHUD.GetComponent<Canvas>().worldCamera = clientCamera;
            clientHUD.GetComponent<Canvas>().planeDistance = 1f;
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
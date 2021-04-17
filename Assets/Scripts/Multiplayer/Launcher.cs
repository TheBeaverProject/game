using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Scripts.Gamemodes;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Multiplayer
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Client's version number
        /// </summary>
        private string gameVersion = "0.0.1";
        
        /// <summary>
        /// The maximum number of player that can be in a single room
        /// </summary>
        [Tooltip("The maximum number of player that can be in a single room")]
        [SerializeField]
        private byte maxPlayersPerRoom = 10;

        [Tooltip("UI Panel for the user to enter its name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        
        [Tooltip("UI Label to inform the user of the connection progress")]
        [SerializeField]
        private GameObject progressLabel;

        public GamemodeSelection GamemodeSelection;

        private void Start()
        {
            if (controlPanel == null)
            {
                return; // Using in standalone mode
            }
            
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        /// <summary>
        /// MonoBehavior method called on GameObject by Unity during early initialisation phase
        /// </summary>
        private void Awake()
        {
            // #Critical
            // Makes sure the PhotonNetwork.LoadLevel() can be called and all the clients in the same room can sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// Kepp track of the current process to properly adjust behavior
        /// when receiving callbacks.
        /// Typically used for the OnConnectedToMaster() callback
        /// </summary>
        private bool isConnecting;

        public void Connect()
        {
            PhotonNetwork.NickName = Firebase.AuthHandler.loggedinUser.Username;
            
            if (controlPanel != null)
            {
                progressLabel.SetActive(true);
                controlPanel.SetActive(false);
            }
            
            // Checks if the client is connected
            if (PhotonNetwork.IsConnected)
            {
                JoinPhotonRoom(GamemodeSelection.SelectedGameMode);
            }
            else
            {
                // keep track of the will to join a room
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        public void JoinPhotonRoom(Mode gamemode)
        {
            Hashtable expectedProperties = new Hashtable();
            expectedProperties.Add("gm", GamemodeSelection.SelectedGameMode);

            PhotonNetwork.JoinRandomRoom(expectedProperties, this.maxPlayersPerRoom);
        }

        public override void OnConnectedToMaster()
        {
            // #Critical: The first we try to do is join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            // We join the room only if we are in the process of a connection
            if (isConnecting)
            {
                JoinPhotonRoom(GamemodeSelection.SelectedGameMode);
                isConnecting = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (controlPanel != null)
            {
                progressLabel.SetActive(false);
                controlPanel.SetActive(true);
            }
            
            isConnecting = false;
            Debug.LogWarningFormat("Multiplayer/Launcher: OnDisconnected was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"Multiplayer/Launcher: OnJoinRandomFailed was called by PUN. No random room were found. returnCode: {returnCode}, message: {message}");

            // #Critical: We failed to join a random room, so we create a new one.
            Hashtable properties = new Hashtable();
            properties.Add("gm", GamemodeSelection.SelectedGameMode);

            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom, CustomRoomProperties = properties});
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Multiplayer/Launcher: OnJoinedRoom called by PUN.");

            if (PhotonNetwork.IsMasterClient && !PhotonNetwork.OfflineMode)
            {
                switch ((Mode) PhotonNetwork.CurrentRoom.CustomProperties["gm"])
                {
                    case Mode.FFADeathMatch:
                        PhotonNetwork.LoadLevel("FFADeathMatchDemo");
                        break;
                    case Mode.TeamDeathMatch:
                        PhotonNetwork.LoadLevel("TeamDeathMatchDemo");
                        break;
                }
            }
        }
    }
}

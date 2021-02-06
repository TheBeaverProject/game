using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
        [Tooltip("he maximum number of player that can be in a single room")]
        [SerializeField]
        private byte maxPlayersPerRoom = 10;

        /// <summary>
        /// MonoBehavior method called on GameObject by Unity during early initialisation phase
        /// </summary>
        private void Awake()
        {
            // #Critical
            // Makes sure the PhotonNetwork.LoadLevel() can be called and all the clients in the same room can sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        

        public void Connect()
        {
            // Checks if the client is connected
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Multiplayer/Launcher: OnConnectedToMaster was called by PUN");
            
            // #Critical: The first we try to do is join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("Multiplayer/Launcher: OnDisconnected was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"Multiplayer/Launcher: OnJoinRandomFailed was called by PUN. No random room were found. returnCode: {returnCode}, message: {message}");
            
            // #Critical: We failed to join a random room, so we create a new one.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Multiplayer/Launcher: OnJoinedRoom called by PUN.");
        }
    }
}

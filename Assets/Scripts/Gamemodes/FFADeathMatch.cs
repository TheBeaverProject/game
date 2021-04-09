using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Multiplayer;
using Photon.Pun;
using Photon.Realtime;
using PlayerManagement;
using UnityEngine;

namespace Scripts.Gamemodes
{
    public class FFADeathMatch : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [Header("Setup")]
        public int PointsPerKill = 10;

        public int PointsPerAssists = 4;

        public float GameDurationInMinutes = 10;
        
        public Dictionary<Player, DeathmatchPlayerData> DeathmatchData;
        
        #region MonoBehaviour Callbacks

        private void Start()
        {
            SetupDeathmatchData();
        }
        
        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        #endregion

        #region PUN Callbacks

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            
            if (eventCode == EventCodes.Kill)
            {
                EventCustomData.Kill eventData = (EventCustomData.Kill) photonEvent.CustomData;
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // Add the player to the Deathmatch Data
            AddPlayerToDataIfNotExists(newPlayer);
            
            if (PhotonNetwork.IsMasterClient)
            {
                // If we are the Master Client, synchronize data for everyone
                foreach (var deathmatchPlayerData in DeathmatchData)
                {
                    photonView.RPC("UpdateDeathmatchPlayerData", RpcTarget.Others, 
                        deathmatchPlayerData.Key.ActorNumber, deathmatchPlayerData.Value.kills, deathmatchPlayerData.Value.assists, deathmatchPlayerData.Value.deaths);
                }
            }

            base.OnPlayerEnteredRoom(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (!otherPlayer.IsInactive)
            {
                RemovePlayerFromData(otherPlayer);
            }
            
            base.OnPlayerLeftRoom(otherPlayer);
        }

        #endregion

        #region RPC Methods

        [PunRPC]
        void UpdateDeathmatchPlayerData(int playerActorNumber, int kills, int assists, int deaths)
        {
            Debug.Log($"{playerActorNumber}: {kills}, {assists}, {deaths}");
            UpdateDataByPlayer(playerActorNumber, kills, assists, deaths);
        }

        #endregion

        #region Private Methods

        void SetupDeathmatchData()
        {
            DeathmatchData = new Dictionary<Player, DeathmatchPlayerData>();
            
            foreach (var roomPlayerKVP in PhotonNetwork.CurrentRoom.Players)
            {
                var roomPlayer = roomPlayerKVP.Value;
                var playerData = new DeathmatchPlayerData();
                playerData.name = roomPlayer.NickName;
                
                DeathmatchData.Add(roomPlayer, playerData);
            }
        }

        void AddPlayerToDataIfNotExists(Player roomPlayer)
        {
            var playerData = new DeathmatchPlayerData();
            playerData.name = roomPlayer.NickName;

            foreach (var deathmatchPlayerData in DeathmatchData)
            {
                if (deathmatchPlayerData.Key.ActorNumber == roomPlayer.ActorNumber)
                {
                    return;
                }
            }
                
            DeathmatchData.Add(roomPlayer, playerData);
        }

        void RemovePlayerFromData(Player roomPlayer)
        {
            Player toRemove = null;

            foreach (var deathmatchPlayerData in DeathmatchData)
            {
                if (deathmatchPlayerData.Key.ActorNumber == roomPlayer.ActorNumber)
                {
                    toRemove = deathmatchPlayerData.Key;
                }
            }

            DeathmatchData.Remove(toRemove);
        }

        void UpdateDataByPlayer(int playerActorNumber, int kills = -1, int assists = -1, int deaths = -1)
        {
            KeyValuePair<Player, DeathmatchPlayerData> toUpdate;

            foreach (var deathmatchPlayerData in DeathmatchData)
            {
                var deathmatchPlayer = deathmatchPlayerData.Key;

                if (deathmatchPlayer.ActorNumber == playerActorNumber)
                {
                    toUpdate = deathmatchPlayerData;
                }
            }
            
            var playerData = new DeathmatchPlayerData();
            playerData.name = toUpdate.Key.NickName;
            
            playerData.kills = kills == -1 ? toUpdate.Value.kills : kills;
            playerData.deaths = deaths == -1 ? toUpdate.Value.deaths : deaths;
            playerData.assists = assists == -1 ? toUpdate.Value.assists : assists;

            DeathmatchData[toUpdate.Key] = playerData;
        }

        #endregion
    }
    
    public struct DeathmatchPlayerData
    {
        public string name;
        public int kills;
        public int assists;
        public int deaths;
    }
}
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Multiplayer;
using Photon.Pun;
using Photon.Realtime;
using PlayerManagement;
using UI.HUD;
using UnityEngine;

namespace Scripts.Gamemodes
{
    public class FFADeathMatch : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [Header("Setup")]
        public int PointsPerKill = 10;

        public int PointsPerAssists = 4;

        public int GameDurationInMinutes = 10;
        public double StartTime;
        public double ElapsedTime;
        
        public FFAManager FFAManager;
        
        public Dictionary<Player, DeathmatchPlayerData> DeathmatchData;

        private bool startTimer;
        
        #region MonoBehaviour Callbacks

        private void Start()
        {
            SetupDeathmatchData();

            if (PhotonNetwork.IsMasterClient)
            {
                StartTimer();
            }
            else
            {
                StartTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
                startTimer = true;
            }
        }

        private bool playerInitialized = false;
        private void Update()
        {
            if (!playerInitialized)
            {
                if (PlayerManager.LocalPlayerInstance)
                {
                    FFAManager.PlayerManager.HUD.Init(HUDType.Deathmatch);
                }
            }
            
            if (startTimer)
            {
                ElapsedTime = PhotonNetwork.Time - StartTime;

                if (PlayerManager.LocalPlayerInstance)
                {
                    FFAManager.PlayerManager.HUD.UpdateCountdown(GameDurationInMinutes * 60 - ElapsedTime);
                }

                if (ElapsedTime >= GameDurationInMinutes * 60)
                {
                    // TODO: Timer is finished
                }
            }
        }

        public override void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
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
                Dictionary<string, int> eventData = (Dictionary<string, int>) photonEvent.CustomData;
                
                IncrementDataByPlayer(eventData["killerActorNum"], kills: 1);
                IncrementDataByPlayer(eventData["deadActorNum"], deaths: 1);
                IncrementDataByPlayer(eventData["assistActorNum"], assists: 1);
                
                Debug.Log($"Kill Event: {eventData["killerActorNum"]} killed {eventData["deadActorNum"]} with assist by {eventData["assistActorNum"]}");
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // Add the player to the Deathmatch Data
            AddPlayerToDataIfNotExists(newPlayer);
            
            if (PhotonNetwork.IsMasterClient)
            {
                // If we are the Master Client, synchronize player data for everyone
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
        
        public void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
        {
            object propsTime;
            if (propertiesThatChanged.TryGetValue("StartTime", out propsTime))
            {
                StartTime = (double) propsTime;
            }
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

        #region Timer

        void StartTimer()
        {
            var CustomValue = new Hashtable();
            StartTime = PhotonNetwork.Time;
            startTimer = true;
            CustomValue.Add("StartTime", StartTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValue);
        }

        #endregion

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
        
        void IncrementDataByPlayer(int playerActorNumber, int kills = -1, int assists = -1, int deaths = -1)
        {
            if (playerActorNumber == -1)
            {
                return;
            }
            
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
            playerData.kills = kills == -1 ? toUpdate.Value.kills : toUpdate.Value.kills + kills;
            playerData.deaths = deaths == -1 ? toUpdate.Value.deaths : toUpdate.Value.deaths + deaths;
            playerData.assists = assists == -1 ? toUpdate.Value.assists : toUpdate.Value.assists + assists;

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
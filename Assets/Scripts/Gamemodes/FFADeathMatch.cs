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
    public class FFADeathMatch : Gamemode, IOnEventCallback
    {
        [Header("Setup")]
        public int PointsPerKill = 10;

        public int PointsPerAssists = 4;

        public int GameDurationInMinutes = 10;
        public double StartTime;
        public double ElapsedTime;
        
        public FFAManager FFAManager;

        public GameData DeathMatchData = new GameData();

        private bool startTimer;
        
        #region MonoBehaviour Callbacks

        private void Start()
        {
            DeathMatchData.SetupData(PhotonNetwork.CurrentRoom.Players);

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
        
        private void Update()
        {
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

        #region Gamemode callbacks

        private bool playerInitialized = false;
        public override void OnPlayerRespawn(PlayerManager playerManager)
        {
            if (!playerInitialized)
            {
                if (PlayerManager.LocalPlayerInstance)
                {
                    playerManager.HUD.Init(HUDType.Deathmatch);
                    playerInitialized = true;
                }
            }
        }

        public override void OnPlayerDeath()
        {
            // ya zebi il est mort
        }

        #endregion

        #region PUN Callbacks

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            
            if (eventCode == EventCodes.Kill)
            {
                Dictionary<string, int> eventData = (Dictionary<string, int>) photonEvent.CustomData;
                
                DeathMatchData.IncrementDataByPlayer(eventData["killerActorNum"], kills: 1);
                DeathMatchData.IncrementDataByPlayer(eventData["deadActorNum"], deaths: 1);
                DeathMatchData.IncrementDataByPlayer(eventData["assistActorNum"], assists: 1);
                
                Debug.Log($"Kill Event: {eventData["killerActorNum"]} killed {eventData["deadActorNum"]} with assist by {eventData["assistActorNum"]}");
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // Add the player to the Deathmatch Data
            DeathMatchData.AddPlayerToDataIfNotExists(newPlayer);
            
            if (PhotonNetwork.IsMasterClient)
            {
                // If we are the Master Client, synchronize player data for everyone
                foreach (var deathmatchPlayerData in DeathMatchData.Dictionary)
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
                DeathMatchData.RemovePlayerFromData(otherPlayer);
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
            DeathMatchData.UpdateDataByPlayer(playerActorNumber, kills, assists, deaths);
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

        #endregion
    }
}
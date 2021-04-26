using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Firebase;
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

        public GameData PlayersData = new GameData();

        private bool startTimer;
        
        #region MonoBehaviour Callbacks

        private void Start()
        {
            PlayersData.SetupData(PhotonNetwork.CurrentRoom.Players);

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
                    startTimer = false;
                    
                    InitEndgameScreen();
                }
            }

            if (Input.GetKey(KeyCode.Tab))
            {
                FFAManager?.PlayerManager?.HUD.ScoreBoard.SetAsFFA(PlayersData.GetSortedPlayerData());
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

        public override void OnPlayerRespawn(PlayerManager playerManager)
        {
            playerManager.HUD.Init(HUDType.Deathmatch);
                    
            playerManager.HUD.ScoreBoard.SetAsFFA(PlayersData.GetSortedPlayerData());
        }

        #endregion

        #region PUN Callbacks

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            
            if (eventCode == EventCodes.Kill)
            {
                Dictionary<string, int> eventData = (Dictionary<string, int>) photonEvent.CustomData;
                
                PlayersData.IncrementDataByPlayer(eventData["killerActorNum"], kills: 1, points: PointsPerKill);
                PlayersData.IncrementDataByPlayer(eventData["assistActorNum"], assists: 1, points: PointsPerAssists);
                PlayersData.IncrementDataByPlayer(eventData["deadActorNum"], deaths: 1);
                
                FFAManager.PlayerManager.HUD.ScoreBoard.SetAsFFA(PlayersData.GetSortedPlayerData());

                Debug.Log($"Kill Event: {eventData["killerActorNum"]} killed {eventData["deadActorNum"]} with assist by {eventData["assistActorNum"]}");
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // Add the player to the Deathmatch Data
            PlayersData.AddPlayerToDataIfNotExists(newPlayer);
            
            if (PhotonNetwork.IsMasterClient)
            {
                // If we are the Master Client, synchronize player data for everyone
                foreach (var deathmatchPlayerData in PlayersData.Dictionary)
                {
                    photonView.RPC("UpdateDeathmatchPlayerData", RpcTarget.Others, 
                        deathmatchPlayerData.Key.ActorNumber, deathmatchPlayerData.Value.kills, deathmatchPlayerData.Value.assists, deathmatchPlayerData.Value.deaths, deathmatchPlayerData.Value.points);
                }
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (!otherPlayer.IsInactive)
            {
                PlayersData.RemovePlayerFromData(otherPlayer);
            }
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
        void UpdateDeathmatchPlayerData(int playerActorNumber, int kills, int assists, int deaths, int points)
        {
            PlayersData.UpdateDataByPlayer(playerActorNumber, kills, assists, deaths, points);
            FFAManager?.PlayerManager?.HUD.ScoreBoard.SetAsFFA(PlayersData.GetSortedPlayerData());
        }

        [PunRPC]
        void RegisterMatch(string documentId)
        {
            StatisticsHandler.RegisterMatch(documentId, success =>
            {
                if (success)
                    Debug.Log("Successfully registered in the finished match");
            });
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
        
        void InitEndgameScreen()
        {
            Player winner = GetWinner();
            
            // Disable movement
            FFAManager.PlayerManager.DisableMovement();
            // Instantiate endgame screen
            var go = Instantiate(EndgameScreenPrefab, Vector3.zero, Quaternion.identity);

            var controller = go.GetComponent<EndGameScreenController>();

            EndGameScreenController.Result result;

            result = winner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber ?
                EndGameScreenController.Result.Win : EndGameScreenController.Result.Loss;

            controller.SetResult(result);

            go.GetComponentInChildren<ScoreboardController>().SetAsFFA(PlayersData.GetSortedPlayerData());

            if (PhotonNetwork.IsMasterClient)
            {
                StatisticsHandler.PostNewMatch(Mode.FFADeathMatch.ToString(), winner.NickName, PlayersData, (success, document) =>
                {
                    var documentId = document.GetId();
                
                    photonView.RPC("RegisterMatch", RpcTarget.All, documentId);
                });
            }
        }

        Player GetWinner()
        {
            Player winner = null;
            int points = 0;

            foreach (var kvp in PlayersData.Dictionary)
            {
                if (kvp.Value.points >= points)
                {
                    winner = kvp.Key;
                }
            }

            return winner;
        }

        #endregion
    }
}
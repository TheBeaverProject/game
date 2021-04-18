using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Multiplayer;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PlayerManagement;
using UI.HUD;
using UnityEngine;

namespace Scripts.Gamemodes
{
    public class TeamDeathMatch : Gamemode, IOnEventCallback
    {
        [Header("Setup")]
        public int PointsPerKill = 10;
        
        public int PointsPerAssist = 4;

        public int PointsPerTeamKill = -5;

        public int GameDurationInMinutes = 10;
        
        public double StartTime;
        public double ElapsedTime;
        private bool startTimer;

        [Header("Values")]
        public int Team1TotalPoints = 0;
        
        public int Team2TotalPoints = 0;

        public GameData PlayersData = new GameData();

        public TeamManager TeamManager;
        
        #region MonoBehaviours callbacks

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

                if (PlayerManager.LocalPlayerInstance != null)
                {
                    TeamManager.PlayerManager.HUD.UpdateCountdown(GameDurationInMinutes * 60 - ElapsedTime);
                }

                if (ElapsedTime >= GameDurationInMinutes * 60)
                {
                    startTimer = false;
                    
                    byte winner = GetWinner();

                    if (winner == 0)
                    {
                        // Add 2 minutes to game time if it is a draw
                        GameDurationInMinutes += 2;
                        startTimer = true;
                    }
                    else // Game is finished
                    {
                        InitEndgameScreen(winner);
                        // TODO: Register game in firebase if masterclient
                    }
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

        #region Gamemode Callbacks
        
        public override void OnPlayerRespawn(PlayerManager playerManager)
        {
            playerManager.HUD.Init(HUDType.TeamDeathmatch);
            playerManager.HUD.SetTeamPoints(Team1TotalPoints, Team2TotalPoints);

            playerManager.HUD.ScoreBoard.Set(
                PlayersData.GetSortedPlayerDataByTeam(TeamManager.Team1.Code), 
                PlayersData.GetSortedPlayerDataByTeam(TeamManager.Team2.Code));
        }

        public override void OnPlayerJoinedTeam()
        {
            Debug.Log("OnPlayerJoinedTeam");
            
            TeamManager.PlayerManager.HUD.ScoreBoard.Set(
                PlayersData.GetSortedPlayerDataByTeam(TeamManager.Team1.Code), 
                PlayersData.GetSortedPlayerDataByTeam(TeamManager.Team2.Code));
        }

        #endregion

        #region Pun Callbacks

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            
            if (eventCode == EventCodes.Kill)
            {
                Dictionary<string, int> eventData = (Dictionary<string, int>) photonEvent.CustomData;

                var killer = PhotonNetwork.CurrentRoom.GetPlayer(eventData["killerActorNum"]);
                var assist = PhotonNetwork.CurrentRoom.GetPlayer(eventData["assistActorNum"]);
                var dead   = PhotonNetwork.CurrentRoom.GetPlayer(eventData["deadActorNum"]);

                var killerTeam = killer?.GetPhotonTeam();
                var assistTeam = assist?.GetPhotonTeam();
                var deadTeam = dead?.GetPhotonTeam();

                if (killerTeam == deadTeam) // Team kill
                {
                    if (killerTeam.Code == 1)
                    {
                        Team1TotalPoints += PointsPerTeamKill;
                    }
                    else
                    {
                        Team2TotalPoints += PointsPerTeamKill;
                    }

                    PlayersData.IncrementDataByPlayer(eventData["killerActorNum"], kills: 1, points: PointsPerTeamKill);
                }
                else
                {
                    if (killerTeam.Code == 1)
                    {
                        Team1TotalPoints += PointsPerKill;
                    }
                    else
                    {
                        Team2TotalPoints += PointsPerKill;
                    }

                    PlayersData.IncrementDataByPlayer(eventData["killerActorNum"], kills: 1, points: PointsPerKill);
                    PlayersData.IncrementDataByPlayer(eventData["assistActorNum"], assists: 1, points: PointsPerAssist);
                }
                
                PlayersData.IncrementDataByPlayer(eventData["deadActorNum"], deaths: 1);

                TeamManager.PlayerManager.HUD.SetTeamPoints(Team1TotalPoints, Team2TotalPoints);                
                TeamManager.PlayerManager.HUD.ScoreBoard.Set(
                    PlayersData.GetSortedPlayerDataByTeam(1), 
                    PlayersData.GetSortedPlayerDataByTeam(2));

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
                foreach (var playerData in PlayersData.Dictionary)
                {
                    photonView.RPC("UpdatePlayerData", RpcTarget.Others, 
                        playerData.Key.ActorNumber, playerData.Value.kills, playerData.Value.assists, playerData.Value.deaths);
                }
                
                // If we are the master client, update the points for everyone
                photonView.RPC("UpdateTeamPoints", RpcTarget.Others, Team1TotalPoints, Team2TotalPoints);
            }

            base.OnPlayerEnteredRoom(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (!otherPlayer.IsInactive)
            {
                PlayersData.RemovePlayerFromData(otherPlayer);
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
        void UpdatePlayerData(int playerActorNumber, int kills, int assists, int deaths)
        {
            Debug.Log($"{playerActorNumber}: {kills}, {assists}, {deaths}");
            PlayersData.UpdateDataByPlayer(playerActorNumber, kills, assists, deaths);
        }

        [PunRPC]
        void UpdateTeamPoints(int team1Points, int team2Points)
        {
            Team1TotalPoints = team1Points;
            Team2TotalPoints = team2Points;
        }

        #endregion

        #region Private Methods

        void InitEndgameScreen(byte winner)
        {
            // Disable movement
            TeamManager.PlayerManager.DisableMovement();
            // Instantiate endgame screen
            var go = Instantiate(EndgameScreenPrefab, Vector3.zero, Quaternion.identity);

            var controller = go.GetComponent<EndGameScreenController>();

            EndGameScreenController.Result result;

            if (winner == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
            {
                result = EndGameScreenController.Result.Win;
            }
            else
            {
                result = EndGameScreenController.Result.Loss;
            }

            controller.SetResult(result);
            
            go.GetComponentInChildren<ScoreboardController>().Set(
                PlayersData.GetSortedPlayerDataByTeam(TeamManager.Team1.Code), 
                PlayersData.GetSortedPlayerDataByTeam(TeamManager.Team2.Code));
        }

        void StartTimer()
        {
            var CustomValue = new Hashtable();
            StartTime = PhotonNetwork.Time;
            startTimer = true;
            CustomValue.Add("StartTime", StartTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValue);
        }

        byte GetWinner()
        {
            byte winner;
            
            if (Team1TotalPoints > Team2TotalPoints)
            {
                winner = 1;
            } else if (Team2TotalPoints > Team1TotalPoints)
            {
                winner = 2;
            }
            else
            {
                winner = 0;
            }

            return winner;
        }

        #endregion
    }
}
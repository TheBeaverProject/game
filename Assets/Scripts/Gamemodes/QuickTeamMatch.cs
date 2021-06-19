using System;
using System.Collections.Generic;
using System.Linq;
using DiscordRPC;
using ExitGames.Client.Photon;
using Firebase;
using Multiplayer;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PlayerManagement;
using Scripts.Gamemodes.Mechanics;
using UI.HUD;
using UnityEngine;

namespace Scripts.Gamemodes
{
    public class QuickTeamMatch : DominationGamemode, IOnEventCallback
    {
        [Header("Setup")]
        public int PointsPerKill = 10;
        
        public int PointsPerAssist = 4;

        public int PointsPerTeamKill = -5;

        public int PointsPerTickZoneDomination = 1;

        public int GameDurationInMinutes = 10;
        
        [Header("Timer")]
        public double StartTime;
        public double ElapsedTime;
        private bool startTimer;
        public long endUnixTimestamp;

        [Header("Game Values")]
        
        public int Team1TotalPoints = 0;
        public int Team2TotalPoints = 0;
        
        public GameData PlayersData = new GameData();
        public TeamManager TeamManager;
        
        
        #region MonoBehaviours callbacks

        private void Start()
        {
            PlayersData.SetupData(PhotonNetwork.CurrentRoom.Players);
            
            UpdateTeammatesOnHud();
            
            if (PhotonNetwork.IsMasterClient)
            {
                StartTimer();
            }
            else
            {
                StartTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
                endUnixTimestamp = long.Parse(PhotonNetwork.CurrentRoom.CustomProperties["EndTimeUnix"].ToString());
                startTimer = true;
            }
        }
        
        private void Update()
        {
            if (startTimer)
            {
                if (PlayerManager.LocalPlayerInstance != null)
                {
                    ElapsedTime = PhotonNetwork.Time - StartTime;
                    TeamManager.playerManager.HUD.UpdateCountdown(GameDurationInMinutes * 60 - ElapsedTime);
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
                        GameEnd(winner);
                    }
                }
            }
            
            if (Input.GetKey(KeyCode.Tab))
            {
                TeamManager?.playerManager?.HUD.ScoreBoard.Set(
                    PlayersData.GetSortedPlayerDataByTeam(1), 
                    PlayersData.GetSortedPlayerDataByTeam(2));
            }
        }

        private void FixedUpdate()
        {
            HandleZones();
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
            TeamManager.playerManager = playerManager;
            
            playerManager.HUD.Init(HUDType.Teams);
            playerManager.HUD.UpdateTeamPoints(Team1TotalPoints, Team2TotalPoints);

            playerManager.HUD.ScoreBoard.Set(
                PlayersData.GetSortedPlayerDataByTeam(TeamManager.Team1.Code), 
                PlayersData.GetSortedPlayerDataByTeam(TeamManager.Team2.Code));
            UpdateDiscordActivity();
        }

        public override void OnPlayerJoinedTeam()
        {
            Debug.Log("OnPlayerJoinedTeam");
            
            TeamManager.playerManager.HUD.ScoreBoard.Set(
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

                // Update HUD and Killfeed
                TeamManager.playerManager.HUD.UpdateTeamPoints(Team1TotalPoints, Team2TotalPoints);                
                TeamManager.playerManager.HUD.ScoreBoard.Set(
                    PlayersData.GetSortedPlayerDataByTeam(1), 
                    PlayersData.GetSortedPlayerDataByTeam(2));
                TeamManager.playerManager.HUD.AddKillFeedElement(killer, assist, dead);
                UpdateTeammatesOnHud();
                UpdateDiscordActivity();

                Debug.Log($"Kill Event: {eventData["killerActorNum"]} killed {eventData["deadActorNum"]} with assist by {eventData["assistActorNum"]}");
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // Add the player to the Deathmatch Data
            PlayersData.AddPlayerToDataIfNotExists(newPlayer);
            
            // Update the HUD
            UpdateTeammatesOnHud();

            if (PhotonNetwork.IsMasterClient)
            {
                // If we are the Master Client, synchronize player data for everyone
                foreach (var playerData in PlayersData.Dictionary)
                {
                    photonView.RPC("UpdatePlayerData", RpcTarget.Others, 
                        playerData.Key.ActorNumber, playerData.Value.kills, playerData.Value.assists, playerData.Value.deaths, playerData.Value.points);
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
            
            UpdateTeammatesOnHud();
            
            base.OnPlayerLeftRoom(otherPlayer);
        }
        
        public void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
        {
            object propsTime;
            if (propertiesThatChanged.TryGetValue("StartTime", out propsTime))
            {
                StartTime = (double) propsTime;
            }
            
            if (propertiesThatChanged.TryGetValue("EndTimeUnix", out propsTime))
            {
                endUnixTimestamp = (long) propsTime;
            }
        }

        #endregion
        
        #region RPC Methods

        [PunRPC]
        void UpdatePlayerData(int playerActorNumber, int kills, int assists, int deaths, int points)
        {
            PlayersData.UpdateDataByPlayer(playerActorNumber, kills, assists, deaths, points);
            UpdateTeammatesOnHud();
        }

        [PunRPC]
        void UpdateTeamPoints(int team1Points, int team2Points)
        {
            Team1TotalPoints = team1Points;
            Team2TotalPoints = team2Points;
            TeamManager.playerManager.HUD.UpdateTeamPoints(team1Points, team2Points);
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

        void GameEnd(byte winner)
        {
            // Disable movement
            TeamManager.playerManager.DisableMovement();
            // Instantiate endgame screen
            var go = Instantiate(EndgameScreenPrefab, Vector3.zero, Quaternion.identity);
            var controller = go.GetComponent<EndGameScreenController>();

            // Set result on endgame screen
            EndGameScreenController.Result result;
            if (winner == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
                result = EndGameScreenController.Result.Win;
            else
                result = EndGameScreenController.Result.Loss;
            controller.SetResult(result);
            
            // Update endgame scoreboard
            go.GetComponentInChildren<ScoreboardController>().Set(
                PlayersData.GetSortedPlayerDataByTeam(TeamManager.Team1.Code), 
                PlayersData.GetSortedPlayerDataByTeam(TeamManager.Team2.Code));

            // Update ELO
            UpdateElo(result, controller);
            
            // Publish statistics
            if (PhotonNetwork.IsMasterClient)
            {
                StatisticsHandler.PostNewMatch(Type.QuickTeamMatch.ToString(), winner.ToString(), PlayersData, (success, document) =>
                {
                    var documentId = document.GetId();
                
                    photonView.RPC("RegisterMatch", RpcTarget.All, documentId);
                });
            }
        }
        
        void UpdateDiscordActivity()
        {
            var discordController = this.gameObject.GetComponent<DiscordController>();
            var localPlayerData = PlayersData.GetSinglePlayerData(PhotonNetwork.LocalPlayer.ActorNumber);
            discordController.UpdateActivity($"Team Deathmatch | {Team1TotalPoints} - {Team2TotalPoints}", $"KDA: {localPlayerData.kills}/{localPlayerData.deaths}/{localPlayerData.assists} - Score: {localPlayerData.points}", endTimestamp: endUnixTimestamp);
        }

        void UpdateTeammatesOnHud()
        {
            PhotonTeam team;
            if ((team = PhotonNetwork.LocalPlayer.GetPhotonTeam()) != null)
            {
                var teammates = PlayersData?.GetPlayerDataByTeam(team.Code);
                teammates.Remove(teammates.Find(data => data.name == PhotonNetwork.NickName));
            
                TeamManager.playerManager.HUD.UpdateTeammatesInfo(teammates);
            }
        }

        void StartTimer()
        {
            var CustomValue = new Hashtable();
            StartTime = PhotonNetwork.Time;
            endUnixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() + GameDurationInMinutes * 60;
            startTimer = true;
            CustomValue.Add("StartTime", StartTime);
            CustomValue.Add("EndTimeUnix", endUnixTimestamp);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValue);
        }

        /// <summary>
        /// 1 = team1 | 2 = team2 | 0 = draw
        /// </summary>
        /// <returns>byte representing the winner of the match</returns>
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
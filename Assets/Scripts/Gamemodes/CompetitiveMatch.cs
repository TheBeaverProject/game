﻿using System;
using System.Collections.Generic;
using DiscordRPC;
using ExitGames.Client.Photon;
using Firebase;
using Multiplayer;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PlayerManagement;
using UI.HUD;
using UnityEngine;

namespace Scripts.Gamemodes
{
    public class CompetitiveMatch : Gamemode, IOnEventCallback
    {
        [Header("Setup")]
        public double RoundDurationInMinutes = 2.5;
        
        public double RoundsToWin = 6;
        
        public int PointsPerKill = 10;
        
        public int PointsPerAssist = 4;

        public int PointsPerTeamKill = -5;
        
        [Header("Timer")]
        public double StartTime;
        public double ElapsedTime;
        private bool startTimer;
        public long endUnixTimestamp;

        [Header("Zone Domination Mechanics")]
        
        public Mechanics.DominationPoint ZoneA;
        public Mechanics.DominationPoint ZoneB;

        public int[] ZoneAPoints = {0, 0};
        public int[] ZoneBPoints = {0, 0};

        public byte ZoneACapturedBy = 0;
        public byte ZoneBCapturedBy = 0;

        public int PointsToCaptureZone = 700;
        
        [Header("Game Values")]
        
        public int Team1Rounds = 0;
        public int Team2Rounds = 0;
        
        public GameData PlayersData = new GameData();
        public RoundsManager RoundsManager;
        
        
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
                    RoundsManager.playerManager.HUD.UpdateCountdown(RoundDurationInMinutes * 60 - ElapsedTime);
                }

                if (Team1Rounds == 6 || Team2Rounds == 6)
                {
                    byte winner = GetOverallWinner();

                    if (winner != 0) // Game is finished
                    {
                        startTimer = false;
                        GameEnd(winner);
                    }
                }
                
                if (ElapsedTime >= RoundDurationInMinutes * 60 || AreZoneCaptured) // End of the round
                {
                    startTimer = false;
                    byte winner = AreZoneCaptured ? ZoneACapturedBy : GetRoundWinner();
                    
                    RoundEnd(winner);
                }
            }
            
            if (Input.GetKey(KeyCode.Tab))
            {
                RoundsManager?.playerManager?.HUD.ScoreBoard.Set(
                    PlayersData.GetSortedPlayerDataByTeam(1), 
                    PlayersData.GetSortedPlayerDataByTeam(2));
            }
        }

        private void FixedUpdate()
        {
            ComputeZonePoints();
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
            RoundsManager.playerManager = playerManager;
            
            playerManager.HUD.Init(HUDType.Rounds);
            playerManager.HUD.UpdateRounds(Team1Rounds, Team2Rounds);

            playerManager.HUD.ScoreBoard.Set(
                PlayersData.GetSortedPlayerDataByTeam(RoundsManager.Team1.Code), 
                PlayersData.GetSortedPlayerDataByTeam(RoundsManager.Team2.Code));
            
            UpdateDiscordActivity();
            UpdateTeammatesOnHud();
        }

        public override void OnPlayerJoinedTeam()
        {
            Debug.Log("OnPlayerJoinedTeam");
            
            RoundsManager.playerManager.HUD.ScoreBoard.Set(
                PlayersData.GetSortedPlayerDataByTeam(RoundsManager.Team1.Code), 
                PlayersData.GetSortedPlayerDataByTeam(RoundsManager.Team2.Code));
            
            UpdateTeammatesOnHud();
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
                var deadTeam = dead?.GetPhotonTeam();

                UpdatePoints(killerTeam, deadTeam, eventData);

                // Update HUD and Killfeed             
                RoundsManager?.playerManager?.HUD.ScoreBoard.Set(
                    PlayersData.GetSortedPlayerDataByTeam(1), 
                    PlayersData.GetSortedPlayerDataByTeam(2));
                RoundsManager?.playerManager?.HUD.AddKillFeedElement(killer, assist, dead);
                UpdateTeammatesOnHud();

                Debug.Log($"Kill Event: {eventData["killerActorNum"]} killed {eventData["deadActorNum"]}" + 
                          $"with assist by {eventData["assistActorNum"]}");
                
                
                if (PlayersData.IsEveryPlayerDead(deadTeam.Code)) // End of the round
                {
                    byte winnerTeamCode = (byte) (deadTeam.Code == 1 ? 2 : 1);

                    RoundEnd(winnerTeamCode);
                }
            }
            
            UpdateDiscordActivity();
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
                        playerData.Key.ActorNumber, playerData.Value.kills, playerData.Value.assists, playerData.Value.deaths, playerData.Value.points);
                }
                
                // If we are the master client, update the points for everyone
                photonView.RPC("UpdateRounds", RpcTarget.Others, Team1Rounds, Team2Rounds);
            }

            UpdateTeammatesOnHud();
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
        }

        [PunRPC]
        void UpdateRounds(int team1Rounds, int team2Rounds)
        {
            Team1Rounds = team1Rounds;
            Team2Rounds = team2Rounds;
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

        [PunRPC]
        void RoundStart()
        {
            Debug.LogWarning("--------- New round Starting ----------");
            
            ResetZones();
            
            foreach (var p in PhotonNetwork.PlayerList)
            {
                PlayersData.SetPlayerState(p.ActorNumber, true);
            }
            
            RoundsManager.ResetLocalPlayer();
            UpdateTeammatesOnHud();
        }
        
        [PunRPC]
        void RestartTimer(double startTime, long endTimestamp)
        {
            StartTime = startTime;
            endUnixTimestamp = endTimestamp;
            startTimer = true;
        }

        #endregion
        
        #region Private Methods

        #region Event Handling

        private void UpdatePoints(PhotonTeam killerTeam, PhotonTeam deadTeam, Dictionary<string, int> eventData)
        {
            if (killerTeam == deadTeam) // Team kill
            {
                PlayersData.IncrementDataByPlayer(eventData["killerActorNum"], kills: 1, points: PointsPerTeamKill);
            }
            else
            {
                PlayersData.IncrementDataByPlayer(eventData["killerActorNum"], kills: 1, points: PointsPerKill);
                PlayersData.IncrementDataByPlayer(eventData["assistActorNum"], assists: 1, points: PointsPerAssist);
            }

            PlayersData.IncrementDataByPlayer(eventData["deadActorNum"], deaths: 1);
            PlayersData.SetPlayerState(eventData["deadActorNum"], false);
        }

        #endregion

        #region Management

        private void RoundEnd(byte winnerTeamCode)
        {
            if (winnerTeamCode == 1)
            {
                Team1Rounds++;
            }
            else if (winnerTeamCode == 2)
            {
                Team2Rounds++;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                Invoke("RoundStartMaster", 2);
            }
        }

        private void RoundStartMaster()
        {
            photonView.RPC("RestartTimer", RpcTarget.AllBufferedViaServer, PhotonNetwork.Time, DateTimeOffset.Now.ToUnixTimeSeconds() + (long) (RoundDurationInMinutes * 60));
            photonView.RPC("RoundStart", RpcTarget.AllViaServer);
        }

        void GameEnd(byte winner)
        {
            // Disable movement
            RoundsManager.playerManager.DisableMovement();
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
                PlayersData.GetSortedPlayerDataByTeam(RoundsManager.Team1.Code), 
                PlayersData.GetSortedPlayerDataByTeam(RoundsManager.Team2.Code));

            // Update ELO
            UpdateElo(result, controller);
            
            // Publish statistics
            if (PhotonNetwork.IsMasterClient)
            {
                StatisticsHandler.PostNewMatch(Type.CompetitiveMatch.ToString(), winner.ToString(), PlayersData, (success, document) =>
                {
                    var documentId = document.GetId();
                
                    photonView.RPC("RegisterMatch", RpcTarget.Others, documentId);
                });
            }
        }
        
        /// <summary>
        /// 1 = team1 | 2 = team2 | 0 = draw
        /// </summary>
        /// <returns>byte representing the winner of the round</returns>
        byte GetRoundWinner()
        {
            byte winner;
            var aliveTeam1 = PlayersData.GetPlayerDataAlive(1);
            var aliveTeam2 = PlayersData.GetPlayerDataAlive(2);
            
            if (aliveTeam1.Count > aliveTeam2.Count)
            {
                winner = 1;
            } else if (aliveTeam1.Count < aliveTeam2.Count)
            {
                winner = 2;
            }
            else
            {
                winner = 0;
            }

            return winner;
        }

        /// <summary>
        /// 1 = team1 | 2 = team2 | 0 = draw
        /// </summary>
        /// <returns>byte representing the winner of the match</returns>
        byte GetOverallWinner()
        {
            byte winner;
            
            if (Team1Rounds == 6)
            {
                winner = 1;
            } else if (Team2Rounds == 6)
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

        #region Zone Mechanics

        private void ComputeZonePoints()
        {
            if (ZoneACapturedBy == 0)
            {
                ZoneAPoints[0] = ZoneAPoints[0] > 0 ? ZoneAPoints[0] - 1 : 0;
                ZoneAPoints[1] = ZoneAPoints[1] > 0 ? ZoneAPoints[1] - 1 : 0;

                if (ZoneA.GetDominatingPhotonTeam != null)
                {
                    int teamI = ZoneA.GetDominatingPhotonTeam.Code - 1;
                    ZoneAPoints[teamI] += 2;
                    ZoneACapturedBy = ZoneAPoints[teamI] >= PointsToCaptureZone ? ZoneA.GetDominatingPhotonTeam.Code : (byte) 0;
                }
            }
            else
            {
                ZoneA.defaultColor = ZoneACapturedBy == 1 ? ZoneA.teamColor1 : ZoneA.teamColor2;
            }

            if (ZoneBCapturedBy == 0)
            {
                ZoneBPoints[0] = ZoneBPoints[0] > 0 ? ZoneBPoints[0] - 1 : 0;
                ZoneBPoints[1] = ZoneBPoints[1] > 0 ? ZoneBPoints[1] - 1 : 0;

                if (ZoneB.GetDominatingPhotonTeam != null)
                {
                    int teamI = ZoneB.GetDominatingPhotonTeam.Code - 1;
                    ZoneBPoints[teamI] += 2;
                    ZoneBCapturedBy = ZoneBPoints[teamI] >= PointsToCaptureZone ? ZoneB.GetDominatingPhotonTeam.Code : (byte) 0;
                }
            }
            else
            {
                ZoneB.defaultColor = ZoneBCapturedBy == 1 ? ZoneB.teamColor1 : ZoneB.teamColor2;
            }
        }

        private bool AreZoneCaptured => ZoneACapturedBy != 0 && ZoneACapturedBy == ZoneBCapturedBy;

        private void ResetZones()
        {
            ZoneA.defaultColor = new Color(0, 0, 0, 0);
            ZoneB.defaultColor = new Color(0, 0, 0, 0);
            ZoneAPoints = new []{0, 0};
            ZoneBPoints = new []{0, 0};
            ZoneACapturedBy = 0;
            ZoneBCapturedBy = 0;
        }

        #endregion
        
        void UpdateDiscordActivity()
        {
            var discordController = this.gameObject.GetComponent<DiscordController>();
            var localPlayerData = PlayersData.GetSinglePlayerData(PhotonNetwork.LocalPlayer.ActorNumber);
            discordController.UpdateActivity($"Rounds Deathmatch | {Team1Rounds} - {Team2Rounds}", $"KDA: {localPlayerData.kills}/{localPlayerData.deaths}/{localPlayerData.assists} - Score: {localPlayerData.points}", endTimestamp: endUnixTimestamp);
        }
        
        void UpdateTeammatesOnHud()
        {
            var teammates = PlayersData.GetPlayerDataByTeam(PhotonNetwork.LocalPlayer.GetPhotonTeam().Code);
            teammates.Remove(teammates.Find(data => data.name == PhotonNetwork.NickName));
            
            RoundsManager.playerManager.HUD.UpdateTeammatesInfo(teammates);
        }

        void StartTimer()
        {
            var CustomValue = new Hashtable();
            StartTime = PhotonNetwork.Time;
            endUnixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() + (long) (RoundDurationInMinutes * 60);
            startTimer = true;
            CustomValue.Add("StartTime", StartTime);
            CustomValue.Add("EndTimeUnix", endUnixTimestamp);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValue);
        }

        #endregion
    }
}
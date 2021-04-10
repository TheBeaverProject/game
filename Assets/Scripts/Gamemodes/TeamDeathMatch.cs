using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Multiplayer;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Scripts.Gamemodes
{
    public class TeamDeathMatch : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [Header("Setup")]
        public int PointsPerKill = 10;

        public int PointsPerAssists = 4;
        
        public int PointsPerTeamKill = -5;

        public int GameDurationInMinutes = 10;

        [Header("Values")]
        public int Team1TotalPoints = 0;
        
        public int Team2TotalPoints = 0;
        
        
        
        public TeamManager TeamManager;
        
        #region MonoBehaviours callbacks

        private void Start()
        {
               
        }

        private void Update()
        {
            
        }

        #endregion

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            
            if (eventCode == EventCodes.Kill)
            {
                Dictionary<string, int> eventData = (Dictionary<string, int>) photonEvent.CustomData;

                Debug.Log($"Kill Event: {eventData["killerActorNum"]} killed {eventData["deadActorNum"]} with assist by {eventData["assistActorNum"]}");
            }
        }

    }
}
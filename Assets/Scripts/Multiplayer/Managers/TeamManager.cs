using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PlayerManagement;
using Scripts.Gamemodes;
using UnityEngine;

namespace Multiplayer
{
    public class TeamManager : GameManager
    {
        [Header("Team names")]
        public string Team1Name;
        public string Team2Name;

        [Header("Teams")]
        public PhotonTeamsManager PhotonTeamsManager;
        public PhotonTeam Team1 = new PhotonTeam();
        public PhotonTeam Team2 = new PhotonTeam();

        [Header("Spawn Points")]
        public List<Transform> Team1SpawnPoints;
        public List<Transform> Team2SpawnPoints;

        [Header("Setup")]
        public bool AllowTeamSelection = false;

        #region MonoBeavhiour callbacks

        private byte localPlayerTeamCode;
        void Start()
        {
            PhotonTeamsManager = PhotonTeamsManager.Instance;
            PhotonTeamsManager.TryGetTeamByCode(1, out Team1);
            PhotonTeamsManager.TryGetTeamByCode(2, out Team2);

            Team1.Name = Team1Name;
            Team2.Name = Team2Name;
        }

        public bool localPlayerJoinedTeam;
        public PlayerManager PlayerManager;
        void Update()
        {
            if (PlayerManager.LocalPlayerInstance == null && !localPlayerJoinedTeam )
            {
                if (!AllowTeamSelection)
                {
                    localPlayerTeamCode = SelectNextTeam();

                    playerStartPos = SelectSpawnPoint(localPlayerTeamCode);

                    PlayerManager = RespawnPlayer();
                }
                else
                {
                    // TODO: Support team selection modal when it is allowed
                }
            } else if (!localPlayerJoinedTeam)
            {
                localPlayerJoinedTeam = JoinTeam(PlayerManager.LocalPlayerInstance.GetPhotonView());
            }
            
            if (PlayerManager != null)
            {
                if (PlayerManager.Health <= 0)
                {
                    PlayerManager = RespawnPlayer();
                }
            }
        }

        #endregion

        /// <summary>
        /// Function called by the client who want to join a team.
        /// </summary>
        /// <param name="PhotonView">PhotonView belonging to the player in the game</param>
        /// <param name="team">Team the player wants to join</param>
        /// <returns>Returns true if the player has sucessfully joined the team</returns>
        public bool JoinTeam(PhotonView View)
        {
            var currentTeam = View.Controller.GetPhotonTeam();
            
            if (currentTeam != null)
            {
                if (currentTeam.Code == localPlayerTeamCode)
                {
                    return true;
                }

                return View.Controller.SwitchTeam(localPlayerTeamCode);
            }

            bool success = View.Controller.JoinTeam(localPlayerTeamCode);

            if (success)
                gamemodeController.OnPlayerJoinedTeam();

            return success;
        }

        /// <summary>
        /// Selects the next team the player will join according to the player count
        /// </summary>
        /// <returns>int representing the next team</returns>
        byte SelectNextTeam()
        {
            Photon.Realtime.Player[] Team1Players;
            Photon.Realtime.Player[] Team2Players;
            PhotonTeamsManager.TryGetTeamMembers(Team1, out Team1Players);
            PhotonTeamsManager.TryGetTeamMembers(Team2, out Team2Players);

            if (Team1Players.Length > Team2Players.Length)
            {
                return 2;
            }
            else if (Team1Players.Length < Team2Players.Length)
            {
                return 1;
            } else
            {
                return (byte) Random.Range(1, 2);
            }
        }
        
        PlayerManager RespawnPlayer()
        {
            if (PlayerManager.LocalPlayerInstance != null)
            {
                PhotonNetwork.Destroy(PlayerManager.LocalPlayerInstance);
                PlayerManager.LocalPlayerInstance = null;
                PlayerManager = null;
            }

            // TODO: Buy Menu before respawning

            return InstantiateLocalPlayer();
        }

        Vector3 SelectSpawnPoint(int team)
        {
            switch (team)
            {
                case 1:
                    return Team1SpawnPoints[Random.Range(0, Team1SpawnPoints.Count)].position;
                case 2:
                    return Team2SpawnPoints[Random.Range(0, Team2SpawnPoints.Count)].position;
            }

            return new Vector3(0, 10, 0);
        }
    }
}
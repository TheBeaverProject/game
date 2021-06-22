using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PlayerManagement;
using UnityEngine;

namespace Multiplayer
{
    /// <summary>
    /// TeamManager with added support for spec mode and respawning at end of rounds
    /// </summary>
    public class RoundsManager : TeamManager
    {
        #region MonoBeavhiour callbacks

        public bool LocalPlayerReadyToSpawn = false;
        public bool LocalPlayerInSpecMode = false;

        void Update()
        {
            if (PlayerManager.LocalPlayerInstance == null && !localPlayerJoinedTeam )
            {
                if (!AllowTeamSelection)
                {
                    localPlayerTeamCode = SelectNextTeam();

                    playerStartPos = SelectSpawnPoint(localPlayerTeamCode);

                    playerManager = RespawnPlayer();
                }
                else
                {
                    // TODO: Support team selection modal when it is allowed
                }
            } else if (!localPlayerJoinedTeam)
            {
                localPlayerJoinedTeam = JoinTeam(PlayerManager.LocalPlayerInstance.GetPhotonView());
            }
            
            if (playerManager != null)
            {
                if (playerManager.Health <= 0 && !LocalPlayerInSpecMode) // Player is dead
                {
                    playerManager.EnterSpecMode();
                    LocalPlayerInSpecMode = true;
                }
            }
        }

        public void ResetLocalPlayer()
        {
            playerManager.ExitSpecMode();
            LocalPlayerInSpecMode = false;
            playerManager = RespawnPlayer();
        }
        
        #endregion
    }
}
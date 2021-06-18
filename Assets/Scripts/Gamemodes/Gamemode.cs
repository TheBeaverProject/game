using System.Linq;
using Firebase;
using Photon.Pun;
using PlayerManagement;
using UnityEngine;

namespace Scripts.Gamemodes
{
    public abstract class Gamemode : MonoBehaviourPunCallbacks
    {
        public GameObject EndgameScreenPrefab;
        
        /// <summary>
        /// Called after the player has been respawned by the GameManager
        /// </summary>
        /// <param name="playerManager">new playerManager linked to the player</param>
        public virtual void OnPlayerRespawn(PlayerManager playerManager) {}

        /// <summary>
        /// Called when the player is declared dead by the GameManager
        /// </summary>
        public virtual void OnPlayerDeath() {}

        /// <summary>
        /// Called when the player joins a new team
        /// </summary>
        public virtual void OnPlayerJoinedTeam() {}

        public void UpdateElo(EndGameScreenController.Result result, EndGameScreenController controller)
        {
            int eloPrevUpdate = (int) PhotonNetwork.LocalPlayer.CustomProperties["elo"];
            int gainloss = PlayerHandler.GetEloUpdate(PhotonNetwork.LocalPlayer, 
                result == EndGameScreenController.Result.Win,
                PhotonNetwork.PlayerList.ToList());
            
            PlayerHandler.UpdateLocalPlayerElo(eloPrevUpdate + gainloss, success => {
                if (success)
                {
                    Debug.Log("Sucessfully Updated the Elo of LocalPlayer");
                }
            });
            controller.SetNewElo(eloPrevUpdate, gainloss);
        }
    }
}
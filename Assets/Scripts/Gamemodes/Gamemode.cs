using Photon.Pun;
using PlayerManagement;

namespace Scripts.Gamemodes
{
    public abstract class Gamemode : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Called after the player has been respawned by the GameManager
        /// </summary>
        /// <param name="playerManager">new playerManager linked to the player</param>
        public abstract void OnPlayerRespawn(PlayerManager playerManager);

        /// <summary>
        /// Called when the player is declared dead by the GameManager
        /// </summary>
        public abstract void OnPlayerDeath();
    }
}
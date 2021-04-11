using Photon.Pun;
using PlayerManagement;

namespace Scripts.Gamemodes
{
    public abstract class Gamemode : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Called after the player has been respawned by the GameManager
        /// </summary>
        public abstract void OnPlayerRespawn(PlayerManager playerManager);
    }
}
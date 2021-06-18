using System;
using System.Collections.Generic;
using Photon.Pun;
using PlayerManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Multiplayer
{
    public class FFAManager : GameManager
    {
        [Header("Spawn Points")]
        public List<Transform> SpawnPoints;

        #region MonoBehaviour Callbacks

        private void Start()
        {
            ReadyToSpawn = true;
        }

        public bool ReadyToSpawn = false;
        public PlayerManager playerManager;
        private void Update()
        {
            if (ReadyToSpawn && PlayerManager.LocalPlayerInstance == null)
            {
                playerStartPos = SelectSpawnPoint();
                playerManager = RespawnPlayer();
            }

            if (playerManager != null)
            {
                if (playerManager.Health <= 0)
                {
                    playerManager = RespawnPlayer();
                }
            }
        }

        #endregion

        #region Private Methods

        PlayerManager RespawnPlayer()
        {
            if (PlayerManager.LocalPlayerInstance != null)
            {
                PhotonNetwork.Destroy(PlayerManager.LocalPlayerInstance);
                PlayerManager.LocalPlayerInstance = null;
                playerManager = null;
            }

            // TODO: Buy Menu before respawning

            return InstantiateLocalPlayer();
        }

        /// <summary>
        /// Selects a random spawn point
        /// </summary>
        /// <returns>Vector3 representing the spawnpoint position</returns>
        Vector3 SelectSpawnPoint()
        {
            return SpawnPoints[Random.Range(0, SpawnPoints.Count)].position;
        }

        #endregion
    }
}
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

        private bool ReadyToSpawn = false;
        private PlayerManager playerManager;
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
            GameObject _gunPrefab = null;
            
            if (PlayerManager.LocalPlayerInstance != null)
            {
                // temp: add previous weapon to the new instance
                _gunPrefab = playerManager.playerWeapon;
                PhotonNetwork.Destroy(PlayerManager.LocalPlayerInstance);
                PlayerManager.LocalPlayerInstance = null;
                playerManager = null;
            }

            // TODO: Buy Menu before respawning

            var _playerManager = InstantiateLocalPlayer();
            
            // temp: add previous weapon to the new instance
            if (_gunPrefab != null)
            {
                _playerManager.AddGunPrefabToPlayer(_gunPrefab);    
            }

            return _playerManager;
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
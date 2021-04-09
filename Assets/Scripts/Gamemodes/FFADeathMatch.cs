using System;
using Multiplayer;
using PlayerManagement;
using UnityEngine;

namespace Scripts.Gamemodes
{
    public class FFADeathMatch : MonoBehaviour
    {
        #region MonoBehaviour Callbacks

        private void Start()
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                
            }
        }

        #endregion
    }
}
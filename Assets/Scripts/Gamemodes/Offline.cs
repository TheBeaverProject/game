using System;
using Multiplayer;
using Photon.Pun;
using PlayerManagement;
using UI;
using UI.BuyMenu;
using UI.HUD;
using UnityEngine;

namespace Scripts.Gamemodes
{
    public class Offline : GameManager
    {
        public Launcher launcher;
        
        private void Start()
        {
            PhotonNetwork.OfflineMode = true;
            
            launcher.Connect();

            InstantiateLocalPlayer();
        }
    }
}
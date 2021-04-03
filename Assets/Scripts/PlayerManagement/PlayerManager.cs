﻿using Guns;
using Photon.Pun;
using UnityEngine;

namespace PlayerManagement
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("HUD of the player")]
        public UI.HUD.Controller HUD;

        [Tooltip("Camera follwing the player")]
        public Camera playerCamera;

        [Tooltip("Camera used to display the weapon")]
        public Camera weaponCamera;
        
        [Tooltip("The current Health of our player")]
        [SerializeField]
        private int health = 100;

        public int Health
        {
            get => health;
            set => health = value < 0 ? 0 : value;
        }
        
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        #endregion
        
        
        #region Private Fields
        
        #endregion

        
        #region MoboBehaviour Callbacks

        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        private void Awake()
        {
            var instancePlayer = this.gameObject;
            if (PhotonNetwork.IsConnected && photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = instancePlayer;
            }
        }
        

        private void Start()
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine)
            {
                return;
            }
            
            HUD.playerName.text = PhotonNetwork.NickName;
        }

        #endregion

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting) // Local Player
            {
                stream.SendNext(Health);
                //Debug.Log($"Sent Health for {this.gameObject.name}: {Health}");
            }
            else if (stream.IsReading) // Network Player
            {
                Health = (int) stream.ReceiveNext();
                //Debug.Log($"Received Health for {this.gameObject.name}: {tempHealth}");
            }
        }

        #region Player

        private GameObject playerGun;
        
        public void AddGunPrefabToPlayer(GameObject gunPrefab)
        {
            if (playerGun != null)
            {
                PhotonNetwork.Destroy(playerGun);
            }
            
            var transform = playerCamera.transform;
            playerGun = PhotonNetwork.Instantiate(gunPrefab.name, transform.position, transform.rotation, 0);
            
            // Sets the gun as the children of the camera
            playerGun.transform.SetParent(transform);

            // Position correctly the gun
            // Local values so it looks good on camera
            playerGun.transform.Rotate(gunPrefab.transform.rotation.eulerAngles);
            playerGun.transform.localPosition = playerGun.GetComponent<Gunnable>().weaponCameraPlacement;
            playerGun.transform.RotateAround(playerGun.transform.position, Vector3.up, -2);
            playerGun.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            // Sets the holder of the gun
            playerGun.GetComponent<Gunnable>().holder = this;
        }

        public void DisableShooting()
        {
            if (playerGun == null)
                return;
            
            playerGun.GetComponent<Gunnable>().AllowShooting = false;
        }

        public void EnableShooting()
        {
            if (playerGun == null)
                return;
            
            playerGun.GetComponent<Gunnable>().AllowShooting = true;
        }

        public void TakeDamage(double weaponDamage, LayerMask bodyZone)
        {
            Debug.Log("TakeDamage called");
            
            switch (bodyZone)
            {
                case 9: // playerHead
                    weaponDamage *= 2;
                    break;
                case 10: // playerBody
                    weaponDamage *= 1;
                    break;
                case 11: // playerLegs
                    weaponDamage *= 0.75;
                    break;
            }

            int newHealth =  Health - ((int) weaponDamage);

            photonView.RPC("UpdateHealth", RpcTarget.All, newHealth);
        }

        [PunRPC] 
        void UpdateHealth(int newHealth)
        {
            Health = newHealth;
            
            if (PhotonNetwork.IsConnected && photonView.IsMine)
            {
                HUD.healthDisplay.SetHUDHealth(Health);
            }
        }
        
        #endregion
    }
}
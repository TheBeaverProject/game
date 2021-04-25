using System.Collections.Generic;
using Guns;
using Multiplayer;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Scripts;
using TMPro;
using UI;
using UnityEngine;

namespace PlayerManagement
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("HUD of the player")]
        public UI.HUD.Controller HUD;

        [Tooltip("Holder of the camera follwing the player")]
        public GameObject playerCameraHolder;
        
        [Tooltip("Camera follwing the player")]
        public Camera playerCamera;

        [Tooltip("Camera used to display the weapon")]
        public Camera weaponCamera;

        [Tooltip("Text used to display infos about the player")]
        public TextMeshPro playerText;

        [Tooltip("Line renderer to display bullets traces")]
        public LineRenderer lineRenderer;
        
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
            playerText.text = $"{PhotonNetwork.NickName}";
        }

        #endregion

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting) // Local Player
            {
                stream.SendNext(Health);
            }
            else if (stream.IsReading) // Network Player
            {
                Health = (int) stream.ReceiveNext();
            }
        }

        #region Player

        public GameObject playerWeapon;
        
        public void AddGunPrefabToPlayer(GameObject gunPrefab)
        {
            if (playerWeapon != null)
            {
                PhotonNetwork.Destroy(playerWeapon);
            }
            
            var transform = playerCamera.transform;
            playerWeapon = PhotonNetwork.Instantiate(gunPrefab.name, transform.position, transform.rotation, 0);
            
            // Sets the gun as the children of the camera
            playerWeapon.transform.SetParent(transform);
            
            // Sets the layer so it is rendered by the weaponCamera
            Utils.SetLayerRecursively(playerWeapon, 12);
            playerWeapon.layer = 12;
            

            // Position correctly the gun
            // Local values so it looks good on camera
            playerWeapon.transform.Rotate(gunPrefab.transform.rotation.eulerAngles);
            playerWeapon.transform.localPosition = playerWeapon.GetComponent<Gunnable>().weaponCameraPlacement;
            playerWeapon.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            // Sets the holder of the gun
            var GunnableScript = playerWeapon.GetComponent<Gunnable>();
            GunnableScript.holder = this;
            
            if (GunnableScript is HitScanWeapon)
            {
                ((HitScanWeapon) GunnableScript).lineRenderer = this.lineRenderer;
            }
        }

        public void DisableShooting()
        {
            if (playerWeapon == null)
                return;
            
            playerWeapon.GetComponent<Gunnable>().AllowShooting = false;
        }

        public void EnableShooting()
        {
            if (playerWeapon == null)
                return;
            
            playerWeapon.GetComponent<Gunnable>().AllowShooting = true;
        }

        public void DisableMovement()
        {
            this.gameObject.GetComponent<PlayerMenusHandler>().UnLockCursor();
            this.gameObject.GetComponent<PlayerMovementManager>().enabled = false;
        }

        public void EnableMovement()
        {
            this.gameObject.GetComponent<PlayerMenusHandler>().LockCursor();
            this.gameObject.GetComponent<PlayerMovementManager>().enabled = true;
        }
        
        // List to hold the actor number of the players who dealt damage to this player and the dealt damage
        public Dictionary<int, int> TookDamageFrom = new Dictionary<int, int>();

        private void AddDamageDealer(int dealerActorNumber, int damage)
        {
            if (TookDamageFrom.ContainsKey(dealerActorNumber))
            {
                TookDamageFrom[dealerActorNumber] += damage;
            }
            else
            {
                TookDamageFrom.Add(dealerActorNumber, damage);
            }
        }

        private int GetAssistActorNum(int killerActorNumber)
        {
            int assistActorNum = -1;
            int assistDamage = 0;
            
            foreach (var kvp in TookDamageFrom)
            {
                if (assistDamage < kvp.Value && kvp.Key != killerActorNumber)
                {
                    assistActorNum = kvp.Key;
                }
            }

            return assistActorNum;
        }

        private bool killed = false;
        public void TakeDamage(double weaponDamage, LayerMask bodyZone, Player dealer)
        {
            if (killed)
            {
                return;
            }
            
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

            if (newHealth <= 0 && !killed) // Kill -> Raise event
            {
                Events.SendKillEvent(dealer.ActorNumber, GetAssistActorNum(dealer.ActorNumber), photonView.OwnerActorNr);
                killed = true;
            }

            photonView.RPC("UpdateHealth", RpcTarget.All, newHealth, dealer.ActorNumber);
        }

        [PunRPC] 
        void UpdateHealth(int newHealth, int dealerActorNumber)
        {
            AddDamageDealer(dealerActorNumber, Health - newHealth);
            
            Health = newHealth;
            
            if (PhotonNetwork.IsConnected && photonView.IsMine)
            {
                HUD.healthDisplay.SetHUDHealth(Health);
            }
        }
        
        #endregion
    }
}
using Guns;
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
        
        [Tooltip("The current Health of our player")]
        [SerializeField]
        private int health = 100;

        // Temporary
        [Tooltip("Prefab used by the gun TEMPORARY")] [SerializeField]
        private GameObject gunPrefab;

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
        }

        private void Update()
        {
            if (PhotonNetwork.IsConnected && photonView.IsMine == false)
            {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.B) && playerGun == null)
            {
                photonView.RPC("AddGunPrefabToPlayer", RpcTarget.All);
            }
        }

        #endregion

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting) // Local Player
            {
                stream.SendNext(Health);
            }
            else // Network Player
            {
                this.Health = (int) stream.ReceiveNext();

                if (photonView.IsMine)
                {
                    Debug.Log("Updating health on the HUD");
                    HUD.healthDisplay.SetHUDHealth(Health);
                }
            }
        }
        
        #region Player

        private GameObject playerGun;
        
        [PunRPC] void AddGunPrefabToPlayer()
        {
            var transform = playerCamera.transform;
            playerGun = Instantiate(gunPrefab, transform.position, transform.rotation, transform);
            
            playerGun.transform.Rotate(gunPrefab.transform.rotation.eulerAngles);
            playerGun.transform.localPosition = gunPrefab.transform.position;
            playerGun.transform.RotateAround(playerGun.transform.position, Vector3.up, -3);
            
            playerGun.GetComponent<Gunnable>().holder = this;
        }

        public void TakeDamage(double weaponDamage, LayerMask bodyZone)
        {
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

            Health -= (int) weaponDamage;
            
            if (PhotonNetwork.IsConnected && photonView.IsMine)
            {
                HUD.healthDisplay.SetHUDHealth(Health);
            }
        }
        
        #endregion
    }
}
using Photon.Pun;
using UnityEngine;

namespace PlayerManagement
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("HUD of the player")]
        public UI.HUD.Controller HUD;
        
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
        }

        private void Update()
        {
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
            }
        }
        
        #region Player Methods

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
            HUD.healthDisplay.SetHUDHealth(Health);
        }
        
        #endregion
    }
}
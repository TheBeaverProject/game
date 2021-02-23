using Photon.Pun;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("HUD of the player")]
        public UI.HUD.Controller HUD;
        
        [Tooltip("The current Health of our player")]
        public int Health = 100;
        
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
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = instancePlayer;
            }
            
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(instancePlayer);
        }
        

        private void Start()
        {
            if (PhotonNetwork.IsConnected && photonView.IsMine)
            {
                HUD.playerName.text = PhotonNetwork.NickName;
            }
        }

        private void Update()
        {
        }

        #endregion

        #region IPunObservable implementation
        
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
        
        #endregion
        
        #region Player Control
        
        #endregion
    }
}
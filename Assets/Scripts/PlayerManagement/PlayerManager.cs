using System;
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
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ChromaticAberration = UI.Effects.ChromaticAberration;
using Random = UnityEngine.Random;

namespace PlayerManagement
{
    public enum PlayerType
    {
        Client,
        IA
    }
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
        
        public PlayerType Type;

        public Transform shootingTransform;

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
            if (PhotonNetwork.IsConnected)
            {
                playerText.text = $"{photonView.Controller.NickName}";

                PhotonTeam team = photonView.Controller.GetPhotonTeam();
                if (team != null)
                {
                    playerText.color = team.Color;
                }
            }

            if (PhotonNetwork.IsConnected && !photonView.IsMine)
            {
                return;
            }

            _volume = FindObjectOfType<Volume>();
            _volume.profile.TryGet(out _chromaticAberration);
            _volume.profile.TryGet(out _colorAdjustments);
            
            // Fix a l'arrache pour avoir les bonnes valeurs quand on spawn
            _chromaticAberration.intensity.value = 0;
            _colorAdjustments.saturation.value = 8.3f;
            
            satPrevValue = _colorAdjustments.saturation.value;

            if (Type == PlayerType.Client)
            {
                // Set this to avoid the camera crashing into the walls
                GetComponent<CharacterController>().radius = 1;
                
                HUD.playerName.text = PhotonNetwork.NickName;
                this.gameObject.name = PhotonNetwork.NickName;
            }
            else
            {
                int numberOfBot = FindObjectsOfType<AI>().Length;
                playerText.text = $"Bot #{numberOfBot}";
                this.gameObject.name = $"Bot #{numberOfBot}";
            }
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

            Gunnable gunnable;
            
            if (Type == PlayerType.IA)
            {
                
                playerWeapon = PhotonNetwork.Instantiate(gunPrefab.name, new Vector3(0, 0.7f, 0), this.transform.rotation);
                var gunnableScript = playerWeapon.GetComponent<Gunnable>();

                // Sets the holder of the gun
                gunnable = playerWeapon.GetComponent<Gunnable>();
                gunnable.holder = this;
                gunnable.PlaceAIWeapon();
            }
            else
            {
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
                gunnable = playerWeapon.GetComponent<Gunnable>();
                gunnable.holder = this;
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

        private ColorAdjustments _colorAdjustments;
        private float satPrevValue;
        public void EnterSpecMode()
        {
            if (Type == PlayerType.IA)
            {
                this.gameObject.SetActive(false);
                return;
            }
            
            DisableShooting();
            this.gameObject.GetComponent<PlayerMovementManager>().enabled = false;
            this.gameObject.GetComponent<PlayerMenusHandler>().allowBuy = false;

            // Set the screen in surveillance camera like
            _colorAdjustments.saturation.value = -100f;
            _chromaticAberration.intensity.value = 1f;
            
            photonView.RPC("SpecMode", RpcTarget.All);
            
            HUD.DisplayAnnouncement("You've entered spec mode until the round ends.");
        }

        public void ExitSpecMode()
        {
            // Reset the screen effects
            _colorAdjustments.saturation.value = satPrevValue;
            _chromaticAberration.intensity.value = 0f;
            this.gameObject.GetComponent<PlayerMenusHandler>().allowBuy = true;
            this.gameObject.GetComponent<PlayerMovementManager>().enabled = true;
        }

        [PunRPC]
        void SpecMode()
        {
            // Remove the layers so player does not interact with zones and grenades
            Utils.SetLayerRecursively(this.gameObject, 0);
            
            playerText.text = "[Spec] " + playerText.text;
            
            GameObject playerModel = null;

            foreach (var go in this.GetComponentsInChildren<Animator>())
            {
                Debug.Log("found " + go.name);
                
                if (go.name == "French")
                {
                    playerModel = go.gameObject;
                }
            }

            if (playerModel == null)
            {
                Debug.LogWarning("PlayerManager.EnterSpecMode: Could not find the player model");
            }
            else
            {
                playerModel.SetActive(false);
            
                if (playerWeapon != null)
                {
                    playerWeapon.SetActive(false);
                }
            }
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
            
            photonView.RPC("DamageEffect", RpcTarget.All, (float) weaponDamage);

            if (newHealth <= 0 && !killed) // Kill -> Raise event
            {
                Events.SendKillEvent(dealer.ActorNumber, GetAssistActorNum(dealer.ActorNumber), photonView.OwnerActorNr);
                killed = true;
            }

            photonView.RPC("UpdateHealth", RpcTarget.AllViaServer, newHealth, dealer.ActorNumber);
        }

        private Volume _volume;
        private UnityEngine.Rendering.Universal.ChromaticAberration _chromaticAberration;
        [PunRPC] 
        void DamageEffect(float amount)
        {
            if (!photonView.IsMine || Type == PlayerType.IA) return;   
            
            var effScript = playerCamera.GetComponent<ChromaticAberration>();
            float duration = 0.4f;
            float clampedAmout = amount / 100;

            StartCoroutine(Utils.SmoothTransition(f =>
            {
                _chromaticAberration.intensity.value = Mathf.SmoothStep(0, clampedAmout, f);
            }, duration / 4, (() =>
            {
                StartCoroutine(Utils.SmoothTransition(f =>
                {
                    _chromaticAberration.intensity.value = Mathf.SmoothStep(clampedAmout, 0, f);
                }, duration / 2 + duration / 4));
            })));
        }

        [PunRPC] 
        void UpdateHealth(int newHealth, int dealerActorNumber)
        {
            AddDamageDealer(dealerActorNumber, Health - newHealth);
            
            Health = newHealth;
            
            if (PhotonNetwork.IsConnected && photonView.IsMine && Type != PlayerType.IA)
            {
                HUD.healthDisplay.SetHUDHealth(Health);
            }
        }
        
        #endregion
    }
}
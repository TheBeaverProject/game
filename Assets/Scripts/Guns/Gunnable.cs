using System;
using System.Collections;
using Photon.Pun;
using PlayerManagement;
using Scripts;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Guns
{
    public abstract class Gunnable: MonoBehaviourPun
    {
        #region Weapon Attributes
        //Weapon Name
        [Header("Weapon Attributes")]
        [SerializeField]
        public string weaponName;
        
        //Weapon Damage
        [SerializeField]
        public int damage;
        
        // Magazine
        [SerializeField]
        protected int magazineSize;
        [SerializeField]
        protected int magazineNumber;

        // Weapon Scope
        [Header("Scope settings")]
        [SerializeField]
        public bool allowScope;
        
        [SerializeField]
        public float scopedFOV = 30f;

        [SerializeField]
        public ScopeHUDController.scopeType ScopeType;
        
        // Weapon Behavior
        [Header("Weapon Behavior")]
        [SerializeField]
        public bool allowButtonHold;
        [SerializeField]
        protected float timeBetweenShooting;
        [SerializeField]
        protected float reloadTime;
        
        [Tooltip("Burst fire bullets (set to 1 to disable burst)")]
        [SerializeField]
        public int bulletsPerTap;
        
        [Tooltip("Burst Fire time between shots")]
        [SerializeField]
        protected float timeBetweenShots;
        protected bool shooting, readyToShoot, reloading;
        
        [Header("Spread mechanism")]
        [SerializeField]
        public float spread;
        [SerializeField]
        public float range;
        
        [Header("Recoil Settings")]
        public float rotationSpeed = 6;
        public float returnSpeed = 25;
        public Vector3 RecoilRotation = new Vector3(2f, 2f, 2f);
        public Vector3 RecoilRotationAiming = new Vector3(0.5f, 0.5f, 0.5f);

        // Weapon placement
        [Header("Weapon placement")]
        [Tooltip("Weapon placement relative to the body")]
        public Vector3 weaponBodyPlacement;
        
        [Tooltip("Weapon placement relative to the camera")]
        public Vector3 weaponCameraPlacement;

        [Tooltip("Weapon position when aiming (used only with reddot weapons)")]
        public Vector3 weaponAimingPlacement = new Vector3(0, -0.1f, 0.1f);

        [Tooltip("Position of extremity of the barrel")]
        public GameObject barrelTip;

        // Visual Effects
        [Header("Visual Effects")] 
        [FormerlySerializedAs("lineRenderer")]
        public LineRenderer bulletTrail;
        
        public GameObject bulletImpact;

        public GameObject hitParticles;
        
        public GameObject bloodParticles;
        
        public GameObject MuzzleFlash;

        // Sound Effects
        [Header("Sound Effects")]
        public AudioSource weaponAudioSource;
        public AudioClip singleShotSoundEffect;

        [Header("AI related")] public bool AIShooting;
        
        protected int bulletsLeft, bulletsShot, magazineUsed = 0;
        public int GetMagSize => magazineSize;
        public int GetMagLeft => magazineNumber - magazineUsed;
        public float GetTimeBetweenShooting => timeBetweenShooting;
        public float GetTimeBetweenShoot => timeBetweenShots;
        public int GetBulletsLeft => bulletsLeft;

        public bool aiming;

        public bool AllowShooting = true;
        
        #endregion


        /// <summary>
        /// PlayerManager which the gun belongs to.
        /// </summary>
        public PlayerManager holder;

        #region MonoBehaviour Callbacks

        private void Start()
        {
            readyToShoot = true;
            bulletsLeft = magazineSize;

            if (!PhotonNetwork.IsConnected) return;
            
            if (photonView.Owner == null)
                return;

            if (photonView.IsMine)
            {
                // Update the HUD
                holder.HUD.UpdateWeaponDisplay(this);
                
                // Set the recoil values on the camera script
                holder.playerCameraHolder.GetComponent<CameraRecoil>().SetValues(this);
            }
            else if (holder.Type != PlayerType.IA)
            {
                FindWeaponParent();
            }
        }
        
        private void Update()
        {
            if (!AllowShooting)
                return;

            if (holder.Type == PlayerType.Client)
                MyInput();
            else
                AIInput();
        }

        private void OnDestroy()
        {
            // Set the weapon to not aim before it is destroyed
            if (aiming)
            {
                Aim();
            }
        }

        #endregion
        
        #region Input Mechanics
        
        protected abstract void MyInput();

        protected abstract void AIInput();

        #endregion
        
        #region Shooting Mechanics

        protected abstract void Shoot();

        protected abstract void ResetShot();

        #endregion

        #region Reloading Mechanics

        protected void Reload()
        {
            if (magazineUsed == magazineNumber)
            {
                return;
            }
            
            reloading = true;
            //Calls ReloadFinished Method after reloadTime
            Invoke(nameof(ReloadFinished), reloadTime);
        }

        protected void ReloadFinished()
        {
            //Finishes the reload
            bulletsLeft = magazineSize;
            reloading = false;
            magazineUsed++;
            
            // Update the HUD
            holder.HUD.UpdateWeaponDisplay(this);
        }

        #endregion

        #region Scope

        private float baseFov;
        private Quaternion baseRotation;
        
        public void Aim()
        {
            if (aiming)
            {
                aiming = false;
                holder.HUD.DisplayCrosshair(true);
                holder.GetComponentInChildren<ScopeHUDController>().Toggle(ScopeType);
                
                if (ScopeType != ScopeHUDController.scopeType.RedDot)
                {
                    holder.weaponCamera.gameObject.SetActive(true);
                }

                StartCoroutine(Utils.SmoothTransition(
                    f => transform.localPosition = Vector3.Lerp(this.transform.localPosition, weaponCameraPlacement, f)
                    , 0.1f));
                
                StartCoroutine(Utils.SmoothTransition(
                    f => holder.playerCamera.fieldOfView = Mathf.Lerp(scopedFOV, baseFov, f),
                    0.1f));
            }
            else
            {
                aiming = true;
                baseFov = holder.playerCamera.fieldOfView;
                holder.HUD.DisplayCrosshair(false);
                
                if (ScopeType != ScopeHUDController.scopeType.RedDot)
                {
                    holder.weaponCamera.gameObject.SetActive(false);
                }

                StartCoroutine(Utils.SmoothTransition(
                    f => transform.localPosition = Vector3.Lerp(this.transform.localPosition, weaponAimingPlacement, f)
                    , 0.1f, () =>
                    {
                        holder.GetComponentInChildren<ScopeHUDController>().Toggle(ScopeType);
                    }));
                
                StartCoroutine(Utils.SmoothTransition(
                    f => holder.playerCamera.fieldOfView = Mathf.Lerp(baseFov, scopedFOV, f),
                    0.1f));
            }
        }

        #endregion

        #region Private Methods

        private void FindWeaponParent()
        {
            foreach (var view in PhotonNetwork.PhotonViewCollection)
            {
                // Looks for a player object with the same controller => the parent of the gun
                if (view.Controller.Equals(photonView.Controller) && view.TryGetComponent(out holder))
                {
                    // Sets the parent if the gun is not ours
                    transform.SetParent(holder.transform);

                    transform.position = holder.transform.position;
                    transform.rotation = holder.transform.rotation;
                    transform.Rotate(0, 180, 0);
                    transform.localPosition = weaponBodyPlacement;
                    transform.RotateAround(transform.position, Vector3.up, -2);
                    transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                }
            }
        }

        #endregion
    }
}
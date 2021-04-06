﻿using System;
using System.Collections;
using Photon.Pun;
using PlayerManagement;
using Scripts;
using UI;
using UnityEngine;

namespace Guns
{
    public abstract class Gunnable: MonoBehaviourPun
    {
        //Weapon Name
        [Tooltip("Weapon Name")]
        [SerializeField]
        public string weaponName;
        
        //Weapon Damage
        [Tooltip("Weapon Damage")]
        [SerializeField]
        public int damage;

        // Weapon Scope
        [Tooltip("Has a scope")]
        [SerializeField]
        public bool allowScope;

        [Tooltip("Scoped FOV")]
        [SerializeField]
        public float scopedFOV = 30f;

        [Tooltip("Type of the weapon's scope")]
        [SerializeField]
        public ScopeHUDController.scopeType ScopeType;
        
        // Weapon Behavior
        [Tooltip("Allow automatic Firing")]
        [SerializeField]
        public bool allowButtonHold;
        
        [Tooltip("Firing cadence (in seconds)")]
        [SerializeField]
        protected float timeBetweenShooting;

        [Tooltip("Reloading time (in seconds")]
        [SerializeField]
        protected float reloadTime;
        
        [SerializeField]
        public float spread;
        [SerializeField]
        public float range;

        // Gun behavior
        [Tooltip("Burst fire bullets (set to 1 to disable burst)")]
        [SerializeField]
        public int bulletsPerTap;
        
        [Tooltip("Burst Fire time between shots")]
        [SerializeField]
        protected float timeBetweenShots;
        protected bool shooting, readyToShoot, reloading;
        
        // Magazine
        [SerializeField]
        protected int magazineSize;
        [SerializeField]
        protected int magazineNumber;
        
        protected int bulletsLeft, bulletsShot, magazineUsed = 0;
        public int GetMagSize => magazineSize;
        public int GetMagLeft => magazineNumber - magazineUsed;
        public int GetBulletsLeft => bulletsLeft;

        public bool aiming;

        public bool AllowShooting = true;
        

        // Weapon placement
        [Tooltip("Weapon placement relative to the body")]
        public Vector3 weaponBodyPlacement;
        
        [Tooltip("Weapon placement relative to the camera")]
        public Vector3 weaponCameraPlacement;

        [Tooltip("Weapon position when aiming (used only with reddot weapons)")]
        public Vector3 weaponAimingPlacement = new Vector3(0, -0.1f, 0.1f);

        [Tooltip("Position of extremity of the barrel")]
        public GameObject barrelTip;

        // Sound Effects
        public AudioSource weaponAudioSource;
        public AudioClip singleShotSoundEffect;
        
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
                weaponAudioSource.name = PhotonNetwork.NickName;
            }
            else // View is not ours, we need to find the parent
            {
                FindWeaponParent();
            }
        }
        
        private void Update()
        {
            if (!AllowShooting)
                return;

            //MyInput dictates weapon beahvior
            MyInput();
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

        #endregion
        
        #region Shooting Mechanics

        protected abstract void Shoot();
        
        protected void ResetShot()
        {
            readyToShoot = true;
        }

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
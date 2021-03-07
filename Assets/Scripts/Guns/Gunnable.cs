using Photon.Pun;
using PlayerManagement;
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
        //Damage Getter
        public int GetDamage => damage;

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

        public bool AllowShooting = true;
        

        // Weapon placement
        [Tooltip("Weapon placement relative to the body")]
        public Vector3 weaponBodyPlacement;
        
        [Tooltip("Weapon placement relative to the camera")]
        public Vector3 weaponCameraPlacement;

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
        }
        
        private void Update()
        {
            if (!AllowShooting)
                return;

            //MyInput dictates weapon beahvior
            MyInput();
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
    }
}
using Photon.Pun;
using PlayerManagement;
using UnityEngine;

namespace Guns
{
    public abstract class Gunnable: MonoBehaviourPun
    {
        //Weapon Damage
        [Tooltip("Weapon Damage")]
        [SerializeField]
        protected int damage;
        //Weapon Name
        [Tooltip("Weapon Name")]
        [SerializeField]
        public string weaponName;
        
        // Sound Effects
        public AudioSource weaponAudioSource;
        public AudioClip singleShotSoundEffect;
        
        //Damage Getter
        public int GetDamage => damage;
        
        //Gun behavior
        protected bool allowButtonHold, shooting, readyToShoot, reloading;
        protected float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
        
        // Magazine size
        // TODO: Implement magazine number
        protected int bulletsLeft, bulletsShot;
        protected int magazineSize, bulletsPerTap;
        public int GetMagSize => magazineSize;
        public int GetMagLeft => bulletsLeft;
    
        /// <summary>
        /// PlayerManager which the gun belongs to.
        /// </summary>
        public PlayerManager holder;
        
        //Raycast hit
        protected RaycastHit rayHit;
        
        //Gets Player Input
        protected abstract void MyInput();
        
        protected void Reload()
        {
            Debug.Log("Reloading");
            reloading = true;
            //Calls ReloadFinished Method after reloadTime
            Invoke("ReloadFinished", reloadTime);
        }

        protected void ReloadFinished()
        {
            //Finishes the reload
            bulletsLeft = magazineSize;
            reloading = false;
            
            // Update the HUD
            holder.HUD.UpdateWeaponDisplay(this);
        }
        
        protected abstract void Shoot();
        
        protected void ResetShot()
        {
            readyToShoot = true;
        }
    }
}
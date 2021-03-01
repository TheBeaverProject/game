using Photon.Pun;
using PlayerManagement;
using UnityEngine;

namespace Guns
{
    public abstract class Gunnable: MonoBehaviourPun
    {
        [Tooltip("Weapon Damage")]
        [SerializeField]
        protected int damage;

        [Tooltip("Weapon Name")]
        [SerializeField]
        public string weaponName;
        
        public int GetDamage => damage;
        
        //Gun behavior
        protected bool allowButtonHold, shooting, readyToShoot, reloading;
        protected float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
        
        // Magazine size
        protected int bulletsLeft, bulletsShot;
        protected int magazineSize, bulletsPerTap;
        public int GetMagSize => magazineSize;
        public int GetMagLeft => bulletsLeft;
    
        /// <summary>
        /// PlayerManager which the gun belongs to.
        /// </summary>
        public PlayerManager holder;
        
        public LayerMask ennemy;
        
        //Raycast hit
        protected RaycastHit rayHit;
        
        protected abstract void MyInput();
        protected abstract void Reload();
        protected abstract void ReloadFinished();
        protected abstract void Shoot();
        protected abstract void ResetShot();
    }
}
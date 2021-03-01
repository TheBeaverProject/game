using Photon.Pun;
using UnityEngine;

namespace Guns
{
    public abstract class Gunnable: MonoBehaviourPun
    {
        [Tooltip("Weapon Damage")]
        [SerializeField]
        protected int damage;
        
        public int GetDamage => damage;
        
        //Gun behavior
        protected bool allowButtonHold, shooting, readyToShoot, reloading;
        protected float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
        
        // Magazine size
        protected int bulletsLeft, bulletsShot;
        protected int magazineSize, bulletsPerTap;
        protected int GetMagSize => magazineSize;
        protected int GetMagLeft => bulletsLeft;

        public Camera cam;
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
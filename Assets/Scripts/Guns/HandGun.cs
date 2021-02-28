using System;
using System.Security.Cryptography;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Guns
{
    public class HandGun : MonoBehaviourPun , IGunnable

    {
        int damage;
        public int GetDamage => damage;
        float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
        int GetMagSize => magazineSize;
        int magazineSize, bulletsPerTap;
        bool allowButtonHold;
        int bulletsLeft, bulletsShot;
        int GetMagLeft => bulletsLeft;
        public Camera cam;
        public LayerMask ennemy;
        
        //Gun behavior
        bool shooting, readyToShoot, reloading;
        
        //Raycast hit
        RaycastHit rayHit;
        
        

        private void Start()
        {
            cam = GetComponentInParent<GameObject>().GetComponent<Camera>();
            bulletsLeft = magazineSize;
            readyToShoot = true;
        }

        private void Update()
        {
            if (PhotonNetwork.IsConnected && photonView.IsMine == false)
            {
                return;
            }
            MyInput();
        }

        public void MyInput()
        {
            if (allowButtonHold)
                shooting = Input.GetKey(KeyCode.Mouse0);
            else
                shooting = Input.GetKeyDown(KeyCode.Mouse0);

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) ;
            Reload();
            //Shooting mechanic
            if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
            {
                bulletsShot = bulletsPerTap;
                Shoot();
            }
        }

        public void Reload()
        {
            reloading = true;
            Invoke("ReloadFinished", reloadTime);
        }

        public void ReloadFinished()
        {
            bulletsLeft = magazineSize;
            reloading = false;
        }

        public void Shoot()
        {
            readyToShoot = false;
            
            //Spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);
            Vector3 direction = cam.transform.forward + new Vector3(x, y, 0);
            //RayCast
            if (Physics.Raycast(cam.transform.position, direction, out rayHit, range, ennemy))
            {
                Debug.Log(rayHit.collider.name);
                
                //TODO when player has health and can take damage (implement TakeDamage method)
                //if (rayHit.collider.CompareTag("Enemy"))
                //    rayHit.collider.GetComponent<ShootingAI>().TakeDamage(damage);
            }
            bulletsLeft--;
            bulletsShot--;
            Invoke("ResetShot",timeBetweenShooting);
            
            if (bulletsShot > 0 && bulletsLeft > 0)
                Invoke("Shoot",timeBetweenShots);
        }

        public void ResetShot()
        {
            readyToShoot = true;
        }
        
    }
}
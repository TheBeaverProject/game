using System;
using System.Security.Cryptography;
using Photon.Pun;
using PlayerManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Guns
{
    public class Rifle : Gunnable 
    {
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

        protected override void MyInput()
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

        protected override void Reload()
        {
            reloading = true;
            Invoke("ReloadFinished", reloadTime);
        }

        protected override void ReloadFinished()
        {
            bulletsLeft = magazineSize;
            reloading = false;
        }

        protected override void Shoot()
        {
            readyToShoot = false;
            
            //Spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);
            Vector3 direction = cam.transform.forward + new Vector3(x, y, 0);
            
            //RayCast
            if (Physics.Raycast(cam.transform.position, direction, out rayHit, range, ennemy))
            {
                Debug.Log($"Raycast hit: {rayHit.collider.name}");

                PlayerManager damagedPlayerManager;
                if (rayHit.collider.TryGetComponent<PlayerManager>(out damagedPlayerManager))
                {
                    Debug.Log($"Took Damage: {damagedPlayerManager.GetInstanceID()} - Health: {damagedPlayerManager.Health}");
                    damagedPlayerManager.TakeDamage(damage, rayHit.collider.gameObject.layer);
                }
            }
            
            bulletsLeft--;
            bulletsShot--;
            Invoke("ResetShot",timeBetweenShooting);
            
            if (bulletsShot > 0 && bulletsLeft > 0)
                Invoke("Shoot",timeBetweenShots);
        }

        protected override void ResetShot()
        {
            readyToShoot = true;
        }
    }
}
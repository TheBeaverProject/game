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
            // Initialize Rifle attributes
            weaponName = "Rifle";
            magazineSize = 20;
            bulletsPerTap = 1;
            allowButtonHold = true;
            timeBetweenShooting = 0.1f;
            spread = 0.01f;
            range = 100;
            reloadTime = 3;
            bulletsLeft = magazineSize;
            readyToShoot = true;
            
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Weapon instancied");
                if (photonView.IsMine)
                {
                    Debug.Log("photonView.IsMine");
                    // Update the HUD
                    holder.HUD.UpdateWeaponDisplay(this);
                }
                else // View is not ours, we need to find the parent
                {
                    Debug.Log("photonView.IsNotMine");
                    
                    foreach (var view in PhotonNetwork.PhotonViewCollection)
                    {
                        // Looks for a player object with the same controller => the parent of the gun
                        if (view.Controller.Equals(photonView.Controller) && view.TryGetComponent<PlayerManager>(out holder))
                        {
                            // Sets the parent if the gun is not ours
                            transform.SetParent(holder.transform);
                            transform.position = holder.transform.position;
                            transform.localPosition = new Vector3(0.206f, 0.7f);
                        }
                    }
                }
            }
        }

        private void Update()
        {
            MyInput();
        }

        protected override void MyInput()
        {
            if (holder == null) // Process inputs only if the weapon is bound to a player
                return;
            
            if (allowButtonHold)
                shooting = Input.GetKey(KeyCode.Mouse0);
            else
                shooting = Input.GetKeyDown(KeyCode.Mouse0);

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
                Reload();

            //Shooting mechanic
            if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
            {
                bulletsShot = bulletsPerTap;
                Debug.Log("Ready to shoot");
                Shoot();
            }
        }

        protected override void Shoot()
        {
            Debug.Log("Shooting");
            weaponAudioSource.PlayOneShot(singleShotSoundEffect);
            
            readyToShoot = false;
            
            // Spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);
            Vector3 direction = holder.playerCamera.transform.forward + new Vector3(x, y, 0);
            
            // RayCast
            if (Physics.Raycast(holder.playerCamera.transform.position, direction, out rayHit, range))
            {
                Debug.Log($"Raycast hit: {rayHit.collider.name}");
                Debug.DrawRay(holder.playerCamera.transform.position, direction * rayHit.distance, Color.yellow);
                
                if (rayHit.collider.TryGetComponent<PlayerManager>(out PlayerManager damagedPlayerManager))
                {
                    Debug.Log($"Took Damage: {damagedPlayerManager.GetInstanceID()} - Health: {damagedPlayerManager.Health}");
                    
                    damagedPlayerManager.TakeDamage(damage, rayHit.collider.gameObject.layer);
                }
            }
            else
            {
                Debug.DrawRay(holder.playerCamera.transform.position, direction * 1000, Color.red);
            }
            
            bulletsLeft--;
            bulletsShot--;
            
            // Update the HUD
            holder.HUD.UpdateWeaponDisplay(this);
            
            Invoke("ResetShot", timeBetweenShooting);
            
            if (bulletsShot > 0 && bulletsLeft > 0)
                Invoke("Shoot",timeBetweenShots);
        }
    }
}
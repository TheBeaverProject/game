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
                            transform.rotation = holder.transform.rotation;
                            transform.localPosition = new Vector3(0.206f, 0.4f);
                            transform.Rotate(-90, -90, 0);
                        }
                    }
                }
            }
        }

        private void Update()
        {
            //MyInput dictates weapon beahvior
            MyInput();
        }

        protected override void MyInput()
        {
            if (holder == null) // Process inputs only if the weapon is bound to a player
                return;
            
            if (allowButtonHold)
                //Difference between automatic and non automatic weapons
                shooting = Input.GetKey(KeyCode.Mouse0);
            else
                shooting = Input.GetKeyDown(KeyCode.Mouse0);
            
            //Calls Reload 
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
            //Plays shoot sound
            weaponAudioSource.PlayOneShot(singleShotSoundEffect);
            
            readyToShoot = false;
            
            // Spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);
            Vector3 direction = holder.playerCamera.transform.forward + new Vector3(x, y, 0);
            
            // The raycast starting from the camera with the spread added
            if (Physics.Raycast(holder.playerCamera.transform.position, direction, out rayHit, range))
            {
                Debug.Log($"Raycast hit: {rayHit.collider.name}");
                Debug.DrawRay(holder.playerCamera.transform.position, direction * rayHit.distance, Color.yellow);
                //Damages the player if raycast catch a player
                if (rayHit.collider.TryGetComponent<PlayerManager>(out PlayerManager damagedPlayerManager))
                {
                    Debug.Log($"Took Damage: {damagedPlayerManager.GetInstanceID()} - Health: {damagedPlayerManager.Health}");
                    //Damages the player
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
            
            //Calls ResetShot method after timeBetweenShooting time
            Invoke("ResetShot", timeBetweenShooting);
            
            if (bulletsShot > 0 && bulletsLeft > 0)
                Invoke("Shoot",timeBetweenShots);
        }
    }
}
using System;
using System.Security.Cryptography;
using Photon.Pun;
using PlayerManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Guns
{
    public class HitScanWeapon : Gunnable
    {
        //Raycast hit
        protected RaycastHit rayHit;
        protected int layerMask = ~(1 << 12);

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
                Shoot();
            }
            
            //Aim Mechanic
            if (allowScope && Input.GetKeyDown(KeyCode.Mouse1))
            {
                Aim();
            }
        }

        protected override void AIInput()
        {
            if (holder == null)
                return;

            if (!reloading && bulletsLeft <= 0)
                Reload();

            if (AIShooting && readyToShoot && !reloading && bulletsLeft > 0)
            {
                AIShooting = false; // Reset the state of AIShooting so only one bullet is fired.
                bulletsShot = bulletsPerTap;
                Shoot();
            }
        }

        protected override void Shoot()
        {
            // Plays shoot sound
            photonView.RPC("SpawnShotEffects", RpcTarget.All);
            
            readyToShoot = false;
            
            // Apply Recoil to the camera
            if (holder.playerCameraHolder != null)
            {
                holder.playerCameraHolder.GetComponent<CameraRecoil>().Recoil(aiming);
            }
            
            Vector3 direction = holder.playerCamera.transform.forward;
            
            // The raycast starting from the camera with the spread added
            if (Physics.Raycast(holder.playerCamera.transform.position, direction, out rayHit, range, layerMask))
            {
                photonView.RPC("SpawnBulletTrail", RpcTarget.All, new Vector3[] { barrelTip.transform.position, rayHit.point });

                //Damages the player if raycast catch a player
                PlayerManager damagedPlayerManager;
                damagedPlayerManager = 
                    (damagedPlayerManager = rayHit.collider.GetComponent<PlayerManager>()) == null ? 
                        damagedPlayerManager = rayHit.collider.GetComponentInParent<PlayerManager>() : damagedPlayerManager;
                
                if (damagedPlayerManager != null) // PlayerManager found => we hit a player
                {
                    // Damages the player
                    if (damagedPlayerManager != holder)
                    {
                        damagedPlayerManager.TakeDamage(damage, rayHit.collider.gameObject.layer, photonView.Owner);
                        photonView.RPC("SpawnBlood", RpcTarget.All, rayHit.point, rayHit.normal);
                    }
                }
                else
                {
                    // Not a player -> Spawn bullet hit effect
                    photonView.RPC("SpawnBulletImpact", RpcTarget.All, rayHit.point, rayHit.normal);
                }
            }
            else
            {
                photonView.RPC("SpawnBulletTrail", RpcTarget.All, new Vector3[] { barrelTip.transform.position, barrelTip.transform.position + direction * range });
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

        protected override void ResetShot()
        {
            readyToShoot = true;
        }

        #region RPC Methods

        [PunRPC]
        void SpawnShotEffects()
        {
           weaponAudioSource.PlayOneShot(singleShotSoundEffect);

           if (MuzzleFlash != null)
           {
               var mFlash = Instantiate(MuzzleFlash, barrelTip.transform.position, Quaternion.FromToRotation(Vector3.right, barrelTip.transform.right), barrelTip.transform);
               Destroy(mFlash, 0.12f);
           }
        }

        [PunRPC]
        void SpawnBulletTrail(Vector3[] pos = null)
        {
            GameObject bulletTrailEffect = Instantiate(bulletTrail.gameObject, pos[0], Quaternion.identity);

            LineRenderer lineRenderer = bulletTrailEffect.GetComponent<LineRenderer>();
            
            lineRenderer.SetPosition(0, pos[0]);
            lineRenderer.SetPosition(1, pos[1]);
            
            Destroy(bulletTrailEffect, 1);
        }

        [PunRPC]
        void SpawnBulletImpact(Vector3 hitPos, Vector3 hitNormal)
        {
            GameObject hitParticleEffect = Instantiate(hitParticles, hitPos, Quaternion.FromToRotation(Vector3.up, hitNormal));
            GameObject bulletHole = Instantiate(bulletImpact, hitPos, Quaternion.FromToRotation(Vector3.forward, hitNormal));
            
            Destroy(hitParticleEffect, 2);
            Destroy(bulletHole, 2);
        }

        [PunRPC]
        void SpawnBlood(Vector3 hitPos, Vector3 hitNormal)
        {
            GameObject bloodEffect = Instantiate(bloodParticles, hitPos, Quaternion.FromToRotation(Vector3.up, hitNormal));
            
            Destroy(bloodEffect, 2);
        }

        #endregion
    }
}
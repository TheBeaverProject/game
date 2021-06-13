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

        protected override void Shoot()
        {
            // Plays shoot sound
            photonView.RPC("PlayShotSound", RpcTarget.All);
            
            readyToShoot = false;
            
            // Apply Recoil to the camera
            if (holder.playerCameraHolder != null)
            {
                holder.playerCameraHolder.GetComponent<CameraRecoil>().Recoil(aiming);
            }
            
            Vector3 direction = holder.playerCamera.transform.forward;
            
            // The raycast starting from the camera with the spread added
            if (Physics.Raycast(holder.playerCamera.transform.position, direction, out rayHit, range))
            {
                photonView.RPC("SetLineRenderer", RpcTarget.All, 2, new Vector3[] { barrelTip.transform.position, rayHit.point });

                //Damages the player if raycast catch a player
                if (rayHit.collider.TryGetComponent<PlayerManager>(out PlayerManager damagedPlayerManager))
                {
                    //Debug.Log($"Took Damage: {damagedPlayerManager.GetInstanceID()} - Health: {damagedPlayerManager.Health}");
                    //Damages the player
                    if (damagedPlayerManager != holder)
                    {
                        damagedPlayerManager.TakeDamage(damage, rayHit.collider.gameObject.layer, photonView.Owner);
                    }
                }
            }
            else
            {
                //Debug.DrawRay(holder.playerCamera.transform.position, direction * 1000, Color.red);
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
            photonView.RPC("SetLineRenderer", RpcTarget.All, 0, null);
            readyToShoot = true;
        }

        #region RPC Methods

        [PunRPC]
        void PlayShotSound()
        {
           weaponAudioSource.PlayOneShot(singleShotSoundEffect);
        }

        [PunRPC]
        void SetLineRenderer(int count, Vector3[] pos = null)
        {
            lineRenderer.positionCount = count;

            if (pos == null)
            {
                return;
            }
            
            int i = 0;
            foreach (var vector3 in pos)
            {
                lineRenderer.SetPosition(i, vector3);
                i++;
            }
        }
        
        #endregion
    }
}
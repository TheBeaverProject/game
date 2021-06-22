using System;
using Photon.Pun;
using PlayerManagement;
using UnityEngine;

namespace Guns
{
    public class GrenadeThrow : MonoBehaviourPun
    {
        public float throwForce = 20f;
        public float cooldown = 20;
        public GameObject grenadePrefab;
        public PlayerManager holder;

        private float nextThrowIn = 0;
        
        private void Start()
        {
            holder = GetComponentInParent<PlayerManager>();
            nextThrowIn = Time.time;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G) && nextThrowIn <= Time.time)
            {
                nextThrowIn = Time.time + cooldown;
                ThrowGrenade();
                holder.HUD.grenadeDisplay.DisplayCooldown(cooldown);
            }
        }

        private void ThrowGrenade()
        {
            GameObject grenade;

            Vector3 forward = holder != null
                ? holder.playerCamera.transform.forward
                : transform.forward;
            
            Vector3 pos = transform.position + (forward *  1.5f);
            
            if (PhotonNetwork.IsConnectedAndReady)
            {
                grenade = PhotonNetwork.Instantiate(grenadePrefab.name, pos, transform.rotation);
            }
            else
            {
                grenade = Instantiate(grenadePrefab, pos, transform.rotation);
            }
            
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            grenade.transform.localScale = new Vector3(2f, 2f, 2f);
            
            rb.AddForce(forward * throwForce, ForceMode.VelocityChange);
        }
    }
}
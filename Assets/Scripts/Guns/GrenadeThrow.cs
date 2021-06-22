using System;
using Photon.Pun;
using PlayerManagement;
using UnityEngine;

namespace Guns
{
    public class GrenadeThrow : MonoBehaviourPun
    {
        public float throwForce = 20f;
        public GameObject grenadePrefab;
        public PlayerManager holder;

        private void Start()
        {
            holder = GetComponentInParent<PlayerManager>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                ThrowGrenade();
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
            
            rb.AddForce(forward * throwForce, ForceMode.VelocityChange);
        }
    }
}
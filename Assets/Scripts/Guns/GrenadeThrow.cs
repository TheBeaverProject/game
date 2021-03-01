using System;
using Photon.Pun;
using UnityEngine;

namespace Guns
{
    public class GrenadeThrow : MonoBehaviourPun
    {
        public float throwForce = 20f;
        public GameObject grenadePrefab;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                ThrowGrenade();
            }
        }

        private void ThrowGrenade()
        {
            GameObject grenade = PhotonNetwork.Instantiate(grenadePrefab.name, transform.position + new Vector3(0.1f, -0.3f), transform.rotation);
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        }
    }
}
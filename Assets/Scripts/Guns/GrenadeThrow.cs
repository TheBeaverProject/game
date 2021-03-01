using System;
using Photon.Pun;
using UnityEngine;

namespace Guns
{
    public class GrenadeThrow : MonoBehaviourPun
    {
        private float throwForce = 20f;
        private GameObject grenadePrefab;

        private void Update()
        {
            if (Input.GetKey(KeyCode.G))
            {
                ThrowGrenade();
            }
        }

        private void ThrowGrenade()
        {
            GameObject grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        }
    }
}
using System;
using Photon.Pun;
using PlayerManagement;
using UnityEngine;

namespace Guns
{
    public class Grenade : MonoBehaviourPun
    {
        private float delay = 3f;
        private float countDown;
        private bool hasExploded;
        private float radius = 5f;
        private float force = 5f;
        //private GameObject effect;

        private void Start()
        {
            countDown = delay;
        }

        private void Update()
        {
            countDown -= Time.deltaTime;
            if (countDown <= 0f && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }

        private void Explode()
        {
            Debug.Log("Grenade Explosion");
            //Instantiate(effect, transform.position, transform.rotation);

            Collider[] colliders =Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (hit.TryGetComponent<PlayerManager>(out PlayerManager damagedPlayer))
                {
                    rb.AddExplosionForce(force,transform.position,radius);
                }
            }
            
            Destroy(gameObject);
        }
    }
}
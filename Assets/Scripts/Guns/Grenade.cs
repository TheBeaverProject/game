using System;
using Photon.Pun;
using PlayerManagement;
using UnityEngine;

namespace Guns
{
    public class Grenade : MonoBehaviourPun
    {
        public float delay = 3f;
        public float countDown;
        private bool hasExploded;
        public float radius = 5f;
        public float force = 400f;
        public GameObject effect;

        // Sound Effects
        public AudioClip explosionSound;

        private void Start()
        {
            //Starts grenade countdown
            countDown = delay;
        }

        private void Update()
        {
            //Updates the coutndown
            countDown -= Time.deltaTime;
            //Explodes the grenade
            if (countDown <= 0f && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }

        private void Explode()
        {
            var eff = Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(eff, delay + 3);

            //Plays Sound
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 10);

            if (photonView.IsMine) // Execute explosions damages only if we are the owner of the grenade
            {
                //Gets all collider in radius range
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

                foreach (Collider hit in colliders)
                {
                    if (hit.TryGetComponent<PlayerManager>(out PlayerManager damagedPlayer))
                    {
                        Rigidbody rb = hit.GetComponent<Rigidbody>();
                        damagedPlayer.TakeDamage(60, 10, photonView.Controller);
                    }
                }

                //Destroys object
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
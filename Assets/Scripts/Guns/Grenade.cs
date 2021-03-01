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
        private float force = 400f;
        //private GameObject effect;

        // Sound Effects
        public AudioSource grenadeAudioSource;
        public AudioClip explosionSound;
        
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
            //Instantiate(effect, transform.position, transform.rotation);
            grenadeAudioSource.PlayOneShot(explosionSound);
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hit in colliders)
            {
                if (hit.TryGetComponent<PlayerManager>(out PlayerManager damagedPlayer))
                {
                    Rigidbody rb = hit.GetComponent<Rigidbody>();
                    Debug.Log($"Grenade Explosion On Player - RigidBody: {rb.gameObject.name}");
                    rb.AddExplosionForce(force,transform.position,radius); //TODO: Explosion force not working
                    //TODO: Grenade Damages
                }
            }
            
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
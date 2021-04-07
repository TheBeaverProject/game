using System;
using Guns;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerManagement
{
    public class CameraRecoil : MonoBehaviour
    {
        protected float rotationSpeed = 6;
        protected float returnSpeed = 25;
        protected Vector3 RecoilRotation = new Vector3(2f, 2f, 2f);
        protected Vector3 RecoilRotationAiming = new Vector3(0.5f, 0.5f, 0.5f);

        private Vector3 currentRotation;
        private Vector3 rotVector;
        
        private void FixedUpdate()
        {
            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
            rotVector = Vector3.Slerp(rotVector, currentRotation, rotationSpeed * Time.fixedDeltaTime);
            this.transform.localRotation = Quaternion.Euler(rotVector);
        }
        
        /// <summary>
        /// Apply recoil from the weapon's shots to the camera
        /// </summary>
        /// <param name="aiming">Aiming state of the weapon</param>
        public void Recoil(bool aiming = false)
        {
            if (aiming)
            {
                currentRotation += new Vector3(-RecoilRotationAiming.x,
                    Random.Range(-RecoilRotationAiming.y, RecoilRotationAiming.y),
                    Random.Range(-RecoilRotationAiming.z, RecoilRotationAiming.z));
            }
            else
            {
                currentRotation += new Vector3(-RecoilRotation.x,
                    Random.Range(-RecoilRotation.y, RecoilRotation.y),
                    Random.Range(-RecoilRotation.z, RecoilRotation.z));
            }
        }

        public void SetValues(Gunnable weapon)
        {
            this.rotationSpeed = weapon.rotationSpeed;
            this.returnSpeed = weapon.returnSpeed;
            this.RecoilRotation = weapon.RecoilRotation;
            this.RecoilRotationAiming = weapon.RecoilRotationAiming;
        }
    }
}
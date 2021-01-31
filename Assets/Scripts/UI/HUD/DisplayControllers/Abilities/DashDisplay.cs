using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.HUD.DisplayControllers.Abilities
{
    public class DashDisplay : MonoBehaviour
    {
        public Slider cooldownSlider;
        public GameObject dashIcon;
        public TextMeshProUGUI cdText;
        public TextMeshProUGUI keyText;

        private float startTime;
        private float endTime;
        
        /*
         * Displays the cooldown of the ability on the HUD according to the time in seconds
         */
        public void DisplayCooldown(float cooldown)
        {
            this.startTime = Time.time;
            this.endTime = startTime + cooldown;
            
            // Hide icon
            dashIcon.SetActive(false);

            cooldownSlider.maxValue = cooldown;
        }

        // TODO: better way to display the keys from the config
        public void UpdateAssignedKey(KeyCode key)
        {
            keyText.text = key.ToString().ToUpper();
        }

        private void UpdateCooldownDisplay()
        {
            float currentTime = Time.time - this.startTime;
            float elapsedTime = this.endTime - Time.time;
            
            cooldownSlider.value = currentTime;
            cdText.text = String.Format("{0,1:00}", elapsedTime);

            if (Time.time > this.endTime)
            {
                this.startTime = 0;
                this.endTime = 0;
                
                // Resetting Slider, Text and Icon
                cdText.text = "";
                cooldownSlider.value = cooldownSlider.maxValue;
                dashIcon.SetActive(true);
            }
        }
    }
}
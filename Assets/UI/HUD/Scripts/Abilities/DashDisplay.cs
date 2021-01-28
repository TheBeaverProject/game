using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Scripts.Abilities
{
    public class DashDisplay : MonoBehaviour
    {
        public Slider cooldownSlider;
        public GameObject dashIcon;
        public TextMeshProUGUI cdText;

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

        public void UpdateCooldownDisplay()
        {
            float currentTime = Time.time - this.startTime;
            cooldownSlider.value = currentTime;
            cdText.text = String.Format("{0,1:00}", currentTime);

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

        void Update()
        {
            // values are different than 0 => cooldown is in progress
            if (startTime != 0 && endTime != 0)
            {
                UpdateCooldownDisplay();
            }
            // TODO: Remove when merging with dash
            else if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                DisplayCooldown(7f);
            }
        }
    }
}
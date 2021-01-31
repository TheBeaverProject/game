using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Scripts
{
    public class HealthDisplay : MonoBehaviour
    {
        // TODO: Remove when merging with actual controllers
        private int health = 100;
        private int maxHealth = 100;

        // TODO: Remove when merging with actual controllers
        public int Health
        {
            get => health;
            set => health = value < 0 ? 0 : value;
        }

        public TextMeshProUGUI healthText;
        public Slider healthSlider;
        public Image Fill;
        
        public Color maxHealthColor = new Color(31, 235, 248, (float) 255/2);
        public Color minHealthColor = new Color(239, 57, 87, (float) 255/2);

        /*
         * Updates the text and the health bar according to the parameter
         */
        public void SetHUDHealth(int displayHealth)
        {
            /*
             * Creates a gradient from the minHealthColor to the maxHealthColor according to the health value
             */
            Fill.color = Color.Lerp(minHealthColor, maxHealthColor, (float) health / maxHealth);
            
            healthSlider.value = displayHealth;
            healthText.text = $"{displayHealth}";
        }
        
        // TODO: Remove when merging with actual controllers
        void Start()
        {
            // Initialize the Health display with the right values & colors
            SetHUDHealth(health);
        }
        
        // TODO: Remove when merging with actual controllers
        private void Update()
        {
            // Testing purposes, will be removed in after
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Health--;
            }

            SetHUDHealth(Health);
        }
    }
}

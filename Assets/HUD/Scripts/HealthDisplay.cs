using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Scripts
{
    public class HealthDisplay : MonoBehaviour
    {
        private int health = 100;
        public Text healthText;
        public Slider healthSlider;

        public void SetUIHealth(int displayHealth)
        {
            healthSlider.value = displayHealth;
            healthText.text = $"{displayHealth}";
        }

        // Start is called before the first frame update
        void Start()
        {
            SetUIHealth(health);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                health--;
            }

            SetUIHealth(health);
        }
    }
}

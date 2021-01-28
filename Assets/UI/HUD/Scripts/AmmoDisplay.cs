using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Scripts
{
    public class AmmoDisplay : MonoBehaviour
    {
        private static int ammo = 30;
        private static int maxAmmo = 30;
        private static int mags = 3;
        
        public int Health
        {
            get => ammo;
            set => ammo = value < 0 ? 0 : value;
        }
        
        public TextMeshProUGUI magazineAmount;
        public TextMeshProUGUI magazineTotal;
        public TextMeshProUGUI magazinesLeft;
        
        public Slider ammoSlider;
        public Color ammoSliderColor = new Color(236, 211, 43, 200);
        public Image Fill;

        public void SetHUDAmmo(int magAmount, int magTotal, int magsLeft)
        {
            magazineAmount.text = $"{magAmount}";
            magazineTotal.text = $"/{magTotal}";
            magazinesLeft.text = $"x{magsLeft}";

            ammoSlider.maxValue = magTotal;
            ammoSlider.value = magAmount;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            SetHUDAmmo(ammo, maxAmmo, mags);
        }

        // Update is called once per frame
        void Update()
        {
// Testing purposes, will be removed in after
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ammo--;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ammo = maxAmmo;
                mags--;
            }

            SetHUDAmmo(ammo, maxAmmo, mags);
        }
    }
}
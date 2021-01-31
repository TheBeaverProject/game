using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Scripts
{
    public class AmmoDisplay : MonoBehaviour
    {
        // TODO: Remove when merging with actual controllers
        private static int ammo = 30;
        private static int maxAmmo = 30;
        private static int mags = 3;

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
        
        // TODO: Remove when merging with actual controllers
        void Start()
        {
            SetHUDAmmo(ammo, maxAmmo, mags);
        }

        // TODO: Remove when merging with actual controllers
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ammo--;
                
                SetHUDAmmo(ammo, maxAmmo, mags);
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                ammo = maxAmmo;
                mags--;
                
                SetHUDAmmo(ammo, maxAmmo, mags);
            }
        }
    }
}
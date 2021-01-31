using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.HUD.DisplayControllers
{
    public class AmmoDisplay : MonoBehaviour
    {
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
    }
}
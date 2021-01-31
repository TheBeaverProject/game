using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.HUD.Methods
{ 
    public class Healthbar
    {
        public static int maxHealth = 100;
        public static Color maxHealthColor = new Color32(31, 235, 248, 255/2);
        public static Color minHealthColor = new Color32(239, 57, 87, 255/2);

        public static void UpdateHealth(Slider slider, int health)
        {
            slider.value = health;
            /*
             * Creates a gradient from the minHealthColor to the maxHealthColor according to the health value
             */
            slider.GetComponentInChildren<Image>().color = Color32.Lerp(minHealthColor, maxHealthColor, (float) health / maxHealth);
        }
    }
}
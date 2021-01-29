using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Scripts
{
    public class WeaponDisplay : MonoBehaviour
    {
        public TextMeshProUGUI  text1;
        public TextMeshProUGUI  text2;
        public TextMeshProUGUI  text3;

        public TextMeshProUGUI weaponNameText;

        public void ToggleSelectedWeapon(int selected, string weaponName)
        {
            text1.color = Color.white;
            text2.color = Color.white;
            text3.color = Color.white;

            weaponNameText.text = weaponName.ToUpper();

            Color32 selectedColor = new Color32(236, 211, 43, 200);
            
            switch (selected)
            {
                case 1:
                    text1.color = selectedColor;
                    break;
                case 2:
                    text2.color = selectedColor;
                    break;
                case 3:
                    text3.color = selectedColor;
                    break;
            }
        }
        
        // TODO: Remove when merging with actual controllers
        void Start()
        {
            ToggleSelectedWeapon(1, "AK 47");
        }

        // TODO: Remove when merging with actual controllers
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                ToggleSelectedWeapon(1, "AK 47");
            if (Input.GetKeyDown(KeyCode.Alpha2))
                ToggleSelectedWeapon(2, "Glock 18");
            if (Input.GetKeyDown(KeyCode.Alpha3))
                ToggleSelectedWeapon(3, "Knife");
        }
    }
}
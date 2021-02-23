using TMPro;
using UnityEngine;

namespace UI.HUD.DisplayControllers
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
    }
}
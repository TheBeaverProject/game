using System;
using Guns;
using Scripts.UI.HUD.DisplayControllers;
using TMPro;
using UI.HUD.DisplayControllers;
using UnityEngine;

namespace UI.HUD
{
    public class Controller : MonoBehaviour
    {
        public TextMeshProUGUI playerName;
        
        public AmmoDisplay ammoDisplay;
        public HealthDisplay healthDisplay;
        public RoundDisplay roundDisplay;
        public TeamDisplay teamDisplay;
        public WeaponDisplay weaponDisplay;
        
        public static void Init()
        {
            throw new NotImplementedException();
        }

        public void UpdateWeaponDisplay(Gunnable gun)
        {
            ammoDisplay.SetHUDAmmo(gun.GetMagLeft, gun.GetMagSize, 3);
            weaponDisplay.weaponNameText.text = gun.name;
        }
    }
}
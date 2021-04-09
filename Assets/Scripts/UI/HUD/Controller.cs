using System;
using Guns;
using Scripts.UI.HUD.DisplayControllers;
using TMPro;
using UI.HUD.DisplayControllers;
using UnityEngine;

namespace UI.HUD
{
    public enum HUDType
    {
        Deathmatch,
        TeamDeathmatch,
    }
    
    public class Controller : MonoBehaviour
    {
        public TextMeshProUGUI playerName;
        
        public AmmoDisplay ammoDisplay;
        public HealthDisplay healthDisplay;
        public RoundDisplay roundDisplay;
        public CountdownDisplay countdownDisplay;
        public TeamDisplay teamDisplay;
        public WeaponDisplay weaponDisplay;
        public GameObject crossHair;
        
        public void Init(HUDType type)
        {
            switch (type)
            {
                case HUDType.Deathmatch:
                    teamDisplay.gameObject.SetActive(false);
                    roundDisplay.gameObject.SetActive(false);
                    countdownDisplay.gameObject.SetActive(true);
                    break;
                case HUDType.TeamDeathmatch:
                    roundDisplay.gameObject.SetActive(false);
                    teamDisplay.gameObject.SetActive(true);
                    countdownDisplay.gameObject.SetActive(true);
                    break;
            }
        }

        public void UpdateCountdown(double timeInSeconds)
        {
            countdownDisplay.UpdateTime(timeInSeconds);
        }

        public void UpdateWeaponDisplay(Gunnable gun)
        {
            ammoDisplay.SetHUDAmmo(gun.GetBulletsLeft, gun.GetMagSize, gun.GetMagLeft);
            weaponDisplay.weaponNameText.text = gun.weaponName;
        }

        public void DisplayCrosshair(bool display)
        {
            crossHair.SetActive(display);
        }
    }
}
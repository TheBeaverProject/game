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
        public ScoreboardController ScoreBoard;
        public TextMeshProUGUI playerName;
        public AmmoDisplay ammoDisplay;
        public HealthDisplay healthDisplay;
        public RoundDisplay roundDisplay;
        public CountdownDisplay countdownDisplay;
        public TeamDisplay teamDisplay;
        public WeaponDisplay weaponDisplay;
        public TeamPointsDisplay teamPointsDisplay;
        public GameObject crossHair;

        public HUDType currentType;

        private void Start()
        {
            ScoreBoard.Container.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                ScoreBoard.Container.SetActive(true);
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                ScoreBoard.Container.SetActive(false);
            }
        }

        public void Init(HUDType type)
        {
            currentType = type;
            
            switch (type)
            {
                case HUDType.Deathmatch:
                    teamDisplay.gameObject.SetActive(false);
                    roundDisplay.gameObject.SetActive(false);
                    teamPointsDisplay.gameObject.SetActive(false);
                    countdownDisplay.gameObject.SetActive(true);
                    break;
                case HUDType.TeamDeathmatch:
                    roundDisplay.gameObject.SetActive(false);
                    teamDisplay.gameObject.SetActive(true);
                    countdownDisplay.gameObject.SetActive(true);
                    teamPointsDisplay.gameObject.SetActive(true);
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

        public void SetTeamPoints(int bluePoints, int redPoints)
        {
            if (currentType != HUDType.TeamDeathmatch)
            {
                return;
            }
            
            teamPointsDisplay.SetPoints(bluePoints, redPoints);
        }
    }
}
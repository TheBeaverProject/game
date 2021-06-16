using System;
using System.Collections.Generic;
using Guns;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Scripts;
using Scripts.UI.HUD.DisplayControllers;
using TMPro;
using UI.HUD.DisplayControllers;
using UnityEngine;

namespace UI.HUD
{
    public enum HUDType
    {
        FFA,
        Teams,
        Rounds
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
        public KillFeedDisplay killFeedDisplay;
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

        /// <summary>
        /// Sets the HUD layout according to the type of game
        /// </summary>
        /// <param name="type"></param>
        public void Init(HUDType type)
        {
            currentType = type;
            
            switch (type)
            {
                case HUDType.FFA:
                    teamDisplay.gameObject.SetActive(false);
                    roundDisplay.gameObject.SetActive(false);
                    teamPointsDisplay.gameObject.SetActive(false);
                    countdownDisplay.gameObject.SetActive(true);
                    break;
                case HUDType.Teams:
                    roundDisplay.gameObject.SetActive(false);
                    teamDisplay.gameObject.SetActive(true);
                    countdownDisplay.gameObject.SetActive(true);
                    teamPointsDisplay.gameObject.SetActive(true);
                    break;
                case HUDType.Rounds:
                    teamPointsDisplay.gameObject.SetActive(false);
                    teamDisplay.gameObject.SetActive(true);
                    roundDisplay.gameObject.SetActive(true);
                    countdownDisplay.gameObject.SetActive(true);
                    break;
            }
        }

        public void UpdateRounds(int team1Rounds = 0, int team2Rounds = 0)
        {
            if (team1Rounds != 0)
                roundDisplay.SetRounds(0, team1Rounds);
            
            if (team2Rounds != 0)
                roundDisplay.SetRounds(1, team2Rounds);
        }

        /// <summary>
        /// Update the countdown displaying the remaining/elapsed time of the game
        /// </summary>
        /// <param name="timeInSeconds">remaining/elapsed time in seconds</param>
        public void UpdateCountdown(double timeInSeconds)
        {
            countdownDisplay.UpdateTime(timeInSeconds);
        }

        /// <summary>
        /// Updates the main weapon display
        /// </summary>
        /// <param name="gun">gun used to update the display</param>
        public void UpdateWeaponDisplay(Gunnable gun)
        {
            ammoDisplay.SetHUDAmmo(gun.GetBulletsLeft, gun.GetMagSize, gun.GetMagLeft);
            weaponDisplay.weaponNameText.text = gun.weaponName;
        }

        /// <summary>
        /// Displays or hide the crosshair
        /// </summary>
        /// <param name="display"></param>
        public void DisplayCrosshair(bool display)
        {
            crossHair.SetActive(display);
        }

        /// <summary>
        /// Updates the team points on the points display
        /// </summary>
        /// <param name="bluePoints">team blue points</param>
        /// <param name="redPoints">team red points</param>
        public void UpdateTeamPoints(int bluePoints, int redPoints)
        {
            if (currentType != HUDType.Teams)
            {
                return;
            }
            
            teamPointsDisplay.SetPoints(bluePoints, redPoints);
        }

        public void AddKillFeedElement(Player killer, Player assist, Player killed)
        {
            var team = killer.GetPhotonTeam();
            Color iconColor = team == null ? new Color(246, 235, 20, 255) : team.Color;
            killFeedDisplay.AddElement(killer.NickName, killed.NickName, iconColor, killer.IsLocal, killed.IsLocal);
        }

        public struct HUDPlayerInfo
        {
            public string nickname;
            public string icon;
            public int health;
        }

        public void UpdateTeammatesInfo(List<PlayerData> teammates)
        {
            var teammatesHudInfo = new List<HUDPlayerInfo>();

            foreach (var teammate in teammates)
            {
                teammatesHudInfo.Add(new HUDPlayerInfo
                {
                    nickname = teammate.name,
                    icon = teammate.iconUrl,
                    health = teammate.alive ? 100 : 0
                });
            }
        }
    }
}
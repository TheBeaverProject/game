using System;
using TMPro;
using UnityEngine;

namespace UI.HUD.DisplayControllers
{
    public class RoundDisplay : MonoBehaviour
    {
        public TextMeshProUGUI blueText;
        public TextMeshProUGUI redText;

        public int blueRounds = 0;
        public int redRounds = 0;

        public void AddRound(int team)
        {
            switch (team)
            {
                case 0:
                    blueRounds++;
                    blueText.text = String.Format("{0,2:00}", blueRounds);
                    break;
                case 1:
                    redRounds++;
                    redText.text = String.Format("{0,2:00}", redRounds);;
                    break;
            }
        }
    }
}
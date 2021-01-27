using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Scripts
{
    public class RoundDisplay : MonoBehaviour
    {
        public Text blueText;
        public Text redText;

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
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
                AddRound(0);
            if (Input.GetKeyDown(KeyCode.N))
                AddRound(1);
        }
    }
}
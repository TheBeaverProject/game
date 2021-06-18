using System;
using System.Collections.Generic;
using Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

namespace UI.HUD.DisplayControllers
{
    public class TeamDisplay : MonoBehaviour
    {
        public GameObject[] playerContainers = new GameObject[4];

        public void Init()
        {
            foreach (var playerContainer in playerContainers)
            {
                playerContainer.gameObject.SetActive(false);
            }
        }
        
        public void UpdatePlayerHealth(int id, int newHealth)
        {
            if (id < 0 || id >= playerContainers.Length)
                return;
            Slider slider = playerContainers[id].GetComponentInChildren<Slider>();
            Scripts.UI.HUD.Methods.Healthbar.UpdateHealth(slider, newHealth);
        }
        
        public void UpdatePlayerName(int id, string newName)
        {
            if (id < 0 || id >= playerContainers.Length)
                return;
            playerContainers[id].GetComponentInChildren<TextMeshProUGUI>().text = newName.ToUpper();
        }
        
        public void UpdatePlayerIcon(int id, Sprite icon)
        {
            if (id < 0 || id >= playerContainers.Length)
                return;
            
            playerContainers[id].GetComponentInChildren<UnityEngine.UI.Image>().sprite = icon;
        }
        
        public void UpdateTeammates(List<Controller.HUDPlayerInfo> teammates)
        {
            Init();
            
            for (int i = 0; i < teammates.Count; i++)
            {
                var teammate = teammates[i];
                playerContainers[i].gameObject.SetActive(true);
                UpdatePlayerHealth(i, teammate.health);
                UpdatePlayerName(i, teammate.nickname);
                UpdatePlayerIcon(i, teammate.icon);
            }
        }
    }
}


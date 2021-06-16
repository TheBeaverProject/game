using System.Collections.Generic;
using Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.DisplayControllers
{
    public class TeamDisplay : MonoBehaviour
    {
        public GameObject[] playerContainers = new GameObject[4];

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
        
        public void UpdatePlayerIcon(int id, Sprite sprite)
        {
            if (id < 0 || id >= playerContainers.Length)
                return;
            playerContainers[id].GetComponentInChildren<UnityEngine.UI.Image>().sprite = sprite;
        }
        
        public void UpdateTeammates(List<Controller.HUDPlayerInfo> teammates)
        {
            for (int i = 0; i < teammates.Count; i++)
            {
                var teammate = teammates[i];
                UpdatePlayerHealth(i, teammate.health);
                UpdatePlayerName(i, teammate.nickname);

                StartCoroutine(Utils.GetSpriteFromUrl(teammate.icon, (sprite) =>
                {
                    UpdatePlayerIcon(i, sprite);
                }));
            }
        }
    }
}


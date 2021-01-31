using Unity.Entities;
using Unity.Jobs;

namespace src.UI.HUD.Systems
{
    public class TeamUpdateSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // TODO: Implement systems according to player data

            return inputDeps;
        }
        
        /*
        TODO: IMPLEMENT THESES \/
        public void UpdatePlayerHealth(int id, int newHealth)
        {
            if (id < 1 || id > playerContainers.Length)
                return;
            Slider slider = playerContainers[id - 1].GetComponentInChildren<Slider>();

            UI.HUD.Scripts.Controllers.HealthBarController.UpdateHealth(slider, newHealth);
        }
        
        public void UpdatePlayerName(int id, string newName)
        {
            if (id < 1 || id > playerContainers.Length)
                return;

            playerContainers[id - 1].GetComponentInChildren<TextMeshProUGUI>().text = newName.ToUpper();
        }
        
        public void UpdatePlayerIcon(int id, Sprite sprite)
        {
            if (id < 1 || id > playerContainers.Length)
                return;

            playerContainers[id - 1].GetComponentInChildren<UnityEngine.UI.Image>().sprite = sprite;
        }
         */
    }
}
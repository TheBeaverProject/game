using Unity.Entities;
using Unity.Jobs;

namespace src.UI.HUD.Systems
{
    public class HealthUpdateSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // TODO: Implement systems according to player data

            return inputDeps;
        }
        
        /* TODO: Implement this \/
         public void SetHUDHealth(int displayHealth)
        {
            UI.HUD.Scripts.Controllers.HealthBarController.UpdateHealth(healthSlider, displayHealth);
            healthText.text = $"{displayHealth}";
        }
        */
    }
}
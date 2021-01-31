using Unity.Entities;
using Unity.Jobs;

namespace src.UI.HUD.Systems
{
    public class AmmoUpdateSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // TODO: Implement systems according to player data

            return inputDeps;
        }
        
        /* TODO: Implement this \/
        private void SetHUDAmmo(int ammoAmount, int ammoTotal, int magsLeft)
        {
            magazineAmount.text = $"{ammoAmount}";
            magazineTotal.text = $"/{ammoTotal}";
            magazinesLeft.text = $"x{magsLeft}";

            ammoSlider.maxValue = ammoTotal;
            ammoSlider.value = ammoAmount;
        }
        */
    }
}
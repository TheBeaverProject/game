using Unity.Entities;
using Unity.Jobs;

namespace src.UI.HUD.Systems
{
    public class RoundUpdateSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // TODO: Implement systems according to player data

            return inputDeps;
        }
        
        /*
         TODO: Implement this \/
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
        */
    }
}
using Unity.Entities;
using Unity.Jobs;

namespace src.UI.HUD.Systems
{
    public class WeaponUpdateSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // TODO: Implement systems according to player data

            return inputDeps;
        }
        
        /*
        TODO: Impement this \/
        public void ToggleSelectedWeapon(int selected, string weaponName)
        {
            text1.color = Color.white;
            text2.color = Color.white;
            text3.color = Color.white;

            weaponNameText.text = weaponName.ToUpper();

            Color32 selectedColor = new Color32(236, 211, 43, 200);
            
            switch (selected)
            {
                case 1:
                    text1.color = selectedColor;
                    break;
                case 2:
                    text2.color = selectedColor;
                    break;
                case 3:
                    text3.color = selectedColor;
                    break;
            }
        }        
        */
    }
}
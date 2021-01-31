using TMPro;
using Unity.Entities;

namespace src.UI.HUD.Components
{
    [GenerateAuthoringComponent]
    public struct WeaponComponent : IComponentData
    {
        public TextMeshProUGUI text1;
        public TextMeshProUGUI text2;
        public TextMeshProUGUI text3;

        public TextMeshProUGUI weaponNameText;
    }
}
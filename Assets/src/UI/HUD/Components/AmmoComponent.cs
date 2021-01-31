using System;
using TMPro;
using Unity.Entities;
using UnityEngine.UI;

namespace src.UI.HUD.Components
{
    [GenerateAuthoringComponent]
    public struct AmmoComponent : IComponentData
    {
        public TextMeshProUGUI magazineAmount;
        public TextMeshProUGUI magazineTotal;
        public TextMeshProUGUI magazinesLeft;
        
        public Slider ammoSlider;
        public Image Fill;
    }
}
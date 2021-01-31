using System;
using TMPro;
using Unity.Entities;
using UnityEngine.UI;

namespace src.UI.HUD.Components
{
    [GenerateAuthoringComponent]
    public struct HealthComponent : IComponentData
    {
        public TextMeshProUGUI healthText;
        public Slider healthSlider;
    }
}
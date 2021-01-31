using System;
using TMPro;
using Unity.Entities;

namespace src.UI.HUD.Components
{
    [GenerateAuthoringComponent]
    public struct RoundComponent : IComponentData
    {
        public TextMeshProUGUI blueText;
        public TextMeshProUGUI redText;
    }
}
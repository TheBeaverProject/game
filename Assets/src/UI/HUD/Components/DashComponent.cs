using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace src.UI.HUD.Components
{
    [GenerateAuthoringComponent]
    public struct DashComponent
    {
        public Slider cooldownSlider;
        public GameObject dashIcon;
        public TextMeshProUGUI cdText;
        public TextMeshProUGUI keyText;
    }
}
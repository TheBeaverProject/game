using System;
using UnityEngine;
using Unity.Entities;

namespace src.UI.HUD.Components
{
    [GenerateAuthoringComponent]
    public struct TeamComponent : IComponentData
    {
        public GameObject playerContainer1;
        public GameObject playerContainer2;
        public GameObject playerContainer3;
        public GameObject playerContainer4;
    }
}
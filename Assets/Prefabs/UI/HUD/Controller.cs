using System;
using Prefabs.UI.HUD.DisplayControllers;
using UnityEngine;

namespace Prefabs.UI.HUD
{
    public class Controller : MonoBehaviour
    {
        public AmmoDisplay ammoDisplay;
        public HealthDisplay healthDisplay;
        public RoundDisplay roundDisplay;
        public TeamDisplay teamDisplay;
        public WeaponDisplay weaponDisplay;
        
        public static void Init()
        {
            throw new NotImplementedException();
        }
    }
}
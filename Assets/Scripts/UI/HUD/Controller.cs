using System;
using Scripts.UI.HUD.DisplayControllers;
using UI.HUD.DisplayControllers;
using UnityEngine;

namespace Scripts.UI.HUD
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
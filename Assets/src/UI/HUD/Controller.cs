using System;
using src.UI.HUD.DisplayControllers;
using UI.HUD.Scripts;
using UnityEngine;

namespace src.UI.HUD
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
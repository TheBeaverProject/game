using System;
using Guns;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.BuyMenu
{
    public class WeaponPreviewHandler: MonoBehaviour, IPointerEnterHandler
    {
        public GameObject weaponPrefab;
        public Gunnable weaponScript;
        public WeaponPreview Preview;

        private GameObject instancied;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            Preview.DisplayWeapon(weaponPrefab, weaponScript);
        }
    }
}
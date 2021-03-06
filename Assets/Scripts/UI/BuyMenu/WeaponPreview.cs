using Guns;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BuyMenu
{
    public class WeaponPreview: MonoBehaviour
    {
        public TextMeshProUGUI weaponName;
        public Slider damageSlider;
        public Slider spreadSlider;
        public TextMeshProUGUI ammoText;
        public TextMeshProUGUI modeText;
        
        private GameObject instancied;

        public void DisplayWeapon(GameObject weaponPrefab, Gunnable weaponScript)
        {
            var rotation = Quaternion.identity;
            
            if (instancied != null)
            {
                rotation = instancied.transform.rotation;
                Destroy(instancied);
            }
            
            instancied = Instantiate(weaponPrefab, Camera.current.transform);
            instancied.transform.localPosition = new Vector3(0, 0, 2);
            instancied.transform.localScale = new Vector3(2, 2, 2);
            instancied.transform.rotation = rotation;

            weaponName.text = weaponScript.weaponName;
            damageSlider.value = weaponScript.damage;
            spreadSlider.value = weaponScript.spread;
            ammoText.text = $"{weaponScript.GetMagSize}x{weaponScript.GetMagLeft}";
            modeText.text =
                weaponScript.allowButtonHold ?
                    "Automatic" : weaponScript.bulletsPerTap > 1 ?
                        $"Burst x{weaponScript.bulletsPerTap}" : "Semi Automatic";
        }

        public void ClosePreview()
        {
            if (instancied != null)
            {
                Destroy(instancied);
            }
        }
        
        private void Update()
        {
            if (instancied)
            {
                instancied.transform.Rotate(new Vector3(0, 20 * Time.deltaTime, 0));
            }
        }
    }
}
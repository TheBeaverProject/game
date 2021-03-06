using System;
using System.Collections.Generic;
using Guns;
using PlayerManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.BuyMenu
{
    public class BuyMenuController : MonoBehaviour
    {
        public static List<string> Step1Categories = new List<string>()
        {
            "Pistols",
            "SMGs",
            "Rifles",
            "HMGs",
            "Shotguns",
            "Snipers",
            "Grenades"
        };

        public PlayerManager playerManager;
        public GameObject Container;

        public GameObject Step1Container;
        public Button Step1Button;
        
        public GameObject Step2Container;
        public Button Step2Button;

        public GameObject PreviewContainer;

        public List<GameObject> PistolsList;
        public List<GameObject> SMGsList;
        public List<GameObject> RiflesList;
        public List<GameObject> HMGsList;
        public List<GameObject> ShotgunsList;
        public List<GameObject> SnipersList;
        public List<GameObject> GrenadesList;

        private Dictionary<string, List<GameObject>> categoriesAssociation = new Dictionary<string, List<GameObject>>();

        private void Start()
        {
            Container.SetActive(false);
            
            // Create the dictionnary liking weapons and their categories
            categoriesAssociation.Add(Step1Categories[0], PistolsList);
            categoriesAssociation.Add(Step1Categories[1], SMGsList);
            categoriesAssociation.Add(Step1Categories[2], RiflesList);
            categoriesAssociation.Add(Step1Categories[3], HMGsList);
            categoriesAssociation.Add(Step1Categories[4], ShotgunsList);
            categoriesAssociation.Add(Step1Categories[5], SnipersList);
            categoriesAssociation.Add(Step1Categories[6], GrenadesList);

            Vector3 localPosition = Step1Button.transform.localPosition;

            // Populates the Buttons of the Step 1 according to the categories
            foreach (var step1Category in Step1Categories)
            {
                GameObject newObj = Instantiate(Step1Button.gameObject, Step1Container.transform);
                newObj.transform.localPosition = localPosition;
                localPosition.y -= 70;
                
                var newButton = newObj.GetComponent<Button>();

                newButton.name = step1Category;
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = step1Category;

                UnityAction action = () =>
                {
                    SelectStep1(step1Category);
                };

                newButton.onClick.AddListener(action);
            }
            
            // Deactivates the sample Buttons
            Step1Button.gameObject.SetActive(false);
            Step2Button.gameObject.SetActive(false);
        }

        private List<GameObject> Step2Buttons = new List<GameObject>();
        
        // Selects the first step of the Menu and displays the buttons of the second step
        private void SelectStep1(string step)
        {
            Debug.Log($"Selected Step 2 {step}");
            Step2Button.gameObject.SetActive(true);

            // Destroy Previous Buttons
            foreach (var obj in Step2Buttons)
                Destroy(obj);
            
            // Clear the list
            Step2Buttons = new List<GameObject>();
            
            Vector3 localPosition = Step2Button.transform.localPosition;
            
            // Add every button
            foreach (var weapon in categoriesAssociation[step])
            {
                // Do this to handle the case where weapons will use different scripts
                Gunnable weaponScript = null;
                if (weapon.TryGetComponent(out HitScanWeapon hitScanWeaponScript))
                    weaponScript = hitScanWeaponScript;
                if (weaponScript == null) // Continue if no script on the prefab
                    continue;
                
                GameObject newObj = Instantiate(Step2Button.gameObject, Step2Container.transform);
                // Initialize preview on hover Script
                var weaponPreview = newObj.AddComponent<WeaponPreviewHandler>();
                weaponPreview.weaponPrefab = weapon;
                weaponPreview.weaponScript = weaponScript;
                weaponPreview.Preview = PreviewContainer.GetComponent<WeaponPreview>();
                var newButton = newObj.GetComponent<Button>();
                
                newObj.transform.localPosition = localPosition;
                Step2Buttons.Add(newObj);

                newButton.name = weaponScript.name;
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = weaponScript.weaponName;
                newButton.onClick.AddListener(() => BuyWeapon(weapon));
                
                localPosition.y -= 70;
            }
            
            Step2Button.gameObject.SetActive(false);
        }

        private void BuyWeapon(GameObject weaponPrefab)
        {
            if (playerManager == null) return;
            
            Debug.Log($"Bought Weapon {weaponPrefab.name}");
            playerManager.AddGunPrefabToPlayer(weaponPrefab);
        }
    }
}
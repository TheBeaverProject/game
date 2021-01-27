using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Scripts
{
    public class WeaponDisplay : MonoBehaviour
    {
        public GameObject  selected1;
        public GameObject  selected2;
        public GameObject  selected3;

        public void ToggleSelectedWeapon(int selected)
        {
            selected1.SetActive(false);
            selected2.SetActive(false);
            selected3.SetActive(false);
            
            switch (selected)
            {
                case 1:
                    selected1.SetActive(true);
                    break;
                case 2:
                    selected2.SetActive(true);
                    break;
                case 3:
                    selected3.SetActive(true);
                    break;
            }
        }
        
        // Start is called before the first frame update
        void Start()
        {
            ToggleSelectedWeapon(1);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                ToggleSelectedWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                ToggleSelectedWeapon(2);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                ToggleSelectedWeapon(3);
        }
    }
}
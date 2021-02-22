using System;
using UnityEngine;

namespace UI.EscapeMenu
{
    public class EscapeMenuHandler : MonoBehaviour
    {
        public GameObject EscapeMenuContainer;

        public void ResumeButtonHandler()
        {
            EscapeMenuContainer.SetActive(false);
        }

        private void Start()
        {
            EscapeMenuContainer.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EscapeMenuContainer.SetActive(!EscapeMenuContainer.activeInHierarchy);
            }
        }
    }
}

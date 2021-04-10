using System;
using TMPro;
using UnityEngine;

namespace UI.MainMenu.Tabs.HomeTab
{
    public class QuickPlayButtonHandler : MonoBehaviour
    {
        public GamemodeSelection Controller;
        public TextMeshProUGUI ModeText;

        private void Update()
        {
            ModeText.text = Controller.SelectedGameMode.ToString();
        }
    }
}
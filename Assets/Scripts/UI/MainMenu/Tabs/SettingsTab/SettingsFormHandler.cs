using Firebase;
using UnityEngine;

namespace UI.MainMenu.Tabs.SettingsTab
{
    public class SettingsFormHandler : MonoBehaviour
    {
        public void OnLogoutClick()
        {
            AuthHandler.LogOut();
        }
    }
}
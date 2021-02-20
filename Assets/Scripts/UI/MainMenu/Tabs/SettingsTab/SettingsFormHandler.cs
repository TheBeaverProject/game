using Firebase;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.MainMenu.Tabs.SettingsTab
{
    public class SettingsFormHandler : MonoBehaviour
    {
        public void OnLogoutClick()
        {
            AuthHandler.LogOut();
            SceneManager.LoadScene("Authentication");
        }
    }
}
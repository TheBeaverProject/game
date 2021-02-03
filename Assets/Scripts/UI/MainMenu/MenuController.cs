using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
    public class MenuController : MonoBehaviour
    {
        public void AccountHandler()
        {
            
        }

        public void SoloHandler()
        {
            SceneManager.LoadScene("Scenes/InGameHud");
        }

        public void MultiplayerHandler()
        {
            SceneManager.LoadScene("Scenes/InGameHud");
        }

        public void SettingsHandler()
        {
            
        }

        public void QuitHandler()
        {
            Application.Quit();
        }
    }
    
}

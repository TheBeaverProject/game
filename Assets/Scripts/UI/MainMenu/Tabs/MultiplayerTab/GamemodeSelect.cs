using Scripts.Gamemodes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu.Tabs.MultiplayerTab
{
    public class GamemodeSelect : MonoBehaviour
    {
        public GamemodeSelection Controller;
        public Toggle Toggle;
        public Mode GameMode;

        public void Select()
        {
            if (Toggle.isOn)
            {
                Toggle.isOn = false;
                return;
            }
            
            Controller.SelectGameMode(this);
            
            Toggle.isOn = true;
        }
    }
}
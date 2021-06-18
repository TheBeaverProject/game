using System.Collections;
using System.Collections.Generic;
using Scripts.Gamemodes;
using UI.MainMenu.Tabs.MultiplayerTab;
using UnityEngine;

public class GamemodeSelection : MonoBehaviour
{
    public Type SelectedGameMode;

    public void SelectGameMode(GamemodeSelect gamemodeSelect)
    {
        var Selects = GetComponentsInChildren<GamemodeSelect>();

        foreach (var @select in Selects)
        {
            @select.Toggle.isOn = false;
        }

        SelectedGameMode = gamemodeSelect.GameMode;
    }
}

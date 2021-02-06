using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

namespace UI.MainMenu.Tabs.MultiplayerTab
{
    /// <summary>
    /// Player name temporary input field before integration with firebase
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    public class PlayerNameInputField : MonoBehaviour
    { 
        private void OnEnable()
        {
            string defaultName = "";
            TMP_InputField inputField = this.GetComponent<TMP_InputField>();

            if (inputField != null)
            {
                if (PlayerPrefs.HasKey(PlayerPrefKeys.PlayerName))
                {
                    defaultName = PlayerPrefs.GetString(PlayerPrefKeys.PlayerName);
                    inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        public void SetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }

            PhotonNetwork.NickName = value;
            
            PlayerPrefs.SetString(PlayerPrefKeys.PlayerName, value);
        }
    }
}
